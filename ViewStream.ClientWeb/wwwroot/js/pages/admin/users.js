pages.adminUsers = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true };
    let sortKey = 'id';
    let sortDir = 'asc';

    const ALL_ROLES = ['User', 'SuperAdmin', 'ContentManager', 'Moderator', 'Marketing', 'DataProtectionOfficer', 'UserManager'];

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/users', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load users: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-users-content');
        if (!c) return;
        let h = Comp.pageHeader('User Management', 'Monitor user accounts, roles, access permissions, and account moderation.', 
            '<button class="btn btn-secondary btn-sm" id="users-refresh"><i class="fas fa-sync-alt mr-1"></i> Refresh</button>');
        h += `<div class="card p-4 mb-6 font-body">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="users-search" value="${toast.esc(state.search)}" placeholder="Search by name, email, phone...">
                </div>
                <div class="flex items-center gap-2 mb-2 flex-shrink-0">
                    <input type="checkbox" id="users-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="users-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="users-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-users', 'No users found'); }
        else {
            const rows = state.data.items.map(u => {
                const roles = (u.roles || []).map(r => utils.roleBadge(r)).join(' ');
                let statusBadge = utils.boolBadge(u.isActive);
                if (u.isDeleted) {
                    statusBadge = '<span class="badge badge-danger">Deleted</span>';
                }
                return `<tr data-user-id="${u.id}" class="${u.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted font-mono">#${u.id}</td>
                    <td class="font-medium">${toast.esc(u.fullName || '—')}</td>
                    <td>${toast.esc(u.email)}</td>
                    <td><div class="flex flex-wrap gap-1">${roles || '<span class="text-xs text-muted">—</span>'}</div></td>
                    <td>${statusBadge}</td>
                    <td>${u.isBlocked ? '<span class="badge badge-danger"><i class="fas fa-ban mr-1 text-xs"></i>Blocked</span>' : '<span class="text-muted">No</span>'}</td>
                    <td class="text-muted text-sm font-mono">${utils.formatDateShort(u.createdAt)}</td>
                    <td class="text-right">
                        <button class="btn btn-ghost btn-sm view-user-btn" data-id="${u.id}"><i class="fas fa-eye text-xs"></i></button>
                    </td>
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
            state.includeDeleted = document.getElementById('users-include-deleted').checked;
            state.page = 1; loadData();
        });
        document.getElementById('users-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });
        document.querySelectorAll('.view-user-btn').forEach(b => b.addEventListener('click', () => showUserDetail(parseInt(b.dataset.id))));
    }

    async function showUserDetail(id) {
        modal.open('User Details', Comp.pageLoader(), { large: true });
        try {
            const u = await api.get(`/admin/users/${id}`);
            const roles = (u.roles || []).map(r => utils.roleBadge(r)).join(' ');
            
            let footerBtns = `<button class="btn btn-secondary" onclick="modal.close()">Close</button>`;
            if (u.isDeleted) {
                footerBtns += `<button class="btn btn-success" id="restore-user-btn"><i class="fas fa-trash-restore mr-1.5"></i>Restore</button>`;
            } else {
                footerBtns += `<button class="btn btn-primary" id="edit-user-btn"><i class="fas fa-edit mr-1"></i> Edit & Roles</button>
                    ${u.isBlocked ? '<button class="btn btn-success" id="unblock-user-btn"><i class="fas fa-unlock mr-1"></i> Unblock</button>' : '<button class="btn btn-danger" id="block-user-btn"><i class="fas fa-ban mr-1"></i> Block</button>'}
                    <button class="btn btn-danger" id="delete-user-btn"><i class="fas fa-trash-alt mr-1"></i> Delete</button>`;
            }

            modal.open(`User #${u.id}`,
                `<div class="space-y-6 font-body">
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('ID', u.id)}
                        ${Comp.detailRow('Full Name', toast.esc(u.fullName) || '—')}
                        ${Comp.detailRow('Email', toast.esc(u.email))}
                        ${Comp.detailRow('Phone', toast.esc(u.phoneNumber) || '—')}
                        ${Comp.detailRow('Country', toast.esc(u.countryCode) || '—')}
                        ${Comp.detailRow('Status', u.isDeleted ? '<span class="badge badge-danger">Deleted</span>' : utils.boolBadge(u.isActive))}
                        ${Comp.detailRow('Blocked', u.isBlocked ? '<span class="badge badge-danger">Yes</span>' : '<span class="text-muted">No</span>')}
                        ${Comp.detailRow('Blocked Reason', toast.esc(u.blockedReason) || '—')}
                        ${Comp.detailRow('Blocked Until', u.blockedUntil ? utils.formatDate(u.blockedUntil) : '—')}
                        ${Comp.detailRow('Created', utils.formatDate(u.createdAt))}
                        ${u.isDeleted ? Comp.detailRow('Deleted At', utils.formatDate(u.deletedAt)) : ''}
                    </div>
                    <div>
                        <div class="font-semibold text-vs-text mb-2">Assigned Roles</div>
                        <div class="flex flex-wrap gap-1.5">${roles || '<span class="text-sm text-muted">No roles assigned</span>'}</div>
                    </div>
                </div>`,
                { footer: footerBtns }
            );

            document.getElementById('restore-user-btn')?.addEventListener('click', async () => {
                if (!await modal.confirm('Restore User', 'Are you sure you want to restore this soft-deleted user account?', 'success')) return;
                try {
                    await api.post(`/admin/users/${id}/restore`);
                    toast.success('User account restored successfully');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });

            document.getElementById('edit-user-btn')?.addEventListener('click', () => {
                modal.open('Edit User & Roles', 
                    `<div class="space-y-4 font-body">
                        <div class="form-group">
                            <label class="form-label">Full Name</label>
                            <input class="input-field" id="edit-name" value="${toast.esc(u.fullName || '')}">
                        </div>
                        <div class="form-group">
                            <label class="form-label">Phone Number</label>
                            <input class="input-field" id="edit-phone" value="${toast.esc(u.phoneNumber || '')}">
                        </div>
                        <div class="form-group">
                            <label class="form-label">Account Status</label>
                            <select class="input-field" id="edit-active">
                                <option value="true" ${u.isActive ? 'selected' : ''}>Active</option>
                                <option value="false" ${!u.isActive ? 'selected' : ''}>Inactive</option>
                            </select>
                        </div>
                        <div>
                            <label class="form-label mb-2">Roles Assignment</label>
                            <div class="grid grid-cols-2 gap-2 p-3 rounded-xl bg-vs-surface border border-vs-border">
                                ${ALL_ROLES.map(r => {
                                    const checked = (u.roles || []).includes(r) ? 'checked' : '';
                                    return `
                                        <div class="flex items-center gap-2">
                                            <input type="checkbox" id="role-${r}" class="form-checkbox" value="${r}" ${checked}>
                                            <label for="role-${r}" class="form-label mb-0 cursor-pointer text-xs font-semibold">${r}</label>
                                        </div>
                                    `;
                                }).join('')}
                            </div>
                        </div>
                    </div>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-primary" id="save-edit-btn">Save Changes</button>`
                );

                document.getElementById('save-edit-btn')?.addEventListener('click', async () => {
                    const fullName = document.getElementById('edit-name').value.trim();
                    const phoneNumber = document.getElementById('edit-phone').value.trim() || undefined;
                    const isActive = document.getElementById('edit-active').value === 'true';
                    
                    const roles = [];
                    ALL_ROLES.forEach(r => {
                        if (document.getElementById(`role-${r}`).checked) {
                            roles.push(r);
                        }
                    });

                    try {
                        await api.put(`/admin/users/${id}`, { fullName, phoneNumber, isActive, roles });
                        toast.success('User and roles updated successfully');
                        modal.close();
                        loadData();
                    } catch (err) { toast.error(err.message); }
                });
            });

            document.getElementById('block-user-btn')?.addEventListener('click', () => {
                modal.open('Block User Account',
                    `<div class="space-y-4 font-body">
                        <div class="form-group">
                            <label class="form-label">Block Reason <span class="text-vs-danger">*</span></label>
                            <textarea class="input-field" id="block-reason" rows="3" placeholder="Specify clear reason for blocking this user account..." required></textarea>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Block Until (Optional - leave empty for permanent block)</label>
                            <input type="datetime-local" class="input-field" id="block-until">
                        </div>
                    </div>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-danger" id="confirm-block-btn"><i class="fas fa-ban mr-1.5"></i> Block Account</button>`
                );

                document.getElementById('confirm-block-btn')?.addEventListener('click', async () => {
                    const reason = document.getElementById('block-reason').value.trim();
                    const untilVal = document.getElementById('block-until').value;
                    const blockedUntil = untilVal ? new Date(untilVal).toISOString() : null;

                    if (!reason) {
                        toast.warn('Block reason is required');
                        return;
                    }

                    try {
                        await api.post(`/admin/users/${id}/block`, { reason, blockedUntil });
                        toast.success('User has been blocked');
                        modal.close();
                        loadData();
                    } catch (err) { toast.error(err.message); }
                });
            });

            document.getElementById('unblock-user-btn')?.addEventListener('click', async () => {
                if (!await modal.confirm('Unblock User', 'Are you sure you want to unblock this user account?', 'primary')) return;
                try {
                    await api.post(`/admin/users/${id}/unblock`);
                    toast.success('User has been unblocked');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });

            document.getElementById('delete-user-btn')?.addEventListener('click', async () => {
                if (!await modal.confirm('Soft-Delete User', 'Soft-delete this user account? The user will not be able to log in, and they will be filtered as deleted.', 'danger')) return;
                try {
                    await api.del(`/admin/users/${id}`);
                    toast.success('User account soft-deleted');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) { toast.error('Failed to load user: ' + err.message); modal.close(); }
    }

    return {
        render() { return '<div id="admin-users-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();