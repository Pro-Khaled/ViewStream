pages.notifications = (() => {
    let state = { notifications: [], unreadOnly: false, loading: true };

    async function load() {
        state.loading = true; render();
        try {
            state.notifications = await api.get('/users/me/notifications', { 
                limit: 50,
                unreadOnly: state.unreadOnly
            });
        } catch (err) {
            toast.error('Failed to load notifications: ' + err.message);
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('notifications-content');
        if (!c) return;

        const hasUnread = state.notifications.some(n => !n.isRead);
        let h = Comp.pageHeader('Notifications', '',
            hasUnread ? '<button id="mark-all-read-btn" class="btn btn-sm btn-secondary"><i class="fas fa-check-double mr-1.5"></i>Mark All Read</button>' : '');

        // Unread only toggle bar
        h += `<div class="mb-6 flex justify-between items-center font-body bg-vs-surface border border-vs-border p-4 rounded-xl">
            <div class="flex items-center gap-2">
                <input type="checkbox" id="unread-only-toggle" ${state.unreadOnly ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent cursor-pointer">
                <label for="unread-only-toggle" class="text-sm text-vs-dim cursor-pointer select-none font-medium">Unread Only</label>
            </div>
            <div class="text-xs text-vs-muted">Showing latest 50 notifications</div>
        </div>`;

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.notifications.length) {
            h += Comp.emptyState('fa-bell', state.unreadOnly ? 'No unread notifications' : 'No notifications');
        } else {
            h += '<div class="space-y-2 font-body">';
            const typeIcons = {
                new_episode: 'fa-film',
                friend_request: 'fa-user-plus',
                subscription: 'fa-crown',
                recommendation: 'fa-magic'
            };
            state.notifications.forEach(n => {
                h += `<div class="flex items-start gap-4 p-4 rounded-xl ${n.isRead ? 'bg-vs-surface border border-vs-border' : 'bg-vs-accent/5 border border-vs-accent/20'} transition-colors" data-notif-id="${n.id}">
                    <div class="w-10 h-10 rounded-full ${n.isRead ? 'bg-vs-card' : 'bg-vs-accent/15'} flex items-center justify-center flex-shrink-0 border border-vs-border/50">
                        <i class="fas ${typeIcons[n.notificationType] || 'fa-bell'} ${n.isRead ? 'text-vs-muted' : 'text-vs-accent'} text-sm"></i>
                    </div>
                    <div class="flex-1 min-w-0">
                        <div class="flex items-start justify-between gap-2">
                            <div>
                                <p class="text-sm font-semibold ${n.isRead ? 'text-vs-dim' : 'text-vs-text'}">${toast.esc(n.title)}</p>
                                <p class="text-xs text-vs-dim mt-0.5">${toast.esc(n.body)}</p>
                            </div>
                            ${!n.isRead ? '<span class="w-2 h-2 rounded-full bg-vs-accent flex-shrink-0 mt-1.5"></span>' : ''}
                        </div>
                        <p class="text-[10px] text-vs-muted mt-1.5">${utils.formatDate(n.createdAt)}</p>
                    </div>
                    <div class="flex items-center gap-1.5 flex-shrink-0">
                        ${!n.isRead ? `<button class="p-1.5 text-vs-muted hover:text-vs-accent mark-read-btn" data-id="${n.id}" title="Mark as read"><i class="fas fa-check text-xs"></i></button>` : ''}
                        <button class="p-1.5 text-vs-muted hover:text-vs-error delete-notif-btn" data-id="${n.id}" title="Delete"><i class="fas fa-times text-xs"></i></button>
                    </div>
                </div>`;
            });
            h += '</div>';
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('unread-only-toggle')?.addEventListener('change', (e) => {
            state.unreadOnly = e.target.checked;
            load();
        });

        document.getElementById('mark-all-read-btn')?.addEventListener('click', () => {
            api.post('/users/me/notifications/read-all').then(() => {
                state.notifications.forEach(n => n.isRead = true);
                toast.success('All marked as read');
                load();
            }).catch(err => toast.error(err.message));
        });

        const container = document.getElementById('notifications-content');
        container?.addEventListener('click', e => {
            const btn = e.target.closest('button');
            if (!btn) return;
            const id = btn.dataset.id;

            if (btn.classList.contains('mark-read-btn')) {
                api.post(`/users/me/notifications/${id}/read`).then(() => {
                    toast.success('Notification marked as read');
                    load();
                }).catch(err => toast.error(err.message));
            } else if (btn.classList.contains('delete-notif-btn')) {
                api.del(`/users/me/notifications/${id}`).then(() => {
                    toast.success('Notification deleted');
                    load();
                }).catch(err => toast.error(err.message));
            }
        });
    }

    return {
        render() { return '<div id="notifications-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();