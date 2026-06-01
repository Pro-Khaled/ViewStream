pages.adminLogs = (() => {
    let activeTab = 'audit'; // 'audit', 'error', 'search'
    let auditState = { page: 1, search: '', tableName: '', action: '', data: null, loading: true };
    let errorState = { page: 1, search: '', errorCode: '', data: null, loading: true };
    let searchState = { page: 1, search: '', data: null, loading: true };

    // Sorting states
    let auditSortKey = 'changedAt', auditSortDir = 'desc';
    let errorSortKey = 'occurredAt', errorSortDir = 'desc';
    let searchSortKey = 'searchAt', searchSortDir = 'desc';

    async function loadAudit() {
        auditState.loading = true; render();
        try {
            auditState.data = await api.get('/admin/audit/logs', {
                pageNumber: auditState.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: auditState.search || undefined,
                tableName: auditState.tableName || undefined,
                action: auditState.action || undefined,
                sortBy: auditSortKey,
                sortDescending: auditSortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load audit logs: ' + err.message); }
        auditState.loading = false; render();
    }

    async function loadError() {
        errorState.loading = true; render();
        try {
            errorState.data = await api.get('/admin/errors/logs', {
                pageNumber: errorState.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: errorState.search || undefined,
                errorCode: errorState.errorCode || undefined,
                sortBy: errorSortKey,
                sortDescending: errorSortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load error logs: ' + err.message); }
        errorState.loading = false; render();
    }

    async function loadSearch() {
        searchState.loading = true; render();
        try {
            searchState.data = await api.get('/admin/search/logs', {
                pageNumber: searchState.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: searchState.search || undefined,
                sortBy: searchSortKey,
                sortDescending: searchSortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load search logs: ' + err.message); }
        searchState.loading = false; render();
    }

    async function purgeLogs(type) {
        if (!await modal.confirm('Purge System Logs', `Are you sure you want to purge ${type} logs older than 30 days? This action cannot be undone.`, 'danger')) return;
        try {
            let endpoint = '';
            if (type === 'Audit') endpoint = '/admin/audit/logs/purge?daysToKeep=30';
            else if (type === 'Error') endpoint = '/admin/errors/logs/purge?daysToKeep=30';
            else if (type === 'Search') endpoint = '/admin/search/logs/purge?daysToKeep=30';

            const count = await api.delete(endpoint);
            toast.success(`Purge completed. Wiped ${count || 0} old records.`);
            if (type === 'Audit') loadAudit();
            else if (type === 'Error') loadError();
            else if (type === 'Search') loadSearch();
        } catch (err) {
            toast.error('Failed to purge logs: ' + err.message);
        }
    }

    function render() {
        const c = document.getElementById('admin-logs-content');
        if (!c) return;

        let h = Comp.pageHeader('System Logging & Telemetry', 'Monitor database audits, application runtime exceptions, and search terms metrics.');

        h += `<div class="flex gap-1.5 mb-6 bg-vs-surface-2 border border-vs-border rounded-xl p-1.5 w-fit font-body">
            <button class="tab-btn btn btn-sm px-4 py-2 rounded-lg font-bold text-xs ${activeTab === 'audit' ? 'bg-vs-accent text-vs-bg shadow' : 'bg-transparent text-vs-dim hover:bg-vs-surface transition-colors'}" data-tab="audit">
                <i class="fas fa-clipboard-list mr-1.5 text-xs"></i> Audit Logs
            </button>
            <button class="tab-btn btn btn-sm px-4 py-2 rounded-lg font-bold text-xs ${activeTab === 'error' ? 'bg-vs-accent text-vs-bg shadow' : 'bg-transparent text-vs-dim hover:bg-vs-surface transition-colors'}" data-tab="error">
                <i class="fas fa-bug mr-1.5 text-xs"></i> Error Logs
            </button>
            <button class="tab-btn btn btn-sm px-4 py-2 rounded-lg font-bold text-xs ${activeTab === 'search' ? 'bg-vs-accent text-vs-bg shadow' : 'bg-transparent text-vs-dim hover:bg-vs-surface transition-colors'}" data-tab="search">
                <i class="fas fa-search mr-1.5 text-xs"></i> Search Logs
            </button>
        </div>`;

        if (activeTab === 'audit') h += renderAudit();
        else if (activeTab === 'error') h += renderError();
        else h += renderSearch();

        c.innerHTML = h;
        bindEvents();
    }

    function renderAudit() {
        let h = `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                <div class="form-group mb-0">
                    <label class="form-label text-xs">Table Name</label>
                    <input class="input-field" id="audit-table-filter" value="${toast.esc(auditState.tableName)}" placeholder="e.g. Shows">
                </div>
                <div class="form-group mb-0">
                    <label class="form-label text-xs">Action Type</label>
                    <select class="input-field" id="audit-action-filter">
                        <option value="">All Actions</option>
                        <option value="Create" ${auditState.action === 'Create' ? 'selected' : ''}>Create</option>
                        <option value="Update" ${auditState.action === 'Update' ? 'selected' : ''}>Update</option>
                        <option value="Delete" ${auditState.action === 'Delete' ? 'selected' : ''}>Delete</option>
                    </select>
                </div>
                <div class="form-group mb-0">
                    <label class="form-label text-xs">Search Keywords</label>
                    <input class="input-field" id="audit-search" value="${toast.esc(auditState.search)}" placeholder="User or contents...">
                </div>
                <div class="flex items-center gap-2">
                    <button class="btn btn-primary btn-sm flex-1" id="audit-apply-btn"><i class="fas fa-filter mr-1.5 text-xs"></i>Filter</button>
                    <button class="btn btn-secondary text-vs-danger border-vs-danger/20 hover:bg-vs-danger/10 btn-sm" id="audit-purge-btn" title="Purge >30 days"><i class="fas fa-trash-alt"></i></button>
                </div>
            </div>
        </div>`;

        if (auditState.loading) { h += Comp.pageLoader(); }
        else if (!auditState.data?.items?.length) { h += Comp.emptyState('fa-clipboard-list', 'No audit logs found matching criteria'); }
        else {
            const rows = auditState.data.items.map(l => `<tr>
                <td class="text-muted font-mono text-xs">#${l.id}</td>
                <td class="font-mono text-vs-accent font-semibold text-xs">${toast.esc(l.tableName)}</td>
                <td class="text-muted font-mono text-xs">${l.recordId}</td>
                <td>${utils.statusBadge(l.action)}</td>
                <td><span class="font-semibold">${toast.esc(l.changedByUserName || 'System')}</span></td>
                <td class="text-muted text-xs font-mono">${utils.formatDate(l.changedAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm view-audit-detail" data-id="${l.id}" title="Compare Values"><i class="fas fa-code text-xs"></i></button>
                </td>
            </tr>`).join('');

            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'tableName', label: 'Table' }, { key: 'recordId', label: 'Record ID' }, { key: 'action', label: 'Action' }, { key: 'changedByUserName', label: 'Changed By' }, { key: 'changedAt', label: 'Timestamp' }, { key: '', label: '' }],
                rows, 'No logs',
                { tableId: 'audit-tbl', sortKey: auditSortKey, sortDir: auditSortDir, onSort: (k, d) => { auditSortKey = k; auditSortDir = d; loadAudit(); } }
            );
            h += Comp.pagination(auditState.data.pageNumber, auditState.data.totalPages, p => { auditState.page = p; loadAudit(); });
        }
        return h;
    }

    function renderError() {
        let h = `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                <div class="form-group mb-0">
                    <label class="form-label text-xs">Error Code</label>
                    <input class="input-field font-mono" id="error-code-filter" value="${toast.esc(errorState.errorCode)}" placeholder="e.g. 500">
                </div>
                <div class="col-span-2 form-group mb-0">
                    <label class="form-label text-xs">Search Stacktrace / Message</label>
                    <input class="input-field" id="error-search" value="${toast.esc(errorState.search)}" placeholder="Keyword search in error details...">
                </div>
                <div class="flex items-center gap-2">
                    <button class="btn btn-primary btn-sm flex-1" id="error-apply-btn"><i class="fas fa-filter mr-1.5 text-xs"></i>Filter</button>
                    <button class="btn btn-secondary text-vs-danger border-vs-danger/20 hover:bg-vs-danger/10 btn-sm" id="error-purge-btn" title="Purge >30 days"><i class="fas fa-trash-alt"></i></button>
                </div>
            </div>
        </div>`;

        if (errorState.loading) { h += Comp.pageLoader(); }
        else if (!errorState.data?.items?.length) { h += Comp.emptyState('fa-bug', 'No runtime exceptions recorded'); }
        else {
            const rows = errorState.data.items.map(e => `<tr>
                <td class="text-muted font-mono text-xs">#${e.id}</td>
                <td><span class="badge badge-danger text-xxs font-bold uppercase font-mono">${toast.esc(e.errorCode)}</span></td>
                <td class="max-w-xs truncate text-xs font-semibold text-vs-text" title="${toast.esc(e.errorMessage)}">${toast.esc(e.errorMessage)}</td>
                <td class="font-mono text-xxs text-muted max-w-[200px] truncate" title="${toast.esc(e.endpoint)}">${toast.esc(e.endpoint || '—')}</td>
                <td class="text-muted text-xs font-mono">${utils.formatDate(e.occurredAt)}</td>
                <td class="text-right whitespace-nowrap">
                    <button class="btn btn-ghost btn-sm view-error-detail" data-id="${e.id}" title="View Stack Trace"><i class="fas fa-align-left text-xs"></i></button>
                </td>
            </tr>`).join('');

            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'errorCode', label: 'Code' }, { key: 'errorMessage', label: 'Exception Message' }, { key: 'endpoint', label: 'Endpoint' }, { key: 'occurredAt', label: 'Occurred' }, { key: '', label: '' }],
                rows, 'No errors',
                { tableId: 'error-tbl', sortKey: errorSortKey, sortDir: errorSortDir, onSort: (k, d) => { errorSortKey = k; errorSortDir = d; loadError(); } }
            );
            h += Comp.pagination(errorState.data.pageNumber, errorState.data.totalPages, p => { errorState.page = p; loadError(); });
        }
        return h;
    }

    function renderSearch() {
        let h = `<div class="card p-4 mb-6 font-body">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label text-xs">Search Keywords</label>
                    <input class="input-field" id="search-log-search" value="${toast.esc(searchState.search)}" placeholder="Filter search queries by keyword...">
                </div>
                <div class="flex items-center gap-2 w-full md:w-auto">
                    <button class="btn btn-primary btn-sm flex-1 md:w-32" id="search-apply-btn"><i class="fas fa-filter mr-1.5 text-xs"></i>Filter</button>
                    <button class="btn btn-secondary text-vs-danger border-vs-danger/20 hover:bg-vs-danger/10 btn-sm" id="search-purge-btn" title="Purge >30 days"><i class="fas fa-trash-alt"></i></button>
                </div>
            </div>
        </div>`;

        if (searchState.loading) { h += Comp.pageLoader(); }
        else if (!searchState.data?.items?.length) { h += Comp.emptyState('fa-search', 'No queries matching this filter'); }
        else {
            const rows = searchState.data.items.map(l => `<tr>
                <td class="text-muted font-mono text-xs">#${l.id}</td>
                <td><span class="font-semibold text-vs-text">${toast.esc(l.profileName || 'Anonymous')}</span></td>
                <td class="font-mono text-vs-accent font-bold text-xs">"${toast.esc(l.query)}"</td>
                <td class="font-mono text-sm">${l.resultsCount ?? '0'}</td>
                <td class="text-xs font-semibold">${toast.esc(l.clickedShowTitle || '—')}</td>
                <td class="text-muted text-xs font-mono">${utils.formatDate(l.searchAt)}</td>
            </tr>`).join('');

            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'profileName', label: 'Profile' }, { key: 'query', label: 'Query' }, { key: 'resultsCount', label: 'Results' }, { key: 'clickedShowTitle', label: 'Clicked Show' }, { key: 'searchAt', label: 'Timestamp' }],
                rows, 'No logs',
                { tableId: 'search-tbl', sortKey: searchSortKey, sortDir: searchSortDir, onSort: (k, d) => { searchSortKey = k; searchSortDir = d; loadSearch(); } }
            );
            h += Comp.pagination(searchState.data.pageNumber, searchState.data.totalPages, p => { searchState.page = p; loadSearch(); });
        }
        return h;
    }

    async function showAuditDetail(id) {
        modal.open('Audit Log Details', Comp.pageLoader());
        try {
            const log = await api.get(`/admin/audit/logs/${id}`);
            modal.open(`Change Log #${log.id}`,
                `<div class="space-y-4 font-body">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('Entity', log.tableName)}
                        ${Comp.detailRow('Action', utils.statusBadge(log.action))}
                        ${Comp.detailRow('Changed By', log.changedByUserName || 'System')}
                        ${Comp.detailRow('Date', utils.formatDate(log.changedAt))}
                    </div>
                    <div>
                        <span class="form-label text-xs font-bold mb-1.5 block">Audit Delta Diff (JSON)</span>
                        <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
                            <div class="space-y-1">
                                <span class="text-xxs font-bold text-vs-muted">OLD STATE</span>
                                <pre class="p-3 bg-vs-surface-2 border border-vs-border rounded-xl font-mono text-xxs max-h-60 overflow-y-auto whitespace-pre-wrap text-vs-dim">${toast.esc(log.oldValues || 'Empty / Insert Operation')}</pre>
                            </div>
                            <div class="space-y-1">
                                <span class="text-xxs font-bold text-vs-accent">NEW STATE</span>
                                <pre class="p-3 bg-vs-surface-2 border border-vs-border rounded-xl font-mono text-xxs max-h-60 overflow-y-auto whitespace-pre-wrap text-vs-accent">${toast.esc(log.newValues || 'Empty / Delete Operation')}</pre>
                            </div>
                        </div>
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Close</button>`
            );
        } catch (err) { toast.error('Failed to load log detail: ' + err.message); modal.close(); }
    }

    async function showErrorDetail(id) {
        modal.open('Error Exception Details', Comp.pageLoader());
        try {
            const log = await api.get(`/admin/errors/logs/${id}`);
            modal.open(`Error Exception #${log.id}`,
                `<div class="space-y-4 font-body">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('Status Code', `<span class="badge badge-danger">${log.errorCode}</span>`)}
                        ${Comp.detailRow('Endpoint', log.endpoint || '—')}
                        ${Comp.detailRow('Occurred', utils.formatDate(log.occurredAt))}
                    </div>
                    <div>
                        <span class="form-label text-xs font-bold">Exception Message</span>
                        <p class="p-3 bg-vs-surface-2 border border-vs-border rounded-xl font-mono text-xs text-vs-error mb-3 leading-relaxed">${toast.esc(log.errorMessage)}</p>
                    </div>
                    <div>
                        <span class="form-label text-xs font-bold">Stack Trace Telemetry</span>
                        <pre class="p-3 bg-vs-surface-2 border border-vs-border rounded-xl font-mono text-xxs text-vs-muted whitespace-pre overflow-x-auto max-h-60 overflow-y-auto">${toast.esc(log.stackTrace || 'No stack trace telemetry recorded.')}</pre>
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Close</button>`
            );
        } catch (err) { toast.error('Failed to load error detail: ' + err.message); modal.close(); }
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

        // Audit Filters
        document.getElementById('audit-apply-btn')?.addEventListener('click', () => {
            auditState.tableName = document.getElementById('audit-table-filter').value.trim();
            auditState.action = document.getElementById('audit-action-filter').value;
            auditState.search = document.getElementById('audit-search').value.trim();
            auditState.page = 1; loadAudit();
        });
        document.getElementById('audit-purge-btn')?.addEventListener('click', () => purgeLogs('Audit'));
        document.querySelectorAll('.view-audit-detail').forEach(btn => {
            btn.addEventListener('click', () => showAuditDetail(parseInt(btn.dataset.id)));
        });

        // Error Filters
        document.getElementById('error-apply-btn')?.addEventListener('click', () => {
            errorState.errorCode = document.getElementById('error-code-filter').value.trim();
            errorState.search = document.getElementById('error-search').value.trim();
            errorState.page = 1; loadError();
        });
        document.getElementById('error-purge-btn')?.addEventListener('click', () => purgeLogs('Error'));
        document.querySelectorAll('.view-error-detail').forEach(btn => {
            btn.addEventListener('click', () => showErrorDetail(parseInt(btn.dataset.id)));
        });

        // Search Filters
        document.getElementById('search-apply-btn')?.addEventListener('click', () => {
            searchState.search = document.getElementById('search-log-search').value.trim();
            searchState.page = 1; loadSearch();
        });
        document.getElementById('search-purge-btn')?.addEventListener('click', () => purgeLogs('Search'));
    }

    return {
        render() { return '<div id="admin-logs-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadAudit(); }
    };
})();