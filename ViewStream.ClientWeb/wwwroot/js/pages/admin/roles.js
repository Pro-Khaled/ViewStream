pages.adminRoles = (() => {
    let state = { roles: [], permissions: [], loading: true };

    async function loadData() {
        state.loading = true; render();
        try {
            const [roles, perms] = await Promise.all([
                api.get('/admin/roles'),
                api.get('/admin/permissions')
            ]);
            state.roles = roles || [];
            state.permissions = (perms.items || perms) || [];
        } catch (err) { toast.error('Failed to load roles'); }
        state.loading = false; render();
    }

    function permissionCheckboxes(selected = []) {
        // Group permissions by groupName
        const groups = {};
        state.permissions.forEach(p => {
            const g = p.groupName || 'General';
            if (!groups[g]) groups[g] = [];
            groups[g].push(p);
        });
        return Object.entries(groups).map(([group, perms]) => `
            <div class="mb-3">
                <p class="text-xs font-semibold text-vs-muted uppercase tracking-widest mb-1.5">${toast.esc(group)}</p>
                <div class="grid grid-cols-2 gap-1.5">
                    ${perms.map(p => `
                        <label class="flex items-center gap-2 px-3 py-2 rounded-lg bg-vs-card hover:bg-vs-elevated cursor-pointer border border-transparent has-[:checked]:border-vs-accent/40 has-[:checked]:bg-vs-accent/5 transition-all">
                            <input type="checkbox" class="perm-check accent-vs-accent rounded" value="${p.id}" ${selected.includes(p.id) ? 'checked' : ''}>
                            <span class="text-xs text-vs-text font-mono">${toast.esc(p.name)}</span>
                        </label>`).join('')}
                </div>
            </div>`).join('');
    }

    function getCheckedPermissions() {
        return [...document.querySelectorAll('.perm-check:checked')].map(c => parseInt(c.value));
    }

    function render() {
        const c = document.getElementById('admin-roles-content');
        if (!c) return;
        let h = Comp.pageHeader('Roles & Permissions', 'Manage system roles and their permission sets',
            `<button id="create-role-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus mr-1"></i> New Role</button>`);

        if (state.loading) { h += Comp.pageLoader(); }
        else {
            h += `<div class="grid grid-cols-1 xl:grid-cols-2 gap-6">
                <!-- Roles list -->
                <div>
                    <h2 class="font-display font-semibold text-lg text-vs-text mb-3">Roles (${state.roles.length})</h2>
                    <div class="space-y-2">
                        ${state.roles.map(r => `
                        <div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors group">
                            <div class="min-w-0">
                                <div class="flex items-center gap-2 mb-0.5">
                                    <p class="text-sm font-semibold text-vs-text">${toast.esc(r.name)}</p>
                                    ${r.isSystem ? '<span class="px-1.5 py-0.5 rounded text-xs bg-vs-warning/15 text-vs-warning font-medium">System</span>' : ''}
                                </div>
                                <p class="text-xs text-vs-muted">${toast.esc(r.description || '—')} &middot; ${r.permissionIds?.length || 0} permissions</p>
                            </div>
                            <div class="flex items-center gap-1.5 flex-shrink-0 ml-4 opacity-0 group-hover:opacity-100 transition-opacity">
                                ${!r.isSystem ? `
                                <button class="btn btn-ghost btn-sm edit-role-btn" data-id="${r.id}"><i class="fas fa-edit text-xs"></i></button>
                                <button class="btn btn-ghost btn-sm text-vs-error delete-role-btn" data-id="${r.id}"><i class="fas fa-trash-alt text-xs"></i></button>` : ''}
                            </div>
                        </div>`).join('')}
                    </div>
                </div>
                <!-- Permission catalogue -->
                <div>
                    <h2 class="font-display font-semibold text-lg text-vs-text mb-3">All Permissions (${state.permissions.length})</h2>
                    <div class="rounded-xl bg-vs-surface border border-vs-border p-4 max-h-[500px] overflow-y-auto space-y-1">
                        ${(() => {
                            const groups = {};
                            state.permissions.forEach(p => {
                                const g = p.groupName || 'General';
                                if (!groups[g]) groups[g] = [];
                                groups[g].push(p);
                            });
                            return Object.entries(groups).map(([grp, perms]) => `
                                <div class="mb-3">
                                    <p class="text-xs font-semibold text-vs-muted uppercase tracking-widest mb-1">${toast.esc(grp)}</p>
                                    ${perms.map(p => `<p class="text-xs py-1 px-2 rounded font-mono text-vs-dim hover:bg-vs-card">${toast.esc(p.name)}</p>`).join('')}
                                </div>`).join('');
                        })()}
                    </div>
                </div>
            </div>`;
        }

        c.innerHTML = h;
        bindEvents();
    }

    function showRoleModal(role = null) {
        const isEdit = !!role;
        const selectedIds = isEdit ? (role.permissionIds || []) : [];
        modal.open(
            isEdit ? `Edit Role: ${role.name}` : 'Create New Role',
            `<div class="space-y-4">
                ${!isEdit ? `<div>
                    <label class="form-label">Role Name <span class="text-vs-error">*</span></label>
                    <input type="text" id="role-name" class="input-field" placeholder="e.g. ContentEditor" value="${isEdit ? toast.esc(role.name) : ''}">
                </div>` : ''}
                <div>
                    <label class="form-label">Description</label>
                    <input type="text" id="role-desc" class="input-field" placeholder="Brief description" value="${isEdit ? toast.esc(role.description || '') : ''}">
                </div>
                <div>
                    <label class="form-label">Permissions</label>
                    <div class="max-h-64 overflow-y-auto rounded-xl bg-vs-card border border-vs-border p-3 space-y-0.5">
                        ${permissionCheckboxes(selectedIds)}
                    </div>
                    <p class="text-xs text-vs-muted mt-1.5"><span id="perm-count">${selectedIds.length}</span> permissions selected</p>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-role-btn">${isEdit ? 'Save Changes' : 'Create Role'}</button>`
        );

        // Live count of checked permissions
        document.querySelectorAll('.perm-check').forEach(cb => {
            cb.addEventListener('change', () => {
                document.getElementById('perm-count').textContent = getCheckedPermissions().length;
            });
        });

        document.getElementById('save-role-btn')?.addEventListener('click', async () => {
            const desc = document.getElementById('role-desc').value.trim();
            const permissionIds = getCheckedPermissions();
            try {
                if (isEdit) {
                    await api.put(`/admin/roles/${role.id}`, { description: desc, permissionIds });
                    toast.success(`Role "${role.name}" updated`);
                } else {
                    const name = document.getElementById('role-name').value.trim();
                    if (!name) { toast.warning('Role name is required'); return; }
                    await api.post('/admin/roles', { name, description: desc, permissionIds });
                    toast.success(`Role "${name}" created`);
                }
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function bindEvents() {
        document.getElementById('create-role-btn')?.addEventListener('click', () => showRoleModal());

        document.getElementById('admin-roles-content')?.addEventListener('click', async e => {
            const btn = e.target.closest('button');
            if (!btn) return;

            if (btn.classList.contains('edit-role-btn')) {
                const id = btn.dataset.id;
                const role = state.roles.find(r => r.id == id);
                if (role) showRoleModal(role);
            }

            if (btn.classList.contains('delete-role-btn')) {
                const id = btn.dataset.id;
                const role = state.roles.find(r => r.id == id);
                modal.open('Delete Role',
                    `<p class="text-sm text-vs-dim">Permanently delete role <strong class="text-vs-text">"${toast.esc(role?.name)}"</strong>? Users with this role will lose its permissions.</p>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-danger" id="confirm-del-role">Delete</button>`);
                document.getElementById('confirm-del-role')?.addEventListener('click', async () => {
                    try {
                        await api.del(`/admin/roles/${id}`);
                        toast.success('Role deleted');
                        modal.close();
                        loadData();
                    } catch (err) { toast.error(err.message); }
                });
            }
        });
    }

    return {
        render() { return '<div id="admin-roles-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();