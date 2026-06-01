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
            state.permissions = perms || [];
        } catch (err) { toast.error('Failed to load roles and permissions: ' + err.message); }
        state.loading = false; render();
    }

    function permissionCheckboxesGrouped(selected = []) {
        const groups = {};
        state.permissions.forEach(p => {
            const g = p.groupName || 'General';
            if (!groups[g]) groups[g] = [];
            groups[g].push(p);
        });

        return Object.entries(groups).map(([group, perms]) => {
            const groupSafeId = group.replace(/\s+/g, '-').toLowerCase();
            return `
                <div class="mb-4 p-3 rounded-xl bg-vs-surface-2 border border-vs-border">
                    <div class="flex items-center justify-between border-b border-vs-border/60 pb-2 mb-3">
                        <span class="text-xs font-bold text-vs-accent uppercase tracking-wider">${toast.esc(group)}</span>
                        <button type="button" class="btn btn-ghost btn-xxs select-group-all-btn font-semibold text-xxs text-vs-accent" data-target="${groupSafeId}">
                            Select All
                        </button>
                    </div>
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-2" id="group-container-${groupSafeId}">
                        ${perms.map(p => {
                            const isChecked = selected.includes(p.id) ? 'checked' : '';
                            return `
                                <label class="flex items-start gap-2 px-3 py-2 rounded-lg bg-vs-surface border border-vs-border hover:border-vs-accent/40 cursor-pointer transition-all has-[:checked]:border-vs-accent/60 has-[:checked]:bg-vs-accent/5">
                                    <input type="checkbox" class="perm-check form-checkbox mt-0.5" value="${p.id}" ${isChecked}>
                                    <div class="min-w-0">
                                        <div class="text-xs font-semibold text-vs-text font-mono truncate">${toast.esc(p.name)}</div>
                                        ${p.description ? `<div class="text-xxs text-vs-muted mt-0.5 leading-tight">${toast.esc(p.description)}</div>` : ''}
                                    </div>
                                </label>
                            `;
                        }).join('')}
                    </div>
                </div>
            `;
        }).join('');
    }

    function getCheckedPermissions() {
        return [...document.querySelectorAll('.perm-check:checked')].map(c => parseInt(c.value));
    }

    function render() {
        const c = document.getElementById('admin-roles-content');
        if (!c) return;
        let h = Comp.pageHeader('Roles & Permissions', 'Configure user roles and manage access control mapping.',
            `<button id="create-role-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus mr-1 text-xs"></i> Create Role</button>`);

        if (state.loading) { h += Comp.pageLoader(); }
        else {
            h += `<div class="grid grid-cols-1 xl:grid-cols-2 gap-6 font-body">
                <!-- Roles list -->
                <div class="space-y-4">
                    <h2 class="font-display font-semibold text-lg text-vs-text flex items-center gap-2"><i class="fas fa-user-shield text-vs-accent"></i> System Roles</h2>
                    <div class="space-y-3">
                        ${state.roles.map(r => `
                        <div class="card p-5 relative border border-vs-border hover:border-vs-accent/40 transition-all shadow-md group">
                            <div class="flex items-start justify-between">
                                <div>
                                    <div class="flex items-center gap-2 flex-wrap">
                                        <h4 class="font-bold text-vs-text text-base">${toast.esc(r.name)}</h4>
                                        ${r.isSystem ? '<span class="badge badge-warning text-xxs font-bold py-0.5 px-2">System Protected</span>' : ''}
                                    </div>
                                    <p class="text-xs text-vs-muted mt-1 leading-relaxed">${toast.esc(r.description || 'No description provided.')}</p>
                                    <div class="flex items-center gap-1.5 mt-3">
                                        <span class="text-xxs font-bold text-vs-accent bg-vs-accent/10 px-2 py-0.5 rounded-full">${r.permissionIds?.length || 0} permissions mapped</span>
                                    </div>
                                </div>
                                <div class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0">
                                    ${!r.isSystem ? `
                                        <button class="btn btn-ghost btn-sm edit-role-btn" data-id="${r.id}" title="Edit Role"><i class="fas fa-edit text-xs"></i></button>
                                        <button class="btn btn-ghost btn-sm text-vs-danger delete-role-btn" data-id="${r.id}" title="Delete Role"><i class="fas fa-trash text-xs"></i></button>
                                    ` : '<span class="text-xs text-vs-muted font-semibold italic p-1.5">System Lock</span>'}
                                </div>
                            </div>
                        </div>`).join('')}
                    </div>
                </div>

                <!-- Permission catalogue -->
                <div class="space-y-4">
                    <h2 class="font-display font-semibold text-lg text-vs-text flex items-center gap-2"><i class="fas fa-key text-vs-accent"></i> Permissions Registry</h2>
                    <div class="card p-5 border border-vs-border shadow-md space-y-4 max-h-[600px] overflow-y-auto">
                        ${(() => {
                            const groups = {};
                            state.permissions.forEach(p => {
                                const g = p.groupName || 'General';
                                if (!groups[g]) groups[g] = [];
                                groups[g].push(p);
                            });
                            return Object.entries(groups).map(([grp, perms]) => `
                                <div class="p-3 bg-vs-surface-2 rounded-xl border border-vs-border">
                                    <p class="text-xs font-bold text-vs-accent uppercase tracking-widest border-b border-vs-border pb-1.5 mb-2">${toast.esc(grp)}</p>
                                    <div class="space-y-1.5">
                                        ${perms.map(p => `
                                            <div class="p-2 rounded bg-vs-surface border border-vs-border/50 hover:border-vs-dim transition-colors">
                                                <p class="text-xs font-semibold text-vs-text font-mono">${toast.esc(p.name)}</p>
                                                ${p.description ? `<p class="text-xxs text-vs-muted mt-0.5">${toast.esc(p.description)}</p>` : ''}
                                            </div>
                                        `).join('')}
                                    </div>
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
            `<div class="space-y-4 font-body max-h-[70vh] overflow-y-auto pr-2">
                ${!isEdit ? `<div class="form-group">
                    <label class="form-label">Role Name <span class="text-vs-danger">*</span></label>
                    <input type="text" id="role-name" class="input-field" placeholder="e.g. Moderator" value="">
                </div>` : ''}
                <div class="form-group">
                    <label class="form-label">Description</label>
                    <textarea id="role-desc" class="input-field" rows="2" placeholder="Describe the purpose or responsibilities of this role...">${isEdit ? toast.esc(role.description || '') : ''}</textarea>
                </div>
                <div>
                    <label class="form-label mb-2">Grouped Permissions Mappings</label>
                    <div class="space-y-2">
                        ${permissionCheckboxesGrouped(selectedIds)}
                    </div>
                    <div class="flex justify-between items-center mt-3 pt-3 border-t border-vs-border text-xs">
                        <span class="text-vs-muted"><span id="perm-count" class="font-bold text-vs-accent">${selectedIds.length}</span> permissions selected</span>
                        <button type="button" class="btn btn-ghost btn-xxs font-semibold text-vs-accent" id="clear-all-perms">Clear All</button>
                    </div>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-role-btn">${isEdit ? 'Save Changes' : 'Create Role'}</button>`,
            { large: true }
        );

        // Bind Select Group All toggles
        document.querySelectorAll('.select-group-all-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const target = btn.dataset.target;
                const container = document.getElementById(`group-container-${target}`);
                if (!container) return;
                const cbs = container.querySelectorAll('.perm-check');
                const allChecked = [...cbs].every(c => c.checked);
                cbs.forEach(c => c.checked = !allChecked);
                document.getElementById('perm-count').textContent = getCheckedPermissions().length;
            });
        });

        // Live count of checked permissions
        document.querySelectorAll('.perm-check').forEach(cb => {
            cb.addEventListener('change', () => {
                document.getElementById('perm-count').textContent = getCheckedPermissions().length;
            });
        });

        // Clear all button
        document.getElementById('clear-all-perms')?.addEventListener('click', () => {
            document.querySelectorAll('.perm-check').forEach(c => c.checked = false);
            document.getElementById('perm-count').textContent = 0;
        });

        document.getElementById('save-role-btn')?.addEventListener('click', async () => {
            const desc = document.getElementById('role-desc').value.trim();
            const permissionIds = getCheckedPermissions();
            try {
                if (isEdit) {
                    await api.put(`/admin/roles/${role.id}`, { description: desc, permissionIds });
                    toast.success(`Role "${role.name}" updated successfully`);
                } else {
                    const name = document.getElementById('role-name').value.trim();
                    if (!name) { toast.warning('Role name is required'); return; }
                    await api.post('/admin/roles', { name, description: desc, permissionIds });
                    toast.success(`Role "${name}" created successfully`);
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
                if (!await modal.confirm('Delete Role', `Are you sure you want to permanently delete the role "${toast.esc(role?.name)}"? Users with this role will lose its permissions.`, 'danger')) return;
                try {
                    await api.del(`/admin/roles/${id}`);
                    toast.success('Role deleted successfully');
                    loadData();
                } catch (err) { toast.error(err.message); }
            }
        });
    }

    return {
        render() { return '<div id="admin-roles-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();