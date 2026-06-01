pages.adminSharedListItems = (() => {
    let state = { page: 1, search: '', listId: null, data: null, loading: true, sortKey: 'addedAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            const params = {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            };
            if (state.listId) params.sharedListId = state.listId;
            state.data = await api.get('/admin/shared-list-items', params);
        } catch (err) { toast.error('Failed to load items: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function deleteItem(id) {
        if (!await modal.confirm('Remove Item', 'Remove this item from the shared list?', 'danger')) return;
        try { await api.delete(`/admin/shared-list-items/${id}`); toast.success('Item removed'); loadData(); }
        catch (err) { toast.error('Failed to remove: ' + err.message); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        const listContext = state.listId ? ` — List #${state.listId}` : '';
        let h = Comp.pageHeader('Shared List Items', `Content items inside shared lists${listContext}`);
        if (state.listId) {
            h += `<div class="mb-4"><button class="btn btn-ghost btn-sm text-vs-accent" onclick="router.navigate('/admin/shared-lists')"><i class="fas fa-arrow-left mr-1.5 text-xs"></i> Back to Lists</button></div>`;
        }
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Items</label>
                    <input class="input-field" id="sli-search" value="${utils.esc(state.search)}" placeholder="Search by content title...">
                </div>
                <button class="btn btn-primary btn-sm" id="sli-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-film', 'No items found in this list'); }
        else {
            const rows = state.data.items.map(i => `<tr>
                <td class="text-muted font-mono">#${i.id}</td>
                <td class="font-medium">${utils.esc(i.contentTitle || '—')}</td>
                <td><span class="badge badge-info">${utils.esc(i.contentType || 'Unknown')}</span></td>
                <td class="text-muted text-sm">${utils.esc(i.listTitle || '—')}</td>
                <td class="text-muted text-sm">${utils.esc(i.addedByEmail || '—')}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(i.addedAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger del-sli-btn" data-id="${i.id}" title="Remove"><i class="fas fa-trash text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'contentTitle', label: 'Content' }, { key: 'contentType', label: 'Type' }, { key: 'listTitle', label: 'List' }, { key: 'addedByEmail', label: 'Added By' }, { key: 'addedAt', label: 'Date' }, { key: '', label: '' }],
                rows, 'No items found',
                { tableId: 'sli-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('sli-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('sli-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-sli-btn').forEach(b => b.addEventListener('click', () => deleteItem(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init(params) {
            state.listId = params?.listId ? parseInt(params.listId) : null;
            loadData();
        }
    };
})();
