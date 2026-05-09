pages.friends = (() => {
    let state = { friends: [], pending: [], loading: true };

    async function load() {
        state.loading = true; render();
        try {
            const [friendsRes, pendingRes] = await Promise.all([
                api.get('/friends'),
                api.get('/friends/pending?direction=received')
            ]);
            state.friends = friendsRes || [];
            state.pending = pendingRes || [];
        } catch (err) { toast.error('Failed to load friends'); }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('friends-content');
        if (!c) return;
        let h = Comp.pageHeader('Friends', '', '<button id="add-friend-btn" class="btn btn-primary btn-sm"><i class="fas fa-user-plus mr-1"></i> Add Friend</button>');
        if (state.loading) { h += Comp.pageLoader(); }
        else {
            if (state.pending.length) {
                h += `<section class="mb-8">
                    <h2 class="font-display font-semibold text-lg text-vs-text mb-3">Pending Requests (${state.pending.length})</h2>
                    <div class="space-y-2">${state.pending.map(f => `<div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-accent/20">
                        <div class="flex items-center gap-3">
                            <div class="w-10 h-10 rounded-full bg-vs-card flex items-center justify-center text-lg">${toast.esc(f.friendFullName?.[0] || '?')}</div>
                            <div><p class="text-sm font-medium text-vs-text">${toast.esc(f.friendFullName)}</p><p class="text-xs text-vs-muted">${toast.esc(f.friendName)}</p></div>
                        </div>
                        <div class="flex gap-2">
                            <button class="px-3 py-1.5 bg-vs-success/15 text-vs-success text-sm font-medium rounded-lg hover:bg-vs-success/25 accept-btn" data-friend-id="${f.friendId}">Accept</button>
                            <button class="px-3 py-1.5 bg-vs-card text-vs-dim text-sm rounded-lg hover:bg-vs-elevated border border-vs-border decline-btn" data-friend-id="${f.friendId}">Decline</button>
                        </div>
                    </div>`).join('')}</div></section>`;
            }
            h += `<section><h2 class="font-display font-semibold text-lg text-vs-text mb-3">All Friends (${state.friends.length})</h2>
                ${state.friends.length ? `<div class="space-y-2">${state.friends.map(f => `<div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors">
                    <div class="flex items-center gap-3">
                        <div class="w-10 h-10 rounded-full bg-vs-card flex items-center justify-center text-lg">${toast.esc(f.friendFullName?.[0] || '?')}</div>
                        <div><p class="text-sm font-medium text-vs-text">${toast.esc(f.friendFullName || f.friendName)}</p><p class="text-xs text-vs-muted">${toast.esc(f.friendName)}</p></div>
                    </div>
                    <div class="flex items-center gap-2">
                        ${utils.statusBadge(f.status)}
                        <button class="p-2 text-vs-muted hover:text-vs-error unfriend-btn" data-friend-id="${f.friendId}"><i class="fas fa-user-minus text-sm"></i></button>
                    </div>
                </div>`).join('')}</div>` : '<div class="empty-state"><i class="fas fa-users text-3xl text-vs-muted mb-3"></i><p>No friends yet</p></div>'}
            </section>`;
        }
        c.innerHTML = h;
    }

    function showSearchFriendModal() {
        modal.open('Add Friend', `<div class="relative"><input id="friend-search-input" class="input-field" placeholder="Search by email or name..."></div><div id="friend-search-results" class="space-y-2 mt-4"></div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>`);
        setTimeout(() => {
            const input = document.getElementById('friend-search-input');
            input.addEventListener('input', async () => {
                const q = input.value.trim().toLowerCase();
                const resContainer = document.getElementById('friend-search-results');
                if (!q || q.length < 2) { resContainer.innerHTML = ''; return; }
                try {
                    const users = await api.get('/users/search', { q, limit: 10 });
                    resContainer.innerHTML = users.length ? users.map(u => `<div class="flex items-center justify-between p-3 rounded-lg bg-vs-card">
                        <div><p class="text-sm text-vs-text">${toast.esc(u.fullName || u.email)}</p><p class="text-xs text-vs-muted">${toast.esc(u.email)}</p></div>
                        <button class="px-3 py-1 bg-vs-accent/15 text-vs-accent text-xs font-medium rounded-lg hover:bg-vs-accent/25" onclick="pages.friends.sendRequest(${u.id})">Send Request</button>
                    </div>`).join('') : '<p class="text-sm text-vs-muted">No users found</p>';
                } catch (err) { resContainer.innerHTML = '<p class="text-sm text-vs-error">Search failed</p>'; }
            });
        }, 100);
    }

    return {
        render() { return '<div id="friends-content">' + Comp.pageLoader() + '</div>'; },
        init() {
            load();
            // Add friend modal
            document.getElementById('add-friend-btn')?.addEventListener('click', showSearchFriendModal);
            // Accept / decline / unfriend
            const container = document.getElementById('friends-content');
            container?.addEventListener('click', e => {
                const btn = e.target.closest('button');
                if (!btn) return;
                const friendId = btn.dataset.friendId;
                if (btn.classList.contains('accept-btn')) {
                    api.put(`/friends/request/${friendId}`, { status: 'accepted' }).then(() => { toast.success('Request accepted'); load(); }).catch(err => toast.error(err.message));
                } else if (btn.classList.contains('decline-btn')) {
                    api.put(`/friends/request/${friendId}`, { status: 'declined' }).then(() => { toast.info('Request declined'); load(); }).catch(err => toast.error(err.message));
                } else if (btn.classList.contains('unfriend-btn')) {
                    modal.open('Remove Friend', '<p>Remove this friend?</p>',
                        `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                         <button class="btn btn-danger" onclick="pages.friends.removeFriend(${friendId})">Remove</button>`);
                }
            });
        },
        sendRequest: async function (friendId) {
            await api.post('/friends/request', { friendId });
            toast.success('Friend request sent');
            modal.close();
            load();
        },
        removeFriend: async function (friendId) {
            await api.del(`/friends/${friendId}`);
            toast.info('Friend removed');
            modal.close();
            load();
        }
    };
})();