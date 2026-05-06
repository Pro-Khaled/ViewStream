pages.adminLogs = (() => {
    let activeTab = 'audit'; // 'audit', 'error', 'search'
    let auditState = { page: 1, filters: {}, data: null, loading: true };
    let errorState = { page: 1, filters: {}, data: null, loading: true };
    let searchState = { page: 1, filters: {}, data: null, loading: true };

    async function loadAudit() {
        auditState.loading = true; render();
        try { auditState.data = await api.get('/admin/audit/logs', { ...auditState.filters, page: auditState.page, pageSize: CONFIG.PAGE_SIZE }); }
        catch { toast.error('Failed to load audit logs'); }
        auditState.loading = false; render();
    }
    async function loadError() {
        errorState.loading = true; render();
        try { errorState.data = await api.get('/admin/errors/logs', { ...errorState.filters, page: errorState.page, pageSize: CONFIG.PAGE_SIZE }); }
        catch { toast.error('Failed to load error logs'); }
        errorState.loading = false; render();
    }
    async function loadSearch() {
        searchState.loading = true; render();
        try { searchState.data = await api.get('/admin/search/logs', { ...searchState.filters, page: searchState.page, pageSize: CONFIG.PAGE_SIZE }); }
        catch { toast.error('Failed to load search logs'); }
        searchState.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-logs-content');
        if (!c) return;
        let h = `<div class="flex gap-1 mb-6 bg-vs-card rounded-lg p-1 w-fit">
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'audit' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="audit">Audit Logs</button>
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'error' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="error">Error Logs</button>
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'search' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="search">Search Logs</button>
        </div>`;

        if (activeTab === 'audit') h += renderAudit();
        else if (activeTab === 'error') h += renderError();
        else h += renderSearch();

        c.innerHTML = h;
        bindEvents();
    }

    function renderAudit() {
        let h = Comp.pageHeader('Audit Logs', 'Track all data changes');
        h += Comp.filterBar([
            { key: 'tableName', label: 'Table Name' },
            { key: 'recordId', label: 'Record ID', type: 'number' },
            { key: 'action', label: 'Action' }
        ], vals => { auditState.filters = vals; auditState.page = 1; loadAudit(); });
        if (auditState.loading) { h += Comp.pageLoader(); }
        else if (!auditState.data?.items?.length) { h += Comp.emptyState('fa-clipboard-list', 'No audit logs'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Table</th><th>Record</th><th>Action</th><th>Changed By</th><th>Timestamp</th></tr></thead><tbody>` +
                auditState.data.items.map(l => `<tr>
                    <td class="text-muted">${l.id}</td>
                    <td class="font-mono text-sm">${toast.esc(l.tableName)}</td>
                    <td class="text-muted">${l.recordId}</td>
                    <td>${utils.statusBadge(l.action.toLowerCase())}</td>
                    <td>${toast.esc(l.changedByUserName)}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(l.changedAt)}</td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(auditState.data.pageNumber, auditState.data.totalPages, p => { auditState.page = p; loadAudit(); });
        }
        return h;
    }

    function renderError() {
        let h = Comp.pageHeader('Error Logs', 'Monitor application errors');
        h += Comp.filterBar([
            { key: 'errorCode', label: 'Error Code' },
            { key: 'endpoint', label: 'Endpoint' }
        ], vals => { errorState.filters = vals; errorState.page = 1; loadError(); });
        if (errorState.loading) { h += Comp.pageLoader(); }
        else if (!errorState.data?.items?.length) { h += Comp.emptyState('fa-bug', 'No errors'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Code</th><th>Message</th><th>Endpoint</th><th>Occurred</th></tr></thead><tbody>` +
                errorState.data.items.map(e => `<tr>
                    <td class="text-muted">${e.id}</td>
                    <td><span class="badge badge-danger">${toast.esc(e.errorCode)}</span></td>
                    <td class="max-w-[300px]">${toast.esc(e.errorMessage)}</td>
                    <td class="font-mono text-sm text-muted">${toast.esc(utils.truncate(e.endpoint, 35))}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(e.occurredAt)}</td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(errorState.data.pageNumber, errorState.data.totalPages, p => { errorState.page = p; loadError(); });
        }
        return h;
    }

    function renderSearch() {
        let h = Comp.pageHeader('Search Logs', 'Analyze user search behavior');
        h += Comp.filterBar([
            { key: 'profileId', label: 'Profile ID', type: 'number' },
            { key: 'query', label: 'Search Term' }
        ], vals => { searchState.filters = vals; searchState.page = 1; loadSearch(); });
        if (searchState.loading) { h += Comp.pageLoader(); }
        else if (!searchState.data?.items?.length) { h += Comp.emptyState('fa-search', 'No search logs'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Profile</th><th>Query</th><th>Results</th><th>Clicked Show</th><th>Date</th></tr></thead><tbody>` +
                searchState.data.items.map(l => `<tr>
                    <td class="text-muted">${l.id}</td>
                    <td>${toast.esc(l.profileName || '—')}</td>
                    <td class="font-medium">${toast.esc(l.query)}</td>
                    <td class="text-muted">${l.resultsCount ?? '—'}</td>
                    <td>${toast.esc(l.clickedShowTitle || '—')}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(l.searchAt)}</td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(searchState.data.pageNumber, searchState.data.totalPages, p => { searchState.page = p; loadSearch(); });
        }
        return h;
    }

    function bindEvents() {
        document.querySelectorAll('#admin-logs-content .tab-btn').forEach(b => {
            b.addEventListener('click', () => {
                activeTab = b.dataset.tab;
                if (activeTab === 'audit' && !auditState.data) loadAudit();
                else if (activeTab === 'error' && !errorState.data) loadError();
                else if (activeTab === 'search' && !searchState.data) loadSearch();
                render();
            });
        });
    }

    return {
        render() { return '<div id="admin-logs-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadAudit(); }
    };
})();