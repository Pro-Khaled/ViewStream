pages.adminErrors = (() => {
    let state = { page: 1, filters: {}, data: null, loading: true };
    let sortKey = 'occurredAt';
    let sortDir = 'desc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/errors/logs', {
                ...state.filters,
                page: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                orderBy: sortKey,
                isDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load error logs'); }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-errors-content');
        if (!c) return;
        let h = Comp.pageHeader('Error Logs');
        h += Comp.filterBar([
            { key: 'errorCode', label: 'Error Code' },
            { key: 'endpoint', label: 'Endpoint' }
        ], vals => { state.filters = vals; state.page = 1; loadData(); });
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-bug', 'No errors'); }
        else {
            const rows = state.data.items.map(e => `<tr>
                <td class="text-muted">${e.id}</td>
                <td><span class="badge badge-danger">${toast.esc(e.errorCode)}</span></td>
                <td class="max-w-[300px]">${toast.esc(e.errorMessage)}</td>
                <td class="font-mono text-sm text-muted">${toast.esc(utils.truncate(e.endpoint, 35))}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(e.occurredAt)}</td>
            </tr>`).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'errorCode', label: 'Code' },
                    { key: 'errorMessage', label: 'Message' },
                    { key: 'endpoint', label: 'Endpoint' },
                    { key: 'occurredAt', label: 'Occurred' }
                ],
                rows,
                'No errors',
                {
                    tableId: 'errors-table',
                    sortKey,
                    sortDir,
                    onSort: (key, dir) => { sortKey = key; sortDir = dir; loadData(); }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
    }
    return {
        render() { return '<div id="admin-errors-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();