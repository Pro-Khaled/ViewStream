pages.adminAudit = (() => {
    let state = { page: 1, filters: {}, data: null, loading: true };
    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/audit/logs', { ...state.filters, page: state.page, pageSize: CONFIG.PAGE_SIZE });
        } catch (err) { toast.error('Failed to load audit logs'); }
        state.loading = false; render();
    }
    function render() {
        const c = document.getElementById('admin-audit-content');
        if (!c) return;
        let h = Comp.pageHeader('Audit Logs');
        h += Comp.filterBar([
            { key: 'tableName', label: 'Table Name' },
            { key: 'recordId', label: 'Record ID', type: 'number' },
            { key: 'action', label: 'Action' }
        ], vals => { state.filters = vals; state.page = 1; loadData(); });
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-clipboard-list', 'No audit logs'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Table</th><th>Record</th><th>Action</th><th>Changed By</th><th>Timestamp</th></tr></thead><tbody>` +
                state.data.items.map(l => `<tr>
                    <td class="text-muted">${l.id}</td>
                    <td class="font-mono text-sm">${toast.esc(l.tableName)}</td>
                    <td class="text-muted">${l.recordId}</td>
                    <td>${utils.statusBadge(l.action.toLowerCase())}</td>
                    <td>${toast.esc(l.changedByUserName)}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(l.changedAt)}</td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
    }
    return {
        render() { return '<div id="admin-audit-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();