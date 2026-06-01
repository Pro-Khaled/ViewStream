pages.adminOfflineDownloads = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'downloadedAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/offlinedownloads', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load offline downloads: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function deleteDownload(id) {
        if (!await modal.confirm('Delete Download', 'Remove this offline download record?', 'danger')) return;
        try { await api.delete(`/admin/offlinedownloads/${id}`); toast.success('Download removed'); loadData(); }
        catch (err) { toast.error('Failed to remove: ' + err.message); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Offline Downloads', 'Track content downloaded for offline viewing');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Downloads</label>
                    <input class="input-field" id="od-search" value="${utils.esc(state.search)}" placeholder="Search by user or content...">
                </div>
                <button class="btn btn-primary btn-sm" id="od-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-download', 'No offline downloads found'); }
        else {
            const rows = state.data.items.map(d => `<tr>
                <td class="text-muted font-mono">#${d.id}</td>
                <td><span class="font-medium">${utils.esc(d.userEmail || '—')}</span></td>
                <td>${utils.esc(d.contentTitle || d.episodeTitle || '—')}</td>
                <td><span class="badge badge-info">${utils.esc(d.contentType || 'Unknown')}</span></td>
                <td class="text-muted text-sm">${d.fileSizeMb ? d.fileSizeMb.toFixed(1) + ' MB' : '—'}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(d.downloadedAt)}</td>
                <td class="text-muted text-sm">${d.expiresAt ? utils.formatDateShort(d.expiresAt) : '—'}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger del-od-btn" data-id="${d.id}" title="Remove"><i class="fas fa-trash text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'userEmail', label: 'User' }, { key: 'contentTitle', label: 'Content' }, { key: 'contentType', label: 'Type' }, { key: 'fileSizeMb', label: 'Size' }, { key: 'downloadedAt', label: 'Downloaded' }, { key: 'expiresAt', label: 'Expires' }, { key: '', label: '' }],
                rows, 'No downloads found',
                { tableId: 'od-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('od-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('od-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-od-btn').forEach(b => b.addEventListener('click', () => deleteDownload(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
