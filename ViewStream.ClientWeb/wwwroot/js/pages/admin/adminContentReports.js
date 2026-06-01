pages.adminContentReports = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'id', sortDir: 'desc', status: '', targetType: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/contentreports', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                status: state.status || undefined,
                targetType: state.targetType || undefined
            });
        } catch (err) { toast.error('Failed to load content reports: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Content Reports Moderation', 'Moderate user reported shows and episodes');
        
        h += `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                <div class="form-group mb-0 w-full col-span-2">
                    <label class="form-label">Search Query</label>
                    <input class="input-field" id="creport-search" value="${toast.esc(state.search)}" placeholder="Search by title, reporter or reason...">
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Status Filter</label>
                    <select class="input-field" id="creport-status-filter">
                        <option value="">All Statuses</option>
                        <option value="pending" ${state.status === 'pending' ? 'selected' : ''}>Pending</option>
                        <option value="reviewed" ${state.status === 'reviewed' ? 'selected' : ''}>Reviewed</option>
                        <option value="dismissed" ${state.status === 'dismissed' ? 'selected' : ''}>Dismissed</option>
                        <option value="action_taken" ${state.status === 'action_taken' ? 'selected' : ''}>Action Taken</option>
                    </select>
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Target Type</label>
                    <select class="input-field" id="creport-type-filter">
                        <option value="">All Targets</option>
                        <option value="Show" ${state.targetType === 'Show' ? 'selected' : ''}>Show</option>
                        <option value="Episode" ${state.targetType === 'Episode' ? 'selected' : ''}>Episode</option>
                    </select>
                </div>
                <div class="col-span-1 md:col-span-4 text-right">
                    <button class="btn btn-primary btn-sm w-full md:w-auto" id="creport-search-btn">Filter Reports</button>
                </div>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-flag', 'No content reports found'); }
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

                const targetTypeBadge = r.targetType?.toLowerCase() === 'show'
                    ? `<span class="badge bg-vs-accent/10 text-vs-accent"><i class="fas fa-film mr-1 text-[10px]"></i> Show</span>`
                    : `<span class="badge bg-vs-info/10 text-vs-info"><i class="fas fa-play-circle mr-1 text-[10px]"></i> Episode</span>`;

                return `<tr>
                    <td class="text-muted">#${r.id}</td>
                    <td>${targetTypeBadge}</td>
                    <td class="font-bold text-vs-text max-w-[200px] truncate">${toast.esc(r.targetTitle || '—')} <span class="text-xs text-vs-muted">(#${r.showId || r.episodeId})</span></td>
                    <td>${toast.esc(r.reportedByProfileName || '—')}</td>
                    <td><span class="text-sm font-semibold text-vs-accent">${toast.esc(r.reason)}</span></td>
                    <td>${statusBadge}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(r.createdAt || r.reportedAt)}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm view-creport-btn" data-id="${r.id}" title="View Details"><i class="fas fa-eye text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm text-vs-accent update-creport-btn" data-id="${r.id}" data-status="${toast.esc(r.status)}" title="Moderate Status"><i class="fas fa-gavel text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'targetType', label: 'Target Type' },
                    { key: 'targetTitle', label: 'Content Title' },
                    { key: 'reportedByProfileName', label: 'Reporter' },
                    { key: 'reason', label: 'Reason' },
                    { key: 'status', label: 'Status' },
                    { key: 'createdAt', label: 'Reported At' },
                    { key: '', label: '' }
                ],
                rows,
                'No content reports found',
                {
                    tableId: 'content-reports-table',
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
        document.getElementById('creport-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('creport-search').value.trim();
            state.status = document.getElementById('creport-status-filter').value;
            state.targetType = document.getElementById('creport-type-filter').value;
            state.page = 1; loadData();
        });

        document.querySelectorAll('.view-creport-btn').forEach(btn => {
            btn.addEventListener('click', () => showDetailsModal(parseInt(btn.dataset.id)));
        });

        document.querySelectorAll('.update-creport-btn').forEach(btn => {
            btn.addEventListener('click', () => showUpdateModal(parseInt(btn.dataset.id), btn.dataset.status));
        });
    }

    async function showDetailsModal(id) {
        modal.open('Content Report Details', Comp.pageLoader());
        try {
            const r = await api.get(`/admin/contentreports/${id}`);
            modal.open(`Content Report #${r.id}`,
                `<div class="space-y-4 max-h-[70vh] overflow-y-auto px-1 font-body">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('ID', r.id)}
                        ${Comp.detailRow('Target Type', r.targetType)}
                        ${Comp.detailRow('Target Title', r.targetTitle || '—')}
                        ${Comp.detailRow('Reporter Name', r.reportedByProfileName || '—')}
                        ${Comp.detailRow('Reason', r.reason)}
                        ${Comp.detailRow('Status', r.status)}
                        ${Comp.detailRow('Reported At', utils.formatDate(r.createdAt || r.reportedAt))}
                        ${r.showId ? Comp.detailRow('Show ID', r.showId) : ''}
                        ${r.episodeId ? Comp.detailRow('Episode ID', r.episodeId) : ''}
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
        } catch (err) { toast.error('Failed to load content report: ' + err.message); modal.close(); }
    }

    function showUpdateModal(id, currentStatus) {
        modal.open('Moderate Content Report',
            `<div class="space-y-4 font-body">
                <p class="text-sm text-vs-dim">Select the new moderation status for content report <strong class="text-vs-text">#${id}</strong>.</p>
                <div class="form-group">
                    <label class="form-label">Moderation Status <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="update-creport-status">
                        <option value="pending" ${currentStatus === 'pending' ? 'selected' : ''}>Pending</option>
                        <option value="reviewed" ${currentStatus === 'reviewed' ? 'selected' : ''}>Reviewed (Approved)</option>
                        <option value="dismissed" ${currentStatus === 'dismissed' ? 'selected' : ''}>Dismissed</option>
                        <option value="action_taken" ${currentStatus === 'action_taken' ? 'selected' : ''}>Action Taken (Hidden/Removed)</option>
                    </select>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-creport-status"><i class="fas fa-save mr-1"></i> Update Status</button>`
        );

        document.getElementById('save-creport-status')?.addEventListener('click', async () => {
            const status = document.getElementById('update-creport-status').value;
            try {
                await api.put(`/admin/contentreports/${id}/status`, { status });
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
