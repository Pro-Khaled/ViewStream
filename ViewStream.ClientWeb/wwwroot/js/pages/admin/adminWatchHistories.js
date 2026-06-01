pages.adminWatchHistories = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'watchedAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/watch-histories', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load watch history: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function deleteEntry(id) {
        if (!await modal.confirm('Delete History Entry', 'Remove this watch history entry?', 'danger')) return;
        try { await api.delete(`/admin/watch-histories/${id}`); toast.success('Entry deleted'); loadData(); }
        catch (err) { toast.error('Failed to delete: ' + err.message); }
    }

    function progressBar(pct) {
        const p = Math.min(100, Math.max(0, Math.round(pct || 0)));
        const color = p >= 90 ? 'bg-vs-success' : p >= 50 ? 'bg-vs-accent' : 'bg-vs-warning';
        return `<div class="flex items-center gap-2">
            <div class="w-20 h-1.5 bg-vs-border rounded-full overflow-hidden">
                <div class="${color} h-full rounded-full" style="width:${p}%"></div>
            </div>
            <span class="text-xs text-muted">${p}%</span>
        </div>`;
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Watch History', 'Review content viewing history across all users');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search History</label>
                    <input class="input-field" id="wh-search" value="${utils.esc(state.search)}" placeholder="Search by user email or content...">
                </div>
                <button class="btn btn-primary btn-sm" id="wh-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-history', 'No watch history found'); }
        else {
            const rows = state.data.items.map(w => `<tr>
                <td class="font-medium">${utils.esc(w.userEmail || '—')}</td>
                <td>${utils.esc(w.contentTitle || w.episodeTitle || '—')}</td>
                <td><span class="badge badge-info">${utils.esc(w.contentType || 'Unknown')}</span></td>
                <td class="font-mono text-sm">${w.watchedDurationSeconds != null ? utils.formatDuration(w.watchedDurationSeconds) : '—'}</td>
                <td>${progressBar(w.progressPercent)}</td>
                <td>${w.isCompleted ? '<span class="badge badge-success">Completed</span>' : '<span class="badge badge-muted">In Progress</span>'}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(w.watchedAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger del-wh-btn" data-id="${w.id}" title="Delete"><i class="fas fa-trash text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'userEmail', label: 'User' }, { key: 'contentTitle', label: 'Content' }, { key: 'contentType', label: 'Type' }, { key: 'watchedDurationSeconds', label: 'Duration' }, { key: 'progressPercent', label: 'Progress' }, { key: 'isCompleted', label: 'Status' }, { key: 'watchedAt', label: 'Watched' }, { key: '', label: '' }],
                rows, 'No history entries',
                { tableId: 'wh-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('wh-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('wh-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-wh-btn').forEach(b => b.addEventListener('click', () => deleteEntry(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
