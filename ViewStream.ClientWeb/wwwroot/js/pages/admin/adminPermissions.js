pages.adminPermissions = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'name', sortDir: 'asc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/permissions', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load permissions: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function createPermission(payload) {
        try { await api.post('/admin/permissions', payload); toast.success('Permission created'); modal.close(); loadData(); }
        catch (err) { toast.error('Failed to create: ' + err.message); }
    }

    async function updatePermission(id, payload) {
        try { await api.put(`/admin/permissions/${id}`, payload); toast.success('Permission updated'); modal.close(); loadData(); }
        catch (err) { toast.error('Failed to update: ' + err.message); }
    }

    async function deletePermission(id) {
        if (!await modal.confirm('Delete Permission', 'Permanently delete this permission? This may break role access.', 'danger')) return;
        try { await api.delete(`/admin/permissions/${id}`); toast.success('Permission deleted'); loadData(); }
        catch (err) { toast.error('Failed to delete: ' + err.message); }
    }

    function openFormModal(perm = null) {
        modal.open(perm ? 'Edit Permission' : 'Create Permission',
            `<div class="space-y-4">
                <div class="form-group"><label class="form-label">Name <span class="text-vs-danger">*</span></label>
                    <input class="input-field" id="perm-name" value="${utils.esc(perm?.name || '')}" placeholder="e.g. content.view"></div>
                <div class="form-group"><label class="form-label">Display Name</label>
                    <input class="input-field" id="perm-display" value="${utils.esc(perm?.displayName || '')}" placeholder="e.g. View Content"></div>
                <div class="form-group"><label class="form-label">Description</label>
                    <textarea class="input-field" id="perm-desc" rows="2" placeholder="Describe what this permission allows...">${utils.esc(perm?.description || '')}</textarea></div>
                <div class="form-group"><label class="form-label">Group / Module</label>
                    <input class="input-field" id="perm-group" value="${utils.esc(perm?.group || '')}" placeholder="e.g. Content Management"></div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="perm-save-btn"><i class="fas fa-save mr-1.5"></i> ${perm ? 'Update' : 'Create'}</button>`
        );
        document.getElementById('perm-save-btn')?.addEventListener('click', () => {
            const name = document.getElementById('perm-name').value.trim();
            if (!name) { toast.warn('Permission name is required'); return; }
            const payload = {
                name,
                displayName: document.getElementById('perm-display').value.trim(),
                description: document.getElementById('perm-desc').value.trim(),
                group: document.getElementById('perm-group').value.trim()
            };
            perm ? updatePermission(perm.id, payload) : createPermission(payload);
        });
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Permissions', 'Manage system permissions and access control',
            `<button class="btn btn-primary btn-sm" id="perm-create-btn"><i class="fas fa-plus mr-1.5"></i> Add Permission</button>`);
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Permissions</label>
                    <input class="input-field" id="perm-search" value="${utils.esc(state.search)}" placeholder="Search by name or group...">
                </div>
                <button class="btn btn-primary btn-sm" id="perm-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-lock', 'No permissions found'); }
        else {
            const rows = state.data.items.map(p => `<tr>
                <td class="font-mono text-vs-accent text-sm">${utils.esc(p.name || '—')}</td>
                <td class="font-medium">${utils.esc(p.displayName || '—')}</td>
                <td class="text-muted text-sm">${utils.esc(p.description || '—')}</td>
                <td><span class="badge badge-info">${utils.esc(p.group || 'General')}</span></td>
                <td class="text-right">
                    <div class="flex items-center justify-end gap-1.5">
                        <button class="btn btn-ghost btn-sm edit-perm-btn" data-id="${p.id}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                        <button class="btn btn-ghost btn-sm text-vs-danger del-perm-btn" data-id="${p.id}" title="Delete"><i class="fas fa-trash text-xs"></i></button>
                    </div>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'name', label: 'Key' }, { key: 'displayName', label: 'Display Name' }, { key: 'description', label: 'Description' }, { key: 'group', label: 'Group' }, { key: '', label: '' }],
                rows, 'No permissions',
                { tableId: 'perms-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('perm-create-btn')?.addEventListener('click', () => openFormModal());
        document.getElementById('perm-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('perm-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.edit-perm-btn').forEach(b => b.addEventListener('click', () => {
            const p = state.data.items.find(x => x.id == b.dataset.id);
            if (p) openFormModal(p);
        }));
        document.querySelectorAll('.del-perm-btn').forEach(b => b.addEventListener('click', () => deletePermission(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
