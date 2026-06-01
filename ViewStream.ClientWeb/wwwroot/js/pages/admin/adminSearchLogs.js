pages.adminSearchLogs = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'searchedAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/search-logs', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load search logs: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Search Logs', 'Analyze user search queries and trends');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Filter Logs</label>
                    <input class="input-field" id="sl-search" value="${utils.esc(state.search)}" placeholder="Filter by query or user...">
                </div>
                <button class="btn btn-primary btn-sm" id="sl-search-btn">Filter</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-search', 'No search logs found'); }
        else {
            const rows = state.data.items.map(l => `<tr>
                <td class="font-medium text-vs-accent">${utils.esc(l.query || '—')}</td>
                <td class="text-muted text-sm">${utils.esc(l.userEmail || 'Anonymous')}</td>
                <td class="text-center font-mono">${l.resultsCount ?? '—'}</td>
                <td class="text-muted text-sm">${utils.esc(l.filterApplied || 'None')}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(l.searchedAt)}</td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'query', label: 'Search Query' }, { key: 'userEmail', label: 'User' }, { key: 'resultsCount', label: 'Results' }, { key: 'filterApplied', label: 'Filters' }, { key: 'searchedAt', label: 'Time' }],
                rows, 'No search logs',
                { tableId: 'sl-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('sl-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('sl-search').value.trim(); state.page = 1; loadData(); });
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
