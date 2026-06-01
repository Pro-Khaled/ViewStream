pages.adminProfiles = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/profiles', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load profiles: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function deleteProfile(id) {
        if (!await modal.confirm('Delete Profile', 'Permanently delete this user profile? All profile data will be lost.', 'danger')) return;
        try { await api.delete(`/admin/profiles/${id}`); toast.success('Profile deleted'); loadData(); }
        catch (err) { toast.error('Failed to delete: ' + err.message); }
    }

    async function showDetails(id) {
        modal.open('Profile Details', Comp.pageLoader());
        try {
            const p = await api.get(`/admin/profiles/${id}`);
            modal.open(`Profile: ${p.name}`,
                `<div class="space-y-4">
                    <div class="flex items-center gap-4 p-4 card bg-vs-surface-2">
                        <div class="w-14 h-14 rounded-full flex items-center justify-center text-2xl" style="background:${p.avatarColor || '#6366f1'}">
                            ${p.avatarUrl ? `<img src="${utils.esc(p.avatarUrl)}" class="w-full h-full rounded-full object-cover">` : `<span>${(p.name || '?')[0].toUpperCase()}</span>`}
                        </div>
                        <div>
                            <h3 class="text-lg font-semibold">${utils.esc(p.name || '—')}</h3>
                            <span class="text-muted text-sm">${p.isKidsProfile ? '👶 Kids Profile' : '👤 Standard Profile'}</span>
                        </div>
                    </div>
                    <div class="grid grid-cols-2 gap-4">
                        ${Comp.detailRow('ID', p.id)}
                        ${Comp.detailRow('User', p.userEmail || '—')}
                        ${Comp.detailRow('Language', p.language || 'Default')}
                        ${Comp.detailRow('Maturity Rating', p.maturityRating || '—')}
                        ${Comp.detailRow('Pin Locked', p.isPinEnabled ? 'Yes' : 'No')}
                        ${Comp.detailRow('Created', utils.formatDate(p.createdAt))}
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                 <button class="btn btn-danger" id="modal-del-profile-btn">Delete Profile</button>`
            );
            document.getElementById('modal-del-profile-btn')?.addEventListener('click', () => { modal.close(); deleteProfile(id); });
        } catch (err) { toast.error('Failed to load profile: ' + err.message); modal.close(); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('User Profiles', 'Manage individual viewing profiles across accounts');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Profiles</label>
                    <input class="input-field" id="prof-search" value="${utils.esc(state.search)}" placeholder="Search by name or user email...">
                </div>
                <button class="btn btn-primary btn-sm" id="prof-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-user-circle', 'No profiles found'); }
        else {
            const rows = state.data.items.map(p => `<tr>
                <td>
                    <div class="flex items-center gap-3">
                        <div class="w-8 h-8 rounded-full flex items-center justify-center text-sm font-bold flex-shrink-0" style="background:${p.avatarColor || '#6366f1'}">
                            ${(p.name || '?')[0].toUpperCase()}
                        </div>
                        <span class="font-medium">${utils.esc(p.name || '—')}</span>
                    </div>
                </td>
                <td class="text-muted text-sm">${utils.esc(p.userEmail || '—')}</td>
                <td>${p.isKidsProfile ? '<span class="badge badge-info">Kids</span>' : '<span class="badge badge-muted">Standard</span>'}</td>
                <td class="text-muted text-sm">${utils.esc(p.language || 'Default')}</td>
                <td class="text-muted text-sm">${utils.esc(p.maturityRating || '—')}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(p.createdAt)}</td>
                <td class="text-right">
                    <div class="flex items-center justify-end gap-1.5">
                        <button class="btn btn-ghost btn-sm view-prof-btn" data-id="${p.id}" title="View Details"><i class="fas fa-eye text-xs"></i></button>
                        <button class="btn btn-ghost btn-sm text-vs-danger del-prof-btn" data-id="${p.id}" title="Delete"><i class="fas fa-trash text-xs"></i></button>
                    </div>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'name', label: 'Name' }, { key: 'userEmail', label: 'Account' }, { key: 'isKidsProfile', label: 'Type' }, { key: 'language', label: 'Language' }, { key: 'maturityRating', label: 'Rating' }, { key: 'createdAt', label: 'Created' }, { key: '', label: '' }],
                rows, 'No profiles found',
                { tableId: 'profiles-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('prof-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('prof-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.view-prof-btn').forEach(b => b.addEventListener('click', () => showDetails(parseInt(b.dataset.id))));
        document.querySelectorAll('.del-prof-btn').forEach(b => b.addEventListener('click', () => deleteProfile(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
