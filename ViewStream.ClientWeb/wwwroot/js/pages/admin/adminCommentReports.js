pages.adminCommentReports = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'id', sortDir: 'desc', status: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/commentreports', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                status: state.status || undefined
            });
        } catch (err) { toast.error('Failed to load comment reports: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Comment Reports Moderation', 'Moderate user reported comments');
        
        h += `<div class="card p-4 mb-6 font-body">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Comment or Reporter</label>
                    <input class="input-field" id="report-search" value="${toast.esc(state.search)}" placeholder="Search by comment text, reporter or reason...">
                </div>
                <div class="form-group mb-0 w-full md:w-48">
                    <label class="form-label">Status Filter</label>
                    <select class="input-field" id="report-status-filter">
                        <option value="">All Statuses</option>
                        <option value="pending" ${state.status === 'pending' ? 'selected' : ''}>Pending</option>
                        <option value="reviewed" ${state.status === 'reviewed' ? 'selected' : ''}>Reviewed</option>
                        <option value="dismissed" ${state.status === 'dismissed' ? 'selected' : ''}>Dismissed</option>
                        <option value="action_taken" ${state.status === 'action_taken' ? 'selected' : ''}>Action Taken</option>
                    </select>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="report-search-btn">Filter</button>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-flag', 'No comment reports found'); }
        else {
            const rows = state.data.items.map(r => {
                let statusBadge = `<span class="badge bg-vs-warning/10 text-vs-warning font-semibold uppercase">${toast.esc(r.status)}</span>`;
                if (r.status?.toLowerCase() === 'dismissed') {
                    statusBadge = `<span class="badge bg-vs-muted/20 text-vs-muted font-semibold uppercase">${toast.esc(r.status)}</span>`;
                } else if (r.status?.toLowerCase() === 'action_taken' || r.status?.toLowerCase() === 'actiontaken') {
                    statusBadge = `<span class="badge bg-vs-error/10 text-vs-error font-semibold uppercase">Action Taken</span>`;
                } else if (r.status?.toLowerCase() === 'reviewed') {
                    statusBadge = `<span class="badge bg-vs-success/10 text-vs-success font-semibold uppercase">${toast.esc(r.status)}</span>`;
                }

                return `<tr>
                    <td class="text-muted">#${r.id}</td>
                    <td class="font-medium text-vs-text max-w-[250px] truncate" title="${toast.esc(r.commentText || '')}">${toast.esc(r.commentText || '—')}</td>
                    <td>${toast.esc(r.reportedByProfileName || '—')}</td>
                    <td><span class="text-sm font-semibold text-vs-accent">${toast.esc(r.reason)}</span></td>
                    <td>${statusBadge}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(r.createdAt)}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm view-report-btn" data-id="${r.id}" title="View Details"><i class="fas fa-eye text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm text-vs-accent update-report-btn" data-id="${r.id}" data-status="${toast.esc(r.status)}" title="Moderate Status"><i class="fas fa-gavel text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'commentText', label: 'Comment Text' },
                    { key: 'reportedByProfileName', label: 'Reporter' },
                    { key: 'reason', label: 'Reason' },
                    { key: 'status', label: 'Status' },
                    { key: 'createdAt', label: 'Reported At' },
                    { key: '', label: '' }
                ],
                rows,
                'No comment reports found',
                {
                    tableId: 'comment-reports-table',
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
        document.getElementById('report-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('report-search').value.trim();
            state.status = document.getElementById('report-status-filter').value;
            state.page = 1; loadData();
        });

        document.querySelectorAll('.view-report-btn').forEach(btn => {
            btn.addEventListener('click', () => showDetailsModal(parseInt(btn.dataset.id)));
        });

        document.querySelectorAll('.update-report-btn').forEach(btn => {
            btn.addEventListener('click', () => showUpdateModal(parseInt(btn.dataset.id), btn.dataset.status));
        });
    }

    async function showDetailsModal(id) {
        modal.open('Report Details', Comp.pageLoader());
        try {
            const r = await api.get(`/admin/commentreports/${id}`);
            modal.open(`Comment Report #${r.id}`,
                `<div class="space-y-4 max-h-[70vh] overflow-y-auto px-1 font-body">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('ID', r.id)}
                        ${Comp.detailRow('Comment ID', r.commentId)}
                        ${Comp.detailRow('Reporter Name', r.reportedByProfileName || '—')}
                        ${Comp.detailRow('Reason', r.reason)}
                        ${Comp.detailRow('Status', r.status)}
                        ${Comp.detailRow('Created At', utils.formatDate(r.createdAt))}
                    </div>
                    <div class="border-t border-vs-border pt-4">
                        <label class="form-label font-bold mb-1 block">Reported Comment Text</label>
                        <div class="bg-vs-card p-3 border border-vs-border rounded-lg text-vs-text text-sm italic">
                            "${toast.esc(r.commentText || '')}"
                        </div>
                    </div>
                    ${r.details ? `<div class="border-t border-vs-border pt-4">
                        <label class="form-label font-bold mb-1 block">Reporter Details</label>
                        <div class="text-sm text-vs-dim">${toast.esc(r.details)}</div>
                    </div>` : ''}
                    ${r.reviewedByUserName ? `<div class="border-t border-vs-border pt-4 grid grid-cols-2 gap-4">
                        ${Comp.detailRow('Reviewed By', r.reviewedByUserName)}
                        ${Comp.detailRow('Reviewed At', utils.formatDate(r.reviewedAt))}
                    </div>` : ''}
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Close</button>`
            );
        } catch (err) { toast.error('Failed to load report: ' + err.message); modal.close(); }
    }

    function showUpdateModal(id, currentStatus) {
        modal.open('Moderate Comment Report',
            `<div class="space-y-4 font-body">
                <p class="text-sm text-vs-dim">Select the new moderation status for comment report <strong class="text-vs-text">#${id}</strong>.</p>
                <div class="form-group">
                    <label class="form-label">Moderation Status <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="update-report-status">
                        <option value="pending" ${currentStatus === 'pending' ? 'selected' : ''}>Pending</option>
                        <option value="reviewed" ${currentStatus === 'reviewed' ? 'selected' : ''}>Reviewed (Approved)</option>
                        <option value="dismissed" ${currentStatus === 'dismissed' ? 'selected' : ''}>Dismissed</option>
                        <option value="action_taken" ${currentStatus === 'action_taken' ? 'selected' : ''}>Action Taken (Hidden/Removed)</option>
                    </select>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-report-status"><i class="fas fa-save mr-1"></i> Update Status</button>`
        );

        document.getElementById('save-report-status')?.addEventListener('click', async () => {
            const status = document.getElementById('update-report-status').value;
            try {
                await api.put(`/admin/commentreports/${id}/status`, { status });
                toast.success('Report status updated successfully');
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
