pages.adminWatchPartyParticipants = (() => {
    let state = { page: 1, search: '', partyId: null, data: null, loading: true, sortKey: 'joinedAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            const params = {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            };
            if (state.partyId) params.watchPartyId = state.partyId;
            state.data = await api.get('/admin/watchpartyparticipants', params);
        } catch (err) { toast.error('Failed to load participants: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function removeParticipant(partyId, profileId) {
        if (!await modal.confirm('Remove Participant', 'Kick this user from the watch party?', 'danger')) return;
        try { await api.delete(`/admin/watchpartyparticipants/${partyId}/${profileId}`); toast.success('Participant removed'); loadData(); }
        catch (err) { toast.error('Failed to remove participant: ' + err.message); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        const partyContext = state.partyId ? ` — Party #${state.partyId}` : '';
        let h = Comp.pageHeader('Watch Party Participants', `Users participating in watch parties${partyContext}`);
        if (state.partyId) {
            h += `<div class="mb-4"><button class="btn btn-ghost btn-sm text-vs-accent" onclick="router.navigate('/admin/watch-parties')"><i class="fas fa-arrow-left mr-1.5 text-xs"></i> Back to Parties</button></div>`;
        }
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Participants</label>
                    <input class="input-field" id="wpp-search" value="${utils.esc(state.search)}" placeholder="Search by participant name...">
                </div>
                <button class="btn btn-primary btn-sm" id="wpp-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-users', 'No participants found'); }
        else {
            const rows = state.data.items.map(p => {
                let statusBadge = '';
                if (p.leftAt) statusBadge = '<span class="badge badge-muted">Left</span>';
                else statusBadge = p.isOnline ? '<span class="badge badge-success">Online</span>' : '<span class="badge badge-warning">Offline</span>';

                return `<tr>
                    <td class="text-muted font-mono">#${p.partyId}/${p.profileId}</td>
                    <td class="font-medium">${utils.esc(p.profileName || '—')}</td>
                    <td class="text-muted text-sm">Party #${p.partyId}</td>
                    <td>${statusBadge}</td>
                    <td class="text-muted text-sm">${p.leftAt ? utils.formatDateShort(p.leftAt) : '—'}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(p.joinedAt)}</td>
                    <td class="text-right">
                        <button class="btn btn-ghost btn-sm text-vs-danger del-wpp-btn" data-party-id="${p.partyId}" data-profile-id="${p.profileId}" title="Kick Participant"><i class="fas fa-user-slash text-xs"></i></button>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [{ key: 'profileId', label: 'Party/Profile ID' }, { key: 'profileName', label: 'Participant' }, { key: 'partyId', label: 'Party ID' }, { key: 'isOnline', label: 'Status' }, { key: 'leftAt', label: 'Left At' }, { key: 'joinedAt', label: 'Joined' }, { key: '', label: '' }],
                rows, 'No participants found',
                { tableId: 'wpp-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('wpp-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('wpp-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-wpp-btn').forEach(b => b.addEventListener('click', () => removeParticipant(parseInt(b.dataset.partyId), parseInt(b.dataset.profileId))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init(params) {
            state.partyId = params?.partyId ? parseInt(params.partyId) : null;
            loadData();
        }
    };
})();
