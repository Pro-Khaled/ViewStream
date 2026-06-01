pages.adminPersonalizedRows = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'displayOrder', sortDir: 'asc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/personalized-rows', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load personalized rows: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Personalized Rows', 'View homepage content rows and display order (read-only)');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Rows</label>
                    <input class="input-field" id="row-search" value="${utils.esc(state.search)}" placeholder="Search by title or type...">
                </div>
                <button class="btn btn-primary btn-sm" id="row-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-th-list', 'No personalized rows found'); }
        else {
            const rows = state.data.items.map(r => `<tr>
                <td class="font-medium">${utils.esc(r.title || '—')}</td>
                <td><span class="badge badge-info">${utils.esc(r.rowType || 'Custom')}</span></td>
                <td class="text-center font-mono">${r.displayOrder ?? '—'}</td>
                <td class="text-muted text-sm">${r.genreId ? `Genre #${r.genreId}` : '—'}</td>
                <td>${r.isActive ? '<span class="badge badge-success">Active</span>' : '<span class="badge badge-muted">Hidden</span>'}</td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'title', label: 'Title' }, { key: 'rowType', label: 'Type' }, { key: 'displayOrder', label: 'Order' }, { key: 'genreId', label: 'Genre' }, { key: 'isActive', label: 'Status' }],
                rows, 'No rows configured',
                { tableId: 'rows-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('row-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('row-search').value.trim(); state.page = 1; loadData(); });
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
