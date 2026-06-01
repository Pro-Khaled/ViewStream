pages.adminSharedLists = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/sharedlists', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load shared lists: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function deleteList(id) {
        if (!await modal.confirm('Delete List', 'Permanently delete this shared list and all its items?', 'danger')) return;
        try { await api.delete(`/admin/sharedlists/${id}`); toast.success('List deleted'); loadData(); }
        catch (err) { toast.error('Failed to delete: ' + err.message); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Shared Lists', 'Manage community shared content lists');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Lists</label>
                    <input class="input-field" id="shl-search" value="${utils.esc(state.search)}" placeholder="Search by title or owner...">
                </div>
                <button class="btn btn-primary btn-sm" id="shl-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-list-ul', 'No shared lists found'); }
        else {
            const rows = state.data.items.map(l => `<tr>
                <td class="font-medium">${utils.esc(l.title || '—')}</td>
                <td class="text-muted text-sm">${utils.esc(l.description || '—').substring(0, 50)}</td>
                <td class="text-muted text-sm">${utils.esc(l.ownerEmail || '—')}</td>
                <td class="text-center font-mono">${l.itemCount ?? 0}</td>
                <td>${l.isPublic ? '<span class="badge badge-success">Public</span>' : '<span class="badge badge-muted">Private</span>'}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(l.createdAt)}</td>
                <td class="text-right">
                    <div class="flex items-center justify-end gap-1.5">
                        <button class="btn btn-ghost btn-sm view-items-btn" data-id="${l.id}" title="View Items" onclick="router.navigate('/admin/shared-list-items?listId=${l.id}')"><i class="fas fa-list text-xs"></i></button>
                        <button class="btn btn-ghost btn-sm text-vs-danger del-shl-btn" data-id="${l.id}" title="Delete"><i class="fas fa-trash text-xs"></i></button>
                    </div>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'title', label: 'Title' }, { key: 'description', label: 'Description' }, { key: 'ownerEmail', label: 'Owner' }, { key: 'itemCount', label: 'Items' }, { key: 'isPublic', label: 'Visibility' }, { key: 'createdAt', label: 'Created' }, { key: '', label: '' }],
                rows, 'No shared lists',
                { tableId: 'shl-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('shl-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('shl-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-shl-btn').forEach(b => b.addEventListener('click', () => deleteList(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
