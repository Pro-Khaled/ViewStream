pages.adminWatchParties = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc', statusFilter: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/watchparties', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                status: state.statusFilter || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load watch parties: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function endParty(id) {
        if (!await modal.confirm('End Watch Party', 'Force-end this watch party session?', 'warning')) return;
        try { await api.post(`/admin/watchparties/${id}/end`); toast.success('Watch party ended'); loadData(); }
        catch (err) { toast.error('Failed to end party: ' + err.message); }
    }

    async function deleteParty(id) {
        if (!await modal.confirm('Delete Watch Party', 'Permanently delete this watch party and all its participants?', 'danger')) return;
        try { await api.delete(`/admin/watchparties/${id}`); toast.success('Watch party deleted'); loadData(); }
        catch (err) { toast.error('Failed to delete: ' + err.message); }
    }

    function statusBadge(status) {
        const map = { Active: 'badge-success', Ended: 'badge-muted', Scheduled: 'badge-info', Cancelled: 'badge-danger' };
        return `<span class="badge ${map[status] || 'badge-muted'}">${utils.esc(status || 'Unknown')}</span>`;
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Watch Parties', 'Monitor and manage group viewing sessions');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Parties</label>
                    <input class="input-field" id="wp-search" value="${utils.esc(state.search)}" placeholder="Search by host or content...">
                </div>
                <div class="form-group mb-0">
                    <label class="form-label">Status</label>
                    <select class="input-field" id="wp-status">
                        <option value="">All</option>
                        ${['Active','Ended','Scheduled','Cancelled'].map(s => `<option value="${s}" ${state.statusFilter === s ? 'selected' : ''}>${s}</option>`).join('')}
                    </select>
                </div>
                <button class="btn btn-primary btn-sm" id="wp-search-btn">Apply</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-users', 'No watch parties found'); }
        else {
            const rows = state.data.items.map(p => `<tr>
                <td class="font-medium">${utils.esc(p.hostEmail || '—')}</td>
                <td>${utils.esc(p.contentTitle || '—')}</td>
                <td>${statusBadge(p.status)}</td>
                <td class="text-center font-mono">${p.participantCount ?? 0} / ${p.maxParticipants ?? '∞'}</td>
                <td>${p.isPrivate ? '<span class="badge badge-warning">Private</span>' : '<span class="badge badge-info">Public</span>'}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(p.createdAt)}</td>
                <td class="text-right">
                    <div class="flex items-center justify-end gap-1.5">
                        <button class="btn btn-ghost btn-sm" onclick="router.navigate('/admin/watch-party-participants?partyId=${p.id}')" title="View Participants"><i class="fas fa-users text-xs"></i></button>
                        ${p.status === 'Active' ? `<button class="btn btn-ghost btn-sm text-vs-warning end-wp-btn" data-id="${p.id}" title="End Party"><i class="fas fa-stop text-xs"></i></button>` : ''}
                        <button class="btn btn-ghost btn-sm text-vs-danger del-wp-btn" data-id="${p.id}" title="Delete"><i class="fas fa-trash text-xs"></i></button>
                    </div>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'hostEmail', label: 'Host' }, { key: 'contentTitle', label: 'Content' }, { key: 'status', label: 'Status' }, { key: 'participantCount', label: 'Participants' }, { key: 'isPrivate', label: 'Visibility' }, { key: 'createdAt', label: 'Created' }, { key: '', label: '' }],
                rows, 'No watch parties',
                { tableId: 'wp-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('wp-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('wp-search').value.trim();
            state.statusFilter = document.getElementById('wp-status').value;
            state.page = 1; loadData();
        });
        document.querySelectorAll('.end-wp-btn').forEach(b => b.addEventListener('click', () => endParty(parseInt(b.dataset.id))));
        document.querySelectorAll('.del-wp-btn').forEach(b => b.addEventListener('click', () => deleteParty(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
