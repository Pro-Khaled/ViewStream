pages.adminUserPromoUsages = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'usedAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/userpromousages', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load promo usages: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function revokeUsage(userId, promoCodeId) {
        if (!await modal.confirm('Revoke Promo Usage', 'Revoke this promo code usage? The discount may still apply to the current period.', 'warning')) return;
        try { await api.delete(`/admin/userpromousages/${userId}/${promoCodeId}`); toast.success('Usage revoked'); loadData(); }
        catch (err) { toast.error('Failed to revoke: ' + err.message); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Promo Code Usages', 'Track promotional code usage across all users');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Promo Usages</label>
                    <input class="input-field" id="pu-search" value="${utils.esc(state.search)}" placeholder="Search by code or user email...">
                </div>
                <button class="btn btn-primary btn-sm" id="pu-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-tags', 'No promo usages found'); }
        else {
            const rows = state.data.items.map(u => `<tr>
                <td class="text-muted font-mono">#${u.userId}/${u.promoCodeId}</td>
                <td class="font-medium">${utils.esc(u.userEmail || '—')}</td>
                <td><span class="font-mono font-bold text-vs-accent">${utils.esc(u.promoCode || '—')}</span></td>
                <td class="font-bold text-vs-success">${u.discountPercent ? u.discountPercent + '%' : (u.discountAmount ? '$' + u.discountAmount.toFixed(2) : '—')}</td>
                <td class="text-muted text-sm">${utils.esc(u.planName || '—')}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(u.usedAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger revoke-pu-btn" data-user-id="${u.userId}" data-promo-code-id="${u.promoCodeId}" title="Revoke"><i class="fas fa-times text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'userId', label: 'User / Promo ID' }, { key: 'userEmail', label: 'User' }, { key: 'promoCode', label: 'Code' }, { key: 'discount', label: 'Discount' }, { key: 'planName', label: 'Plan' }, { key: 'usedAt', label: 'Used At' }, { key: '', label: '' }],
                rows, 'No usages found',
                { tableId: 'pu-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('pu-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('pu-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.revoke-pu-btn').forEach(b => b.addEventListener('click', () => revokeUsage(parseInt(b.dataset.userId), parseInt(b.dataset.promoCodeId))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
