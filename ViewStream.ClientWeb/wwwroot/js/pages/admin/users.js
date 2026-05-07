pages.adminUsers = (() => {
    let state = { page: 1, search: '', data: null, loading: true };
    let sortKey = 'id';
    let sortDir = 'asc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/users', {
                page: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                search: state.search || undefined,
                orderBy: sortKey,
                isDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load users: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-users-content');
        if (!c) return;
        let h = Comp.pageHeader('User Management', '', '<button class="btn btn-secondary btn-sm" id="users-refresh"><i class="fas fa-sync-alt"></i> Refresh</button>');
        h += `<div class="card p-4 mb-6"><div class="flex items-end gap-4"><div class="flex-1 form-group mb-0"><label class="form-label">Search</label><input class="input-field" id="users-search" value="${toast.esc(state.search)}"></div><button class="btn btn-primary btn-sm" id="users-search-btn">Search</button></div></div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-users', 'No users found'); }
        else {
            const rows = state.data.items.map(u => {
                const roles = (u.roles || []).slice(0, 2).map(r => utils.roleBadge(r)).join(' ');
                return `<tr data-user-id="${u.id}">
                    <td class="text-muted">${u.id}</td>
                    <td class="font-medium">${toast.esc(u.fullName || '—')}</td>
                    <td>${toast.esc(u.email)}</td>
                    <td>${roles}</td>
                    <td>${utils.boolBadge(u.isActive)}</td>
                    <td>${u.isBlocked ? '<span class="badge badge-danger">Blocked</span>' : '<span class="text-muted">No</span>'}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(u.createdAt)}</td>
                    <td><button class="btn btn-ghost btn-sm view-user-btn" data-id="${u.id}"><i class="fas fa-eye"></i></button></td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'fullName', label: 'Name' },
                    { key: 'email', label: 'Email' },
                    { key: '', label: 'Roles' },
                    { key: 'isActive', label: 'Status' },
                    { key: '', label: 'Blocked' },
                    { key: 'createdAt', label: 'Created' },
                    { key: '', label: '' }
                ],
                rows,
                'No users found',
                {
                    tableId: 'users-table',
                    sortKey,
                    sortDir,
                    onSort: (key, dir) => { if (key) { sortKey = key; sortDir = dir; loadData(); } }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('users-refresh')?.addEventListener('click', loadData);
        document.getElementById('users-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('users-search').value.trim();
            state.page = 1; loadData();
        });
        document.querySelectorAll('.view-user-btn').forEach(b => b.addEventListener('click', () => showUserDetail(parseInt(b.dataset.id))));
    }
    async function showUserDetail(id) {
        modal.open('User Details', Comp.pageLoader(), { large: true });
        try {
            const u = await api.get(`/admin/users/${id}`);
            const roles = (u.roles || []).map(r => utils.roleBadge(r)).join(' ');
            modal.open('User #' + u.id,
                `<div class="detail-grid">
                    ${Comp.detailRow('ID', u.id)}
                    ${Comp.detailRow('Full Name', toast.esc(u.fullName) || '—')}
                    ${Comp.detailRow('Email', toast.esc(u.email))}
                    ${Comp.detailRow('Phone', toast.esc(u.phoneNumber) || '—')}
                    ${Comp.detailRow('Country', toast.esc(u.countryCode) || '—')}
                    ${Comp.detailRow('Status', utils.boolBadge(u.isActive))}
                    ${Comp.detailRow('Blocked', u.isBlocked ? '<span class="badge badge-danger">Yes</span>' : '<span class="text-muted">No</span>')}
                    ${Comp.detailRow('Blocked Reason', toast.esc(u.blockedReason) || '—')}
                    ${Comp.detailRow('Blocked Until', u.blockedUntil ? utils.formatDate(u.blockedUntil) : '—')}
                    ${Comp.detailRow('Created', utils.formatDate(u.createdAt))}
                </div>
                <div class="mt-6"><div class="detail-label mb-2">Roles</div><div class="flex flex-wrap gap-2">${roles}</div></div>`,
                {
                    footer: `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                        <button class="btn btn-primary" id="edit-user-btn">Edit</button>
                        ${u.isActive ? '<button class="btn btn-danger" id="block-user-btn">Block</button>' : '<button class="btn btn-success" id="unblock-user-btn">Unblock</button>'}
                        <button class="btn btn-danger" id="delete-user-btn">Delete</button>`
                }
            );
            // Event hooks inside modal (simplified – you can expand)
            document.getElementById('edit-user-btn')?.addEventListener('click', () => {
                modal.open('Edit User', `<div class="form-group"><label class="form-label">Full Name</label><input class="input-field" id="edit-name" value="${toast.esc(u.fullName || '')}"></div><div class="form-group"><label class="form-label">Active</label><select class="input-field" id="edit-active"><option value="true" ${u.isActive ? 'selected' : ''}>Active</option><option value="false" ${!u.isActive ? 'selected' : ''}>Inactive</option></select></div>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-primary" id="save-edit-btn">Save</button>`);
                document.getElementById('save-edit-btn')?.addEventListener('click', async () => {
                    try {
                        await api.put(`/admin/users/${id}`, { fullName: document.getElementById('edit-name').value, isActive: document.getElementById('edit-active').value === 'true' });
                        toast.success('User updated'); modal.close(); loadData();
                    } catch (err) { toast.error(err.message); }
                });
            });
            document.getElementById('block-user-btn')?.addEventListener('click', async () => {
                const reason = prompt('Block reason:') || 'No reason';
                await api.post(`/admin/users/${id}/block`, { reason }); toast.success('User blocked'); modal.close(); loadData();
            });
            document.getElementById('unblock-user-btn')?.addEventListener('click', async () => {
                await api.post(`/admin/users/${id}/unblock`); toast.success('User unblocked'); modal.close(); loadData();
            });
            document.getElementById('delete-user-btn')?.addEventListener('click', async () => {
                if (confirm('Soft-delete this user?')) {
                    await api.del(`/admin/users/${id}`); toast.success('User deleted'); modal.close(); loadData();
                }
            });
        } catch (err) { toast.error('Failed to load user'); modal.close(); }
    }
    return {
        render() { return '<div id="admin-users-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();