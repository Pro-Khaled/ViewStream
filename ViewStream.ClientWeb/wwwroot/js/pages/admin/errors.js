pages.adminErrors = (() => {
    let state = { page: 1, filters: {}, data: null, loading: true };
    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/errors/logs', { ...state.filters, page: state.page, pageSize: CONFIG.PAGE_SIZE });
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
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Code</th><th>Message</th><th>Endpoint</th><th>Occurred</th></tr></thead><tbody>` +
                state.data.items.map(e => `<tr>
                    <td class="text-muted">${e.id}</td>
                    <td><span class="badge badge-danger">${toast.esc(e.errorCode)}</span></td>
                    <td class="max-w-[300px]">${toast.esc(e.errorMessage)}</td>
                    <td class="font-mono text-sm text-muted">${toast.esc(utils.truncate(e.endpoint, 35))}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(e.occurredAt)}</td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
    }
    return {
        render() { return '<div id="admin-errors-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();