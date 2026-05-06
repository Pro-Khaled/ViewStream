pages.watchParties = (() => {
    let state = { parties: [], loading: true };

    async function load() {
        state.loading = true;
        render();
        try {
            state.parties = await api.get('/watch-parties'); // endpoint: GET /watch-parties or /admin/playback/events? maybe only active ones
        } catch (err) {
            toast.error('Failed to load watch parties: ' + err.message);
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('watch-parties-content');
        if (!c) return;

        let h = Comp.pageHeader('Watch Parties', 'Host or join a watch party',
            '<button id="create-party-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus mr-1"></i> Create Party</button>');

        h += `<div class="mb-8 p-5 rounded-xl bg-vs-surface border border-vs-border">
            <h2 class="font-display font-semibold text-lg text-vs-text mb-3">Join a Party</h2>
            <div class="flex gap-3">
                <input type="text" id="party-code-input" placeholder="Enter party code" class="input-field uppercase" style="flex:1">
                <button id="join-party-btn" class="btn btn-primary">Join</button>
            </div>
        </div>`;

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.parties.length) {
            h += Comp.emptyState('fa-users', 'No watch parties found');
        } else {
            h += '<div class="space-y-3">';
            state.parties.forEach(p => {
                const isHost = p.hostProfileId === store.getUser()?.id;
                h += `<div class="p-5 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors">
                    <div class="flex items-start justify-between gap-4">
                        <div>
                            <div class="flex items-center gap-2 mb-1">
                                ${p.isActive ? '<span class="w-2 h-2 rounded-full bg-vs-success pulse-dot"></span>' : ''}
                                <h3 class="font-display font-semibold text-vs-text">${toast.esc(p.showTitle)}</h3>
                                ${utils.statusBadge(p.isActive ? 'active' : 'ended')}
                            </div>
                            <p class="text-sm text-vs-dim">${toast.esc(p.episodeTitle)}</p>
                            <div class="flex items-center gap-4 mt-2 text-xs text-vs-muted">
                                <span><i class="fas fa-user mr-1"></i>Host: ${toast.esc(p.hostProfileName)}</span>
                                <span><i class="fas fa-users mr-1"></i>${p.participantCount} participants</span>
                                <span><i class="fas fa-key mr-1"></i>Code: <span class="text-vs-accent font-mono">${toast.esc(p.partyCode)}</span></span>
                            </div>
                        </div>
                        ${p.isActive ? `<div class="flex gap-2">
                            ${!isHost ? `<button class="btn btn-sm btn-primary join-btn" data-party-id="${p.id}">Join</button>` : `<button class="btn btn-sm btn-danger end-btn" data-party-id="${p.id}">End Party</button>`}
                        </div>` : ''}
                    </div>
                </div>`;
            });
            h += '</div>';
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="watch-parties-content">' + Comp.pageLoader() + '</div>'; },
        init() {
            load();

            const container = document.getElementById('watch-parties-content');
            container?.addEventListener('click', e => {
                const btn = e.target.closest('button');
                if (!btn) return;

                if (btn.id === 'create-party-btn') {
                    // Simple create modal
                    modal.open('Create Watch Party', `<div class="form-group"><label class="form-label">Episode ID</label><input id="party-episode-id" class="input-field" type="number" placeholder="e.g. 10101"></div>`,
                        `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                         <button class="btn btn-primary" onclick="pages.watchParties.createParty()">Create</button>`);
                }

                if (btn.classList.contains('join-btn')) {
                    api.post(`/watch-parties/${btn.dataset.partyId}/participants/join`).then(() => {
                        toast.success('Joined watch party');
                        load();
                    }).catch(err => toast.error(err.message));
                }

                if (btn.classList.contains('end-btn')) {
                    api.post(`/watch-parties/${btn.dataset.partyId}/end`).then(() => {
                        toast.success('Party ended');
                        load();
                    }).catch(err => toast.error(err.message));
                }

                if (btn.id === 'join-party-btn') {
                    const code = document.getElementById('party-code-input').value.trim();
                    if (!code) { toast.warning('Enter a party code'); return; }
                    api.get(`/watch-parties/code/${code}`).then(async (party) => {
                        if (party && party.isActive) {
                            await api.post(`/watch-parties/${party.id}/participants/join`);
                            toast.success('Joined party');
                            load();
                        } else {
                            toast.error('Party not found or inactive');
                        }
                    }).catch(err => toast.error('Invalid party code'));
                }
            });
        },
        createParty: async function () {
            const episodeId = parseInt(document.getElementById('party-episode-id')?.value);
            if (!episodeId) { toast.warning('Enter a valid episode ID'); return; }
            try {
                await api.post('/watch-parties', { episodeId });
                modal.close();
                toast.success('Watch party created');
                load();
            } catch (err) { toast.error(err.message); }
        }
    };
})();