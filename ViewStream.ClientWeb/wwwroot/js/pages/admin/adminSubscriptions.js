pages.adminSubscriptions = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'startDate', sortDir: 'desc', statusFilter: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/subscriptions', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                status: state.statusFilter || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load subscriptions: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function cancelSubscription(id) {
        if (!await modal.confirm('Cancel Subscription', 'Cancel this user\'s subscription? They will retain access until the billing period ends.', 'warning')) return;
        try { await api.post(`/admin/subscriptions/${id}/cancel`); toast.success('Subscription cancelled'); loadData(); }
        catch (err) { toast.error('Failed to cancel: ' + err.message); }
    }

    async function showDetails(id) {
        modal.open('Subscription Details', Comp.pageLoader());
        try {
            const s = await api.get(`/admin/subscriptions/${id}`);
            modal.open('Subscription Details',
                `<div class="space-y-4">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('ID', s.id)}
                        ${Comp.detailRow('User', s.userEmail || '—')}
                        ${Comp.detailRow('Plan', s.planName || '—')}
                        ${Comp.detailRow('Status', utils.statusBadge(s.status))}
                        ${Comp.detailRow('Price', '$' + (s.price || 0).toFixed(2) + '/' + (s.billingCycle || 'mo'))}
                        ${Comp.detailRow('Start Date', utils.formatDate(s.startDate))}
                        ${Comp.detailRow('End Date', s.endDate ? utils.formatDate(s.endDate) : 'Ongoing')}
                        ${Comp.detailRow('Auto Renew', s.autoRenew ? 'Yes' : 'No')}
                        ${Comp.detailRow('Promo Code', s.promoCode || 'None')}
                        ${Comp.detailRow('Discount', s.discountPercent ? s.discountPercent + '%' : 'None')}
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                 ${s.status === 'Active' ? `<button class="btn btn-warning" id="modal-cancel-sub-btn">Cancel Subscription</button>` : ''}`
            );
            document.getElementById('modal-cancel-sub-btn')?.addEventListener('click', () => { modal.close(); cancelSubscription(id); });
        } catch (err) { toast.error('Failed to load subscription: ' + err.message); modal.close(); }
    }

    function statusBadge(status) {
        const map = { Active: 'badge-success', Cancelled: 'badge-danger', Expired: 'badge-muted', Suspended: 'badge-warning', Trial: 'badge-info' };
        return `<span class="badge ${map[status] || 'badge-muted'}">${utils.esc(status || 'Unknown')}</span>`;
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Subscriptions', 'Monitor and manage user subscription plans');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Subscriptions</label>
                    <input class="input-field" id="sub-search" value="${utils.esc(state.search)}" placeholder="Search by user email or plan...">
                </div>
                <div class="form-group mb-0">
                    <label class="form-label">Status</label>
                    <select class="input-field" id="sub-status-filter">
                        <option value="">All Statuses</option>
                        ${['Active','Cancelled','Expired','Suspended','Trial'].map(s => `<option value="${s}" ${state.statusFilter === s ? 'selected' : ''}>${s}</option>`).join('')}
                    </select>
                </div>
                <button class="btn btn-primary btn-sm" id="sub-search-btn">Apply</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-id-card', 'No subscriptions found'); }
        else {
            const rows = state.data.items.map(s => `<tr>
                <td class="font-medium">${utils.esc(s.userEmail || '—')}</td>
                <td><span class="font-semibold text-vs-accent">${utils.esc(s.planName || '—')}</span></td>
                <td class="font-bold">$${(s.price || 0).toFixed(2)}<span class="text-muted text-xs">/${s.billingCycle || 'mo'}</span></td>
                <td>${statusBadge(s.status)}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(s.startDate)}</td>
                <td class="text-muted text-sm">${s.endDate ? utils.formatDateShort(s.endDate) : '<span class="badge badge-success">Ongoing</span>'}</td>
                <td>${s.autoRenew ? '<i class="fas fa-check text-vs-success"></i>' : '<i class="fas fa-times text-vs-danger"></i>'}</td>
                <td class="text-right">
                    <div class="flex items-center justify-end gap-1.5">
                        <button class="btn btn-ghost btn-sm view-sub-btn" data-id="${s.id}" title="View Details"><i class="fas fa-eye text-xs"></i></button>
                        ${s.status === 'Active' ? `<button class="btn btn-ghost btn-sm text-vs-warning cancel-sub-btn" data-id="${s.id}" title="Cancel"><i class="fas fa-ban text-xs"></i></button>` : ''}
                    </div>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'userEmail', label: 'User' }, { key: 'planName', label: 'Plan' }, { key: 'price', label: 'Price' }, { key: 'status', label: 'Status' }, { key: 'startDate', label: 'Start' }, { key: 'endDate', label: 'End' }, { key: 'autoRenew', label: 'Auto Renew' }, { key: '', label: '' }],
                rows, 'No subscriptions',
                { tableId: 'subs-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('sub-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('sub-search').value.trim();
            state.statusFilter = document.getElementById('sub-status-filter').value;
            state.page = 1; loadData();
        });
        document.querySelectorAll('.view-sub-btn').forEach(b => b.addEventListener('click', () => showDetails(parseInt(b.dataset.id))));
        document.querySelectorAll('.cancel-sub-btn').forEach(b => b.addEventListener('click', () => cancelSubscription(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
