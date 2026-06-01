pages.adminPaymentMethods = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/payment-methods', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load payment methods: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function deleteMethod(id) {
        if (!await modal.confirm('Remove Payment Method', 'Remove this payment method? This cannot be undone.', 'danger')) return;
        try { await api.delete(`/admin/payment-methods/${id}`); toast.success('Payment method removed'); loadData(); }
        catch (err) { toast.error('Failed to remove: ' + err.message); }
    }

    function cardIcon(type) {
        const icons = { visa: 'fab fa-cc-visa text-blue-400', mastercard: 'fab fa-cc-mastercard text-red-400', amex: 'fab fa-cc-amex text-blue-300', paypal: 'fab fa-paypal text-blue-500' };
        const key = (type || '').toLowerCase();
        return `<i class="${icons[key] || 'fas fa-credit-card text-muted'} text-lg mr-2"></i>`;
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Payment Methods', 'Manage user payment methods and billing cards');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Payment Methods</label>
                    <input class="input-field" id="pm-search" value="${utils.esc(state.search)}" placeholder="Search by user email or card type...">
                </div>
                <button class="btn btn-primary btn-sm" id="pm-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-credit-card', 'No payment methods found'); }
        else {
            const rows = state.data.items.map(m => `<tr>
                <td class="text-muted font-mono">#${m.id}</td>
                <td><span class="font-medium">${utils.esc(m.userEmail || '—')}</span></td>
                <td><div class="flex items-center">${cardIcon(m.cardType)}<span>${utils.esc(m.cardType || 'Unknown')}</span></div></td>
                <td class="font-mono text-sm">•••• •••• •••• ${utils.esc(m.last4 || '????')}</td>
                <td class="text-muted text-sm">${m.expiryMonth ? `${String(m.expiryMonth).padStart(2,'0')}/${m.expiryYear}` : '—'}</td>
                <td>${m.isDefault ? '<span class="badge badge-success">Default</span>' : '<span class="badge badge-muted">Secondary</span>'}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(m.createdAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger del-pm-btn" data-id="${m.id}" title="Remove"><i class="fas fa-trash text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'userEmail', label: 'User' }, { key: 'cardType', label: 'Card Type' }, { key: 'last4', label: 'Number' }, { key: 'expiry', label: 'Expiry' }, { key: 'isDefault', label: 'Status' }, { key: 'createdAt', label: 'Added' }, { key: '', label: '' }],
                rows, 'No payment methods',
                { tableId: 'pm-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('pm-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('pm-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-pm-btn').forEach(b => b.addEventListener('click', () => deleteMethod(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
