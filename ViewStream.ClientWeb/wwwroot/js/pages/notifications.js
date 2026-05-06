pages.notifications = (() => {
    let state = { notifications: [], loading: true };

    async function load() {
        state.loading = true;
        render();
        try {
            state.notifications = await api.get('/users/me/notifications', { limit: 50 });
        } catch (err) {
            toast.error('Failed to load notifications: ' + err.message);
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('notifications-content');
        if (!c) return;

        let h = Comp.pageHeader('Notifications', '',
            state.notifications.some(n => !n.isRead) ? '<button id="mark-all-read-btn" class="btn btn-sm btn-secondary"><i class="fas fa-check-double"></i> Mark All Read</button>' : '');

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.notifications.length) {
            h += Comp.emptyState('fa-bell', 'No notifications');
        } else {
            h += '<div class="space-y-2">';
            const typeIcons = {
                new_episode: 'fa-film',
                friend_request: 'fa-user-plus',
                subscription: 'fa-crown',
                recommendation: 'fa-magic'
            };
            state.notifications.forEach(n => {
                h += `<div class="flex items-start gap-4 p-4 rounded-xl ${n.isRead ? 'bg-vs-surface border border-vs-border' : 'bg-vs-accent/5 border border-vs-accent/20'} transition-colors" data-notif-id="${n.id}">
                    <div class="w-10 h-10 rounded-full ${n.isRead ? 'bg-vs-card' : 'bg-vs-accent/15'} flex items-center justify-center flex-shrink-0">
                        <i class="fas ${typeIcons[n.notificationType] || 'fa-bell'} ${n.isRead ? 'text-vs-muted' : 'text-vs-accent'} text-sm"></i>
                    </div>
                    <div class="flex-1 min-w-0">
                        <div class="flex items-start justify-between gap-2">
                            <div>
                                <p class="text-sm font-medium ${n.isRead ? 'text-vs-dim' : 'text-vs-text'}">${toast.esc(n.title)}</p>
                                <p class="text-xs text-vs-muted mt-0.5">${toast.esc(n.body)}</p>
                            </div>
                            ${!n.isRead ? '<span class="w-2 h-2 rounded-full bg-vs-accent flex-shrink-0 mt-1.5"></span>' : ''}
                        </div>
                        <p class="text-xs text-vs-muted mt-1">${utils.formatDate(n.createdAt)}</p>
                    </div>
                    ${!n.isRead ? `<button class="p-1 text-vs-muted hover:text-vs-text flex-shrink-0 mark-read-btn" data-id="${n.id}" aria-label="Mark as read"><i class="fas fa-check text-xs"></i></button>` : ''}
                    <button class="p-1 text-vs-muted hover:text-vs-error flex-shrink-0 delete-notif-btn" data-id="${n.id}" aria-label="Delete"><i class="fas fa-times text-xs"></i></button>
                </div>`;
            });
            h += '</div>';
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="notifications-content">' + Comp.pageLoader() + '</div>'; },
        init() {
            load();
            const container = document.getElementById('notifications-content');
            container?.addEventListener('click', e => {
                const btn = e.target.closest('button');
                if (!btn) return;

                // Mark as read
                if (btn.classList.contains('mark-read-btn')) {
                    const id = btn.dataset.id;
                    api.post(`/users/me/notifications/${id}/read`).then(() => {
                        const notif = state.notifications.find(n => n.id == id);
                        if (notif) notif.isRead = true;
                        render();
                    }).catch(err => toast.error(err.message));
                }

                // Delete
                if (btn.classList.contains('delete-notif-btn')) {
                    const id = btn.dataset.id;
                    api.del(`/users/me/notifications/${id}`).then(() => {
                        state.notifications = state.notifications.filter(n => n.id != id);
                        render();
                        toast.success('Notification deleted');
                    }).catch(err => toast.error(err.message));
                }

                // Mark all read
                if (btn.id === 'mark-all-read-btn') {
                    api.post('/users/me/notifications/read-all').then(() => {
                        state.notifications.forEach(n => n.isRead = true);
                        render();
                        toast.success('All marked as read');
                    }).catch(err => toast.error(err.message));
                }
            });
        }
    };
})();