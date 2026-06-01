pages.adminAuditLogs = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true, sortKey: 'id', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/audit/logs', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load audit logs: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Audit Logs', 'Track database modifications', 
            '<button class="btn btn-danger btn-sm" id="purge-audit-btn"><i class="fas fa-trash-restore mr-1.5"></i> Purge Old Logs</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Table or User</label>
                    <input class="input-field" id="audit-search" value="${toast.esc(state.search)}" placeholder="Search by table name, action, or user...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="audit-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="audit-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="audit-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-clipboard-list', 'No audit logs found'); }
        else {
            const rows = state.data.items.map(l => {
                let actionBadge = `<span class="badge bg-vs-info/10 text-vs-info">${toast.esc(l.action)}</span>`;
                if (l.action.toUpperCase() === 'DELETE') actionBadge = `<span class="badge bg-vs-error/10 text-vs-error">${toast.esc(l.action)}</span>`;
                else if (l.action.toUpperCase() === 'INSERT' || l.action.toUpperCase() === 'CREATE') actionBadge = `<span class="badge bg-vs-success/10 text-vs-success">${toast.esc(l.action)}</span>`;
                
                return `<tr class="${l.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">${l.id}</td>
                    <td class="font-bold text-vs-text font-mono text-sm">${toast.esc(l.tableName)}</td>
                    <td>${l.recordId}</td>
                    <td>${actionBadge}</td>
                    <td>${toast.esc(l.changedByUserName || 'System')}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(l.changedAt)}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm view-audit-btn" data-id="${l.id}" title="View Details"><i class="fas fa-eye text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm text-vs-error delete-audit-btn" data-id="${l.id}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'tableName', label: 'Table' },
                    { key: 'recordId', label: 'Record ID' },
                    { key: 'action', label: 'Action' },
                    { key: 'changedByUserName', label: 'Changed By' },
                    { key: 'changedAt', label: 'Timestamp' },
                    { key: '', label: '' }
                ],
                rows,
                'No audit logs found',
                {
                    tableId: 'audit-table',
                    sortKey: state.sortKey,
                    sortDir: state.sortDir,
                    onSort: (key, dir) => { if (key) { state.sortKey = key; state.sortDir = dir; loadData(); } }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('audit-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('audit-search').value.trim();
            state.includeDeleted = document.getElementById('audit-include-deleted').checked;
            state.page = 1; loadData();
        });
        document.getElementById('audit-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });
        document.getElementById('purge-audit-btn')?.addEventListener('click', showPurgeModal);
        
        document.querySelectorAll('.view-audit-btn').forEach(btn => {
            btn.addEventListener('click', () => showDetailsModal(parseInt(btn.dataset.id)));
        });
        document.querySelectorAll('.delete-audit-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id)));
        });
    }

    async function showDetailsModal(id) {
        modal.open('Audit Log Details', Comp.pageLoader());
        try {
            const l = await api.get(`/admin/audit/logs/${id}`);
            let oldHtml = '<span class="text-vs-muted">None</span>';
            let newHtml = '<span class="text-vs-muted">None</span>';
            
            try { if (l.oldValues) oldHtml = `<pre class="bg-vs-card border border-vs-border p-3 rounded-lg overflow-x-auto text-xs text-vs-dim max-h-48">${toast.esc(JSON.stringify(JSON.parse(l.oldValues), null, 2))}</pre>`; } catch { if (l.oldValues) oldHtml = `<pre class="bg-vs-card border border-vs-border p-3 rounded-lg overflow-x-auto text-xs text-vs-dim max-h-48">${toast.esc(l.oldValues)}</pre>`; }
            try { if (l.newValues) newHtml = `<pre class="bg-vs-card border border-vs-border p-3 rounded-lg overflow-x-auto text-xs text-vs-dim max-h-48">${toast.esc(JSON.stringify(JSON.parse(l.newValues), null, 2))}</pre>`; } catch { if (l.newValues) newHtml = `<pre class="bg-vs-card border border-vs-border p-3 rounded-lg overflow-x-auto text-xs text-vs-dim max-h-48">${toast.esc(l.newValues)}</pre>`; }

            modal.open(`Audit Log #${l.id}`,
                `<div class="space-y-4 max-h-[70vh] overflow-y-auto px-1">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('Table Name', l.tableName)}
                        ${Comp.detailRow('Record ID', l.recordId)}
                        ${Comp.detailRow('Action', l.action)}
                        ${Comp.detailRow('Changed By', l.changedByUserName || 'System')}
                        ${Comp.detailRow('Timestamp', utils.formatDate(l.changedAt))}
                        ${Comp.detailRow('IP Address', l.ipAddress || '—')}
                    </div>
                    <div class="col-span-2">${Comp.detailRow('User Agent', l.userAgent || '—')}</div>
                    ${l.notes ? `<div class="col-span-2">${Comp.detailRow('Notes', l.notes)}</div>` : ''}
                    <div>
                        <label class="form-label mb-1.5 font-bold">Old Values</label>
                        ${oldHtml}
                    </div>
                    <div>
                        <label class="form-label mb-1.5 font-bold">New Values</label>
                        ${newHtml}
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Close</button>`
            );
        } catch (err) { toast.error('Failed to load log details: ' + err.message); modal.close(); }
    }

    function showDeleteModal(id) {
        modal.open('Delete Audit Log',
            `<p class="text-sm text-vs-dim">Are you sure you want to permanently delete audit log <strong class="text-vs-text">#${id}</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-audit">Delete</button>`
        );
        document.getElementById('confirm-delete-audit')?.addEventListener('click', async () => {
            try {
                await api.del(`/admin/audit/logs/${id}`);
                toast.success('Audit log deleted successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showPurgeModal() {
        modal.open('Purge Audit Logs',
            `<div class="space-y-4 font-body">
                <p class="text-sm text-vs-dim">Enter the number of days. Logs older than this number of days will be permanently deleted.</p>
                <div class="form-group">
                    <label class="form-label">Days Old <span class="text-vs-error">*</span></label>
                    <input type="number" class="input-field" id="purge-days" value="30" min="1">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-purge-audit"><i class="fas fa-trash-alt mr-1"></i> Purge Logs</button>`
        );
        document.getElementById('confirm-purge-audit')?.addEventListener('click', async () => {
            const days = parseInt(document.getElementById('purge-days').value);
            if (!days || days < 1) { toast.warning('Please enter a valid number of days'); return; }
            try {
                await api.delete(`/admin/audit/logs/purge?daysToKeep=${days}`);
                toast.success(`Successfully purged logs older than ${days} days`);
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
