pages.adminInvoices = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'invoiceDate', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/invoices', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load invoices: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Invoices & Billing', 'View all user invoices and billing records');
        h += `<div class="card p-4 mb-6 font-body">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Invoices</label>
                    <input class="input-field" id="inv-search" value="${toast.esc(state.search)}" placeholder="Search by user email or status...">
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="inv-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-file-invoice-dollar', 'No invoices found'); }
        else {
            const rows = state.data.items.map(inv => `<tr>
                <td class="text-muted font-mono">#${inv.id}</td>
                <td class="font-medium">${toast.esc(inv.userEmail || '—')}</td>
                <td class="font-bold text-vs-success">$${(inv.amount || 0).toFixed(2)}</td>
                <td>${utils.statusBadge(inv.status)}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(inv.invoiceDate)}</td>
                <td class="text-right">
                    <div class="flex items-center justify-end gap-1.5">
                        <button class="btn btn-ghost btn-sm view-inv-btn" data-id="${inv.id}" title="View Details"><i class="fas fa-eye text-xs"></i></button>
                        <a href="${CONFIG.API_BASE}/admin/invoices/${inv.id}/pdf" target="_blank" class="btn btn-ghost btn-sm text-vs-accent" title="Download PDF"><i class="fas fa-file-pdf text-xs"></i></a>
                    </div>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'userEmail', label: 'User Email' }, { key: 'amount', label: 'Amount' }, { key: 'status', label: 'Status' }, { key: 'invoiceDate', label: 'Date' }, { key: '', label: '' }],
                rows, 'No invoices found',
                { tableId: 'invoices-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('inv-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('inv-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.view-inv-btn').forEach(btn => btn.addEventListener('click', () => showDetailsModal(parseInt(btn.dataset.id))));
    }

    async function showDetailsModal(id) {
        modal.open('Invoice Details', Comp.pageLoader());
        try {
            const inv = await api.get(`/admin/invoices/${id}`);
            modal.open(`Invoice #${inv.id}`,
                `<div class="space-y-4 font-body">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('ID', inv.id)}
                        ${Comp.detailRow('User Email', inv.userEmail || '—')}
                        ${Comp.detailRow('Amount', '$' + (inv.amount || 0).toFixed(2))}
                        ${Comp.detailRow('Status', utils.statusBadge(inv.status))}
                        ${Comp.detailRow('Invoice Date', utils.formatDate(inv.invoiceDate))}
                        ${Comp.detailRow('Plan', inv.subscriptionPlan || '—')}
                        ${Comp.detailRow('Due Date', utils.formatDate(inv.dueDate))}
                        ${Comp.detailRow('Paid At', inv.paidAt ? utils.formatDate(inv.paidAt) : '—')}
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                 <a href="${CONFIG.API_BASE}/admin/invoices/${id}/pdf" target="_blank" class="btn btn-primary"><i class="fas fa-file-pdf mr-1.5"></i> Download PDF</a>`
            );
        } catch (err) { toast.error('Failed to load invoice: ' + err.message); modal.close(); }
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
