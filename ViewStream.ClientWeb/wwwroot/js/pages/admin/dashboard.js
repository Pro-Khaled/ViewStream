pages.adminDashboard = (() => {
    let state = { stats: null, loading: true };
    async function load() {
        state.loading = true; render();
        try {
            const [shows, searchLogs, auditLogs, commentReports, contentReports] = await Promise.all([
                api.get('/shows', { page: 1, pageSize: 1 }),
                api.get('/admin/search/logs', { page: 1, pageSize: 5 }),
                api.get('/admin/audit/logs', { page: 1, pageSize: 5 }),
                api.get('/admin/reports/comments', { page: 1, pageSize: 1 }),
                api.get('/admin/reports/content', { page: 1, pageSize: 1 })
            ]);
            state.stats = {
                totalUsers: 12847, // replace with real endpoint if available
                activeSubscriptions: 9521,
                contentItems: shows.totalCount,
                reportsPending: (commentReports.items.filter(r => r.status === 'Pending').length +
                    contentReports.items.filter(r => r.status === 'Pending').length),
                searchLogs: searchLogs.items,
                auditLogs: auditLogs.items
            };
        } catch (err) { toast.error('Failed to load dashboard'); }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-dashboard-content');
        if (!c) return;
        let h = Comp.pageHeader('Admin Dashboard');
        if (state.loading) { h += Comp.pageLoader(); }
        else if (state.stats) {
            const s = state.stats;
            h += `<div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4 mb-8">
                ${Comp.statCard(s.totalUsers, 'Total Users', 'fa-users', 'text-vs-accent')}
                ${Comp.statCard(s.activeSubscriptions, 'Active Subscriptions', 'fa-crown', 'text-vs-success')}
                ${Comp.statCard(s.contentItems, 'Content Items', 'fa-film', 'text-vs-info')}
                ${Comp.statCard(s.reportsPending, 'Reports Pending', 'fa-flag', 'text-vs-warning')}
            </div>
            <div class="grid grid-cols-1 xl:grid-cols-2 gap-6">
                <div class="bg-vs-surface border border-vs-border rounded-xl overflow-hidden">
                    <div class="px-5 py-4 border-b border-vs-border"><h2 class="font-display font-semibold text-vs-text">Recent Searches</h2></div>
                    ${Comp.dataTable(
                ['User', 'Query', 'Results', 'Clicked', 'Time'],
                s.searchLogs.map(l => `<tr class="table-row border-b border-vs-border">
                            <td class="px-4 py-2.5 text-sm text-vs-dim">${toast.esc(l.profileName)}</td>
                            <td class="px-4 py-2.5 text-sm text-vs-text">${toast.esc(l.query)}</td>
                            <td class="px-4 py-2.5 text-sm text-vs-dim">${l.resultsCount ?? '—'}</td>
                            <td class="px-4 py-2.5 text-sm text-vs-accent">${toast.esc(l.clickedShowTitle || '—')}</td>
                            <td class="px-4 py-2.5 text-xs text-vs-muted">${utils.formatDate(l.searchAt)}</td>
                        </tr>`)
            )}
                </div>
                <div class="bg-vs-surface border border-vs-border rounded-xl overflow-hidden">
                    <div class="px-5 py-4 border-b border-vs-border"><h2 class="font-display font-semibold text-vs-text">Recent Audit Activity</h2></div>
                    ${Comp.dataTable(
                ['Table', 'Action', 'By', 'Time'],
                s.auditLogs.map(a => `<tr class="table-row border-b border-vs-border">
                            <td class="px-4 py-2.5 text-sm text-vs-dim">${toast.esc(a.tableName)}</td>
                            <td class="px-4 py-2.5"><span class="badge badge-${a.action === 'Create' ? 'success' : a.action === 'Delete' ? 'danger' : 'info'}">${toast.esc(a.action)}</span></td>
                            <td class="px-4 py-2.5 text-sm text-vs-dim">${toast.esc(a.changedByUserName)}</td>
                            <td class="px-4 py-2.5 text-xs text-vs-muted">${utils.formatDate(a.changedAt)}</td>
                        </tr>`)
            )}
                </div>
            </div>
            <div class="mt-6 flex flex-wrap gap-3">
                <a href="#/admin/users" class="btn btn-primary btn-sm"><i class="fas fa-user-plus"></i> Manage Users</a>
                <a href="#/admin/content" class="btn btn-secondary btn-sm"><i class="fas fa-film"></i> Content Management</a>
                <a href="#/admin/moderation" class="btn btn-secondary btn-sm"><i class="fas fa-shield-alt"></i> Moderation</a>
                <a href="#/admin/audit" class="btn btn-secondary btn-sm"><i class="fas fa-clipboard-list"></i> Audit Logs</a>
                <a href="#/admin/errors" class="btn btn-secondary btn-sm"><i class="fas fa-bug"></i> Error Logs</a>
                <a href="#/admin/promos" class="btn btn-secondary btn-sm"><i class="fas fa-tag"></i> Promo Codes</a>
                <a href="#/admin/roles" class="btn btn-secondary btn-sm"><i class="fas fa-key"></i> Roles & Permissions</a>
            </div>`;
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="admin-dashboard-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();