pages.adminUserLibraries = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'addedAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/userlibraries', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load user libraries: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function removeFromLibrary(id) {
        if (!await modal.confirm('Remove from Library', 'Remove this item from the user\'s library?', 'danger')) return;
        try { await api.delete(`/admin/userlibraries/${id}`); toast.success('Item removed'); loadData(); }
        catch (err) { toast.error('Failed to remove: ' + err.message); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('User Libraries', 'Manage user saved/purchased content libraries');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Libraries</label>
                    <input class="input-field" id="ul-search" value="${utils.esc(state.search)}" placeholder="Search by user email or content...">
                </div>
                <button class="btn btn-primary btn-sm" id="ul-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-bookmark', 'No library items found'); }
        else {
            const rows = state.data.items.map(l => `<tr>
                <td class="text-muted font-mono">#${l.id}</td>
                <td class="font-medium">${utils.esc(l.userEmail || '—')}</td>
                <td>${utils.esc(l.contentTitle || '—')}</td>
                <td><span class="badge badge-info">${utils.esc(l.contentType || 'Unknown')}</span></td>
                <td>${l.isPurchased ? '<span class="badge badge-success">Purchased</span>' : '<span class="badge badge-muted">Saved</span>'}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(l.addedAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger del-ul-btn" data-id="${l.id}" title="Remove"><i class="fas fa-trash text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'userEmail', label: 'User' }, { key: 'contentTitle', label: 'Content' }, { key: 'contentType', label: 'Type' }, { key: 'isPurchased', label: 'Status' }, { key: 'addedAt', label: 'Added' }, { key: '', label: '' }],
                rows, 'No library items',
                { tableId: 'ul-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('ul-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('ul-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-ul-btn').forEach(b => b.addEventListener('click', () => removeFromLibrary(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
