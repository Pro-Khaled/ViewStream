pages.adminLoginSessions = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/loginsessions', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load sessions: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function revokeSession(id) {
        if (!await modal.confirm('Revoke Session', 'This will force the user to log in again. Proceed?', 'danger')) return;
        try {
            await api.delete(`/admin/loginsessions/${id}`);
            toast.success('Session revoked');
            loadData();
        } catch (err) { toast.error('Failed to revoke session: ' + err.message); }
    }

    async function revokeAll(userId) {
        if (!await modal.confirm('Revoke All Sessions', 'This will log out this user from all devices. Proceed?', 'danger')) return;
        try {
            await api.delete(`/admin/loginsessions/user/${userId}`);
            toast.success('All sessions revoked');
            loadData();
        } catch (err) { toast.error('Failed to revoke sessions: ' + err.message); }
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Login Sessions', 'Monitor active user sessions across all devices');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Sessions</label>
                    <input class="input-field" id="ls-search" value="${utils.esc(state.search)}" placeholder="Search by user email or IP...">
                </div>
                <button class="btn btn-primary btn-sm" id="ls-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-shield-alt', 'No active sessions found'); }
        else {
            const rows = state.data.items.map(s => `<tr>
                <td class="text-muted font-mono text-xs">${s.id}</td>
                <td><span class="font-medium">${utils.esc(s.userEmail || '—')}</span></td>
                <td class="text-muted text-sm">${utils.esc(s.ipAddress || '—')}</td>
                <td class="text-muted text-sm max-w-xs truncate" title="${utils.esc(s.userAgent || '')}">${utils.esc((s.userAgent || '').substring(0, 40))}${s.userAgent?.length > 40 ? '…' : ''}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(s.createdAt)}</td>
                <td class="text-muted text-sm">${s.expiresAt ? utils.formatDateShort(s.expiresAt) : '—'}</td>
                <td>${s.isActive ? '<span class="badge badge-success">Active</span>' : '<span class="badge badge-muted">Expired</span>'}</td>
                <td class="text-right">
                    <div class="flex items-center justify-end gap-1.5">
                        <button class="btn btn-ghost btn-sm text-vs-danger revoke-btn" data-id="${s.id}" title="Revoke Session"><i class="fas fa-ban text-xs"></i></button>
                        <button class="btn btn-ghost btn-sm text-vs-warning revoke-all-btn" data-uid="${s.userId}" title="Revoke All for User"><i class="fas fa-user-slash text-xs"></i></button>
                    </div>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'id', label: 'Session ID' }, { key: 'userEmail', label: 'User' }, { key: 'ipAddress', label: 'IP' }, { key: 'userAgent', label: 'Device' }, { key: 'createdAt', label: 'Created' }, { key: 'expiresAt', label: 'Expires' }, { key: 'isActive', label: 'Status' }, { key: '', label: '' }],
                rows, 'No sessions found',
                { tableId: 'sessions-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('ls-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('ls-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.revoke-btn').forEach(b => b.addEventListener('click', () => revokeSession(b.dataset.id)));
        document.querySelectorAll('.revoke-all-btn').forEach(b => b.addEventListener('click', () => revokeAll(b.dataset.uid)));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
