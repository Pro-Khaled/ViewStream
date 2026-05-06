pages.adminRoles = (() => {
    let state = { roles: [], permissions: [], loading: true };
    async function loadData() {
        state.loading = true; render();
        try {
            const [roles, perms] = await Promise.all([
                api.get('/admin/roles'),
                api.get('/admin/permissions')
            ]);
            state.roles = roles;
            state.permissions = perms.items;
        } catch (err) { toast.error('Failed to load roles'); }
        state.loading = false; render();
    }
    function render() {
        const c = document.getElementById('admin-roles-content');
        if (!c) return;
        let h = Comp.pageHeader('Roles & Permissions');
        if (state.loading) { h += Comp.pageLoader(); }
        else {
            h += `<div class="grid grid-cols-1 xl:grid-cols-2 gap-6">
                <div class="card p-4">
                    <h3 class="font-display font-semibold mb-3">Roles</h3>
                    ${state.roles.map(r => `<div class="flex items-center justify-between py-2 border-b border-vs-border last:border-0">
                        <div><p class="text-sm font-medium text-vs-text">${toast.esc(r.name)}</p><p class="text-xs text-vs-muted">${toast.esc(r.description || '')} ${r.isSystem ? '<span class="text-vs-warning">(System)</span>' : ''}</p></div>
                        <button class="btn btn-ghost btn-sm"><i class="fas fa-edit"></i></button>
                    </div>`).join('')}
                </div>
                <div class="card p-4">
                    <h3 class="font-display font-semibold mb-3">Permissions</h3>
                    ${state.permissions.map(p => `<div class="py-2 border-b border-vs-border last:border-0">
                        <p class="text-sm font-mono text-vs-text">${toast.esc(p.name)}</p><p class="text-xs text-vs-muted">${toast.esc(p.groupName)}</p>
                    </div>`).join('')}
                </div>
            </div>`;
        }
        c.innerHTML = h;
    }
    return {
        render() { return '<div id="admin-roles-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();