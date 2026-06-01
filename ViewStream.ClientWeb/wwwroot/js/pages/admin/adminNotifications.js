pages.adminNotifications = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc', showCreate: false };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/notifications', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load notifications: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    async function createNotification(payload) {
        try {
            await api.post('/admin/notifications', payload);
            toast.success('Notification sent successfully');
            modal.close();
            loadData();
        } catch (err) { toast.error('Failed to send notification: ' + err.message); }
    }

    async function deleteNotification(id) {
        if (!await modal.confirm('Delete Notification', 'Remove this notification? This cannot be undone.', 'danger')) return;
        try { await api.delete(`/admin/notifications/${id}`); toast.success('Notification deleted'); loadData(); }
        catch (err) { toast.error('Failed to delete: ' + err.message); }
    }

    function openCreateModal() {
        modal.open('Send Notification',
            `<div class="space-y-4">
                <div class="form-group"><label class="form-label">Title <span class="text-vs-danger">*</span></label><input class="input-field" id="notif-title" placeholder="Notification title"></div>
                <div class="form-group"><label class="form-label">Message <span class="text-vs-danger">*</span></label><textarea class="input-field" id="notif-body" rows="3" placeholder="Notification message body..."></textarea></div>
                <div class="form-group"><label class="form-label">Type</label><select class="input-field" id="notif-type">
                    <option value="General">General</option>
                    <option value="NewContent">New Content</option>
                    <option value="Subscription">Subscription</option>
                    <option value="Social">Social</option>
                    <option value="System">System</option>
                </select></div>
                <div class="form-group"><label class="form-label">Send To (User ID, blank = broadcast)</label><input class="input-field" id="notif-uid" placeholder="User ID or leave empty for all users"></div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="notif-send-btn"><i class="fas fa-paper-plane mr-1.5"></i> Send</button>`
        );
        document.getElementById('notif-send-btn')?.addEventListener('click', () => {
            const title = document.getElementById('notif-title').value.trim();
            const body = document.getElementById('notif-body').value.trim();
            const type = document.getElementById('notif-type').value;
            const userId = document.getElementById('notif-uid').value.trim() || null;
            if (!title || !body) { toast.warn('Title and message are required'); return; }
            createNotification({ title, body, type, userId });
        });
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Notifications', 'Manage and broadcast system notifications',
            `<button class="btn btn-primary btn-sm" id="notif-create-btn"><i class="fas fa-paper-plane mr-1.5"></i> Send Notification</button>`);
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Notifications</label>
                    <input class="input-field" id="notif-search" value="${utils.esc(state.search)}" placeholder="Search by title...">
                </div>
                <button class="btn btn-primary btn-sm" id="notif-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-bell', 'No notifications found'); }
        else {
            const rows = state.data.items.map(n => `<tr>
                <td class="font-medium">${utils.esc(n.title || '—')}</td>
                <td class="text-muted text-sm max-w-xs truncate">${utils.esc((n.body || n.message || '').substring(0, 60))}…</td>
                <td><span class="badge badge-info">${utils.esc(n.type || 'General')}</span></td>
                <td class="text-muted text-sm">${utils.esc(n.userEmail || 'All Users')}</td>
                <td>${n.isRead ? '<span class="badge badge-success">Read</span>' : '<span class="badge badge-warning">Unread</span>'}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(n.createdAt)}</td>
                <td class="text-right">
                    <button class="btn btn-ghost btn-sm text-vs-danger del-notif-btn" data-id="${n.id}" title="Delete"><i class="fas fa-trash text-xs"></i></button>
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'title', label: 'Title' }, { key: 'body', label: 'Message' }, { key: 'type', label: 'Type' }, { key: 'userId', label: 'Recipient' }, { key: 'isRead', label: 'Status' }, { key: 'createdAt', label: 'Sent' }, { key: '', label: '' }],
                rows, 'No notifications',
                { tableId: 'notifs-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('notif-create-btn')?.addEventListener('click', openCreateModal);
        document.getElementById('notif-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('notif-search').value.trim(); state.page = 1; loadData(); });
        document.querySelectorAll('.del-notif-btn').forEach(b => b.addEventListener('click', () => deleteNotification(parseInt(b.dataset.id))));
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
