pages.adminRatings = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/ratings', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load ratings: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function deleteRating(profileId, showId) {
        if (!await modal.confirm('Delete Rating', 'Remove this user rating?', 'danger')) return;
        try { await api.delete(`/admin/ratings/${profileId}/${showId}`); toast.success('Rating removed'); loadData(); }
        catch (err) { toast.error('Failed to remove: ' + err.message); }
    }

    function starRating(value, max = 5) {
        const full = Math.round(value || 0);
        return `<div class="flex items-center gap-0.5">
            ${Array.from({ length: max }, (_, i) => `<i class="fas fa-star text-xs ${i < full ? 'text-yellow-400' : 'text-vs-border'}"></i>`).join('')}
            <span class="text-muted text-xs ml-1">${(value || 0).toFixed(1)}</span>
        </div>`;
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('User Ratings', 'Manage content ratings submitted by users');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Ratings</label>
                    <input class="input-field" id="rat-search" value="${utils.esc(state.search)}" placeholder="Search by content or user...">
                </div>
                <button class="btn btn-primary btn-sm" id="rat-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-star', 'No ratings found'); }
        else {
            const rows = state.data.items.map(r => `<tr>
                <td class="text-muted font-mono">${r.profileId} / ${r.showId}</td>
                <td class="font-medium">${utils.esc(r.profileName || '—')}</td>
                <td>${utils.esc(r.showTitle || '—')}</td>
                <td>${starRating(r.score)}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(r.createdAt || r.ratedAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger del-rat-btn" data-profile-id="${r.profileId}" data-show-id="${r.showId}" title="Delete"><i class="fas fa-trash text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'profileId', label: 'Profile / Show ID' }, { key: 'profileName', label: 'User' }, { key: 'showTitle', label: 'Content' }, { key: 'score', label: 'Rating' }, { key: 'createdAt', label: 'Date' }, { key: '', label: '' }],
                rows, 'No ratings',
                { tableId: 'ratings-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('rat-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('rat-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-rat-btn').forEach(b => b.addEventListener('click', () => deleteRating(parseInt(b.dataset.profileId), parseInt(b.dataset.showId))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
