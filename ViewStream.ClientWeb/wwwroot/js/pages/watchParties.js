pages.watchParties = (() => {
    let state = { page: 1, data: null, loading: true };

    function getActiveProfileId() {
        const token = localStorage.getItem(CONFIG.TOKEN_KEY);
        if (!token) return null;
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
            const payload = JSON.parse(jsonPayload);
            return parseInt(payload.ProfileId || payload.profileId || '0');
        } catch (e) {
            return null;
        }
    }

    async function load() {
        state.loading = true; render();
        try {
            state.data = await api.get('/watch-parties', {
                page: state.page,
                pageSize: CONFIG.PAGE_SIZE
            });
        } catch (err) {
            toast.error('Failed to load watch parties: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('watch-parties-content');
        if (!c) return;

        let h = Comp.pageHeader('Watch Parties', 'Host or join a watch party',
            '<button id="create-party-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus mr-1"></i> Create Party</button>');

        h += `<div class="mb-8 p-5 rounded-xl bg-vs-surface border border-vs-border font-body">
            <h2 class="font-display font-semibold text-lg text-vs-text mb-3">Join a Party</h2>
            <div class="flex gap-3">
                <input type="text" id="party-code-input" placeholder="Enter party code" class="input-field uppercase" style="flex:1">
                <button id="join-party-btn" class="btn btn-primary">Join</button>
            </div>
        </div>`;

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data?.items?.length) {
            h += Comp.emptyState('fa-users', 'No watch parties found');
        } else {
            const activeProfileId = getActiveProfileId();
            
            h += '<div class="space-y-3 font-body">';
            state.data.items.forEach(p => {
                const isHost = p.hostProfileId === activeProfileId;
                
                h += `<div class="p-5 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors" data-party-id="${p.id}">
                    <div class="flex items-start justify-between gap-4">
                        <div>
                            <div class="flex items-center gap-2 mb-1">
                                ${p.isActive ? '<span class="w-2 h-2 rounded-full bg-vs-success pulse-dot"></span>' : ''}
                                <h3 class="font-display font-bold text-vs-text">${toast.esc(p.showTitle)}</h3>
                                ${utils.statusBadge(p.isActive ? 'active' : 'ended')}
                            </div>
                            <p class="text-sm text-vs-dim">${toast.esc(p.episodeTitle)}</p>
                            <div class="flex items-center gap-4 mt-2 text-xs text-vs-dim">
                                <span><i class="fas fa-user mr-1"></i>Host: ${toast.esc(p.hostProfileName)}</span>
                                <span><i class="fas fa-users mr-1"></i>${p.participantCount} participants</span>
                                <span><i class="fas fa-key mr-1"></i>Code: <span class="text-vs-accent font-mono font-semibold">${toast.esc(p.partyCode)}</span></span>
                            </div>
                        </div>
                        ${p.isActive ? `<div class="flex gap-2">
                            ${!isHost ? 
                                `<button class="btn btn-sm btn-primary join-btn" data-party-id="${p.id}">Join</button>
                                 <button class="btn btn-sm btn-ghost text-vs-error hover:bg-vs-error/15 leave-btn" data-party-id="${p.id}">Leave</button>` : 
                                `<button class="btn btn-sm btn-danger end-btn" data-party-id="${p.id}">End Party</button>`
                            }
                        </div>` : ''}
                    </div>
                </div>`;
            });
            h += '</div>';
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; load(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('create-party-btn')?.addEventListener('click', () => {
            modal.open('Create Watch Party', 
                `<div class="form-group font-body">
                    <label class="form-label">Episode ID <span class="text-vs-error">*</span></label>
                    <input id="party-episode-id" class="input-field mt-1" type="number" placeholder="e.g. 10101">
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-new-party">Create Party</button>`
            );

            document.getElementById('save-new-party')?.addEventListener('click', async () => {
                const epIdVal = parseInt(document.getElementById('party-episode-id').value);
                if (!epIdVal) { toast.warning('Episode ID is required'); return; }
                try {
                    await api.post('/watch-parties', { episodeId: epIdVal });
                    modal.close();
                    toast.success('Watch party created successfully!');
                    load();
                } catch (err) {
                    toast.error(err.message);
                }
            });
        });

        document.getElementById('join-party-btn')?.addEventListener('click', async () => {
            const code = document.getElementById('party-code-input').value.trim();
            if (!code) { toast.warning('Enter a party code'); return; }
            try {
                const party = await api.get(`/watch-parties/code/${code}`);
                if (party && party.isActive) {
                    await api.post(`/watch-parties/${party.id}/participants/join`);
                    toast.success('Joined party successfully!');
                    load();
                } else {
                    toast.error('Party not found or inactive');
                }
            } catch (err) {
                toast.error('Invalid party code');
            }
        });

        const container = document.getElementById('watch-parties-content');
        container?.addEventListener('click', async e => {
            const btn = e.target.closest('button');
            if (!btn) return;
            const partyId = parseInt(btn.dataset.partyId);

            if (btn.classList.contains('join-btn')) {
                try {
                    await api.post(`/watch-parties/${partyId}/participants/join`);
                    toast.success('Joined watch party');
                    load();
                } catch (err) {
                    toast.error(err.message);
                }
            } else if (btn.classList.contains('leave-btn')) {
                try {
                    await api.post(`/watch-parties/${partyId}/participants/leave`);
                    toast.info('Left watch party');
                    load();
                } catch (err) {
                    toast.error(err.message);
                }
            } else if (btn.classList.contains('end-btn')) {
                try {
                    await api.post(`/watch-parties/${partyId}/end`);
                    toast.success('Party ended');
                    load();
                } catch (err) {
                    toast.error(err.message);
                }
            }
        });
    }

    return {
        render() { return '<div id="watch-parties-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();