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
        } catch (err) {
            toast.error('Failed to load friends: ' + err.message);
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('friends-content');
        if (!c) return;
        
        let h = Comp.pageHeader('Friends', '', '<button id="add-friend-btn" class="btn btn-primary btn-sm"><i class="fas fa-user-plus mr-1"></i> Add Friend</button>');
        
        if (state.loading) {
            h += Comp.pageLoader();
        } else {
            if (state.pending.length) {
                h += `<section class="mb-8 font-body">
                    <h2 class="font-display font-bold text-lg text-vs-text mb-3">Pending Requests (${state.pending.length})</h2>
                    <div class="space-y-2">${state.pending.map(f => `<div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-accent/20">
                        <div class="flex items-center gap-3">
                            <div class="w-10 h-10 rounded-full bg-vs-card flex items-center justify-center text-lg font-bold border border-vs-border">${toast.esc(f.friendFullName?.[0] || f.friendName?.[0] || '?')}</div>
                            <div>
                                <p class="text-sm font-semibold text-vs-text">${toast.esc(f.friendFullName || f.friendName)}</p>
                                <p class="text-xs text-vs-dim">${toast.esc(f.friendName)}</p>
                            </div>
                        </div>
                        <div class="flex gap-2">
                            <button class="px-3 py-1.5 bg-vs-success/15 hover:bg-vs-success/25 text-vs-success text-sm font-semibold rounded-lg accept-btn" data-friend-id="${f.friendId}">Accept</button>
                            <button class="px-3 py-1.5 bg-vs-card hover:bg-vs-elevated border border-vs-border text-vs-dim text-sm rounded-lg decline-btn" data-friend-id="${f.friendId}">Decline</button>
                        </div>
                    </div>`).join('')}</div>
                </section>`;
            }
            
            h += `<section class="font-body">
                <h2 class="font-display font-bold text-lg text-vs-text mb-3">All Friends (${state.friends.length})</h2>
                ${state.friends.length ? 
                    `<div class="space-y-2">${state.friends.map(f => `<div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors">
                        <div class="flex items-center gap-3">
                            <div class="w-10 h-10 rounded-full bg-vs-card flex items-center justify-center text-lg font-bold border border-vs-border">${toast.esc(f.friendFullName?.[0] || f.friendName?.[0] || '?')}</div>
                            <div>
                                <p class="text-sm font-semibold text-vs-text">${toast.esc(f.friendFullName || f.friendName)}</p>
                                <p class="text-xs text-vs-dim">${toast.esc(f.friendName)}</p>
                            </div>
                        </div>
                        <div class="flex items-center gap-2">
                            ${utils.statusBadge(f.status || 'accepted')}
                            <button class="p-2 text-vs-muted hover:text-vs-error unfriend-btn" data-friend-id="${f.friendId}" title="Remove Friend"><i class="fas fa-user-minus text-sm"></i></button>
                        </div>
                    </div>`).join('')}</div>` : 
                    `<div class="text-center text-vs-muted py-12 bg-vs-surface border border-vs-border rounded-xl">
                        <i class="fas fa-users text-3xl mb-3 block text-vs-muted/50"></i>
                        <p class="text-sm">No friends yet. Add some friends to stream together!</p>
                    </div>`
                }
            </section>`;
        }
        c.innerHTML = h;
        bindEvents();
    }

    function showSearchFriendModal() {
        modal.open('Add Friend', 
            `<div class="space-y-4 font-body">
                <div class="form-group mb-0">
                    <label class="form-label">Search Users</label>
                    <input id="friend-search-input" class="input-field mt-1" placeholder="Search by email or name...">
                </div>
                <div id="friend-search-results" class="space-y-2 max-h-60 overflow-y-auto"></div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>`
        );

        const input = document.getElementById('friend-search-input');
        input?.addEventListener('input', async () => {
            const q = input.value.trim().toLowerCase();
            const resContainer = document.getElementById('friend-search-results');
            if (!q || q.length < 2) { resContainer.innerHTML = ''; return; }
            try {
                const users = await api.get('/users/search', { q, limit: 10 });
                resContainer.innerHTML = users.length ? users.map(u => `<div class="flex items-center justify-between p-3 rounded-lg bg-vs-surface border border-vs-border">
                    <div>
                        <p class="text-sm font-semibold text-vs-text">${toast.esc(u.fullName || u.email)}</p>
                        <p class="text-xs text-vs-dim">${toast.esc(u.email)}</p>
                    </div>
                    <button class="px-3 py-1 bg-vs-accent/15 text-vs-accent text-xs font-semibold rounded-lg hover:bg-vs-accent/25 send-request-btn" data-user-id="${u.id}">Send Request</button>
                </div>`).join('') : '<p class="text-sm text-vs-muted p-2">No users found</p>';

                // Bind send request buttons inside the modal search results
                resContainer.querySelectorAll('.send-request-btn').forEach(btn => {
                    btn.addEventListener('click', async () => {
                        const targetId = parseInt(btn.dataset.userId);
                        try {
                            await api.post('/friends/request', { friendId: targetId });
                            toast.success('Friend request sent!');
                            modal.close();
                            load();
                        } catch (err) {
                            toast.error(err.message);
                        }
                    });
                });
            } catch (err) { 
                resContainer.innerHTML = '<p class="text-sm text-vs-error p-2">Search failed</p>'; 
            }
        });
    }

    function bindEvents() {
        document.getElementById('add-friend-btn')?.addEventListener('click', showSearchFriendModal);

        const container = document.getElementById('friends-content');
        container?.addEventListener('click', async e => {
            const btn = e.target.closest('button');
            if (!btn) return;
            const friendId = btn.dataset.friendId;

            if (btn.classList.contains('accept-btn')) {
                try {
                    await api.put(`/friends/request/${friendId}`, { status: 'accepted' });
                    toast.success('Request accepted');
                    load();
                } catch (err) {
                    toast.error(err.message);
                }
            } else if (btn.classList.contains('decline-btn')) {
                try {
                    // decline is handled by deleting the relationship connection via DELETE
                    await api.del(`/friends/${friendId}`);
                    toast.info('Request declined');
                    load();
                } catch (err) {
                    toast.error(err.message);
                }
            } else if (btn.classList.contains('unfriend-btn')) {
                modal.open('Remove Friend', 
                    `<p class="text-sm text-vs-dim">Are you sure you want to remove this friend? You will no longer be connected.</p>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-danger" id="confirm-unfriend">Remove Friend</button>`
                );
                document.getElementById('confirm-unfriend')?.addEventListener('click', async () => {
                    try {
                        await api.del(`/friends/${friendId}`);
                        toast.info('Friend removed');
                        modal.close();
                        load();
                    } catch (err) {
                        toast.error(err.message);
                    }
                });
            }
        });
    }

    return {
        render() { return '<div id="friends-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();