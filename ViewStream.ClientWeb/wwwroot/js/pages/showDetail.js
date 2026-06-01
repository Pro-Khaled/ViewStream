pages.showDetail = (() => {
    let state = { showId: null, show: null, seasons: null, credits: null, awards: null, rating: null, availabilities: null, myRating: null, loading: true };

    async function load(id) {
        state.showId = id; state.loading = true; render();
        try {
            const [show, seasons, credits, awards, rating, availabilities] = await Promise.all([
                api.get(`/shows/${id}`),
                api.get(`/shows/${id}/seasons`),
                api.get(`/shows/${id}/credits`),
                api.get(`/shows/${id}/awards`),
                api.get(`/shows/${id}/ratings/summary`),
                api.get(`/shows/${id}/availabilities`)
            ]);
            state.show = show;
            state.seasons = seasons;
            state.credits = credits;
            state.awards = awards;
            state.rating = rating;
            state.availabilities = availabilities;

            state.firstEpisodeId = null;
            if (seasons && seasons.length > 0) {
                try {
                    const firstSeasonEps = await api.get(`/seasons/${seasons[0].id}/episodes`);
                    state.firstEpisodeId = firstSeasonEps && firstSeasonEps.length > 0 ? firstSeasonEps[0].id : null;
                } catch (e) {
                    state.firstEpisodeId = null;
                }
            }

            if (store.isAuthenticated()) {
                try {
                    state.myRating = await api.get(`/shows/${id}/ratings/me`);
                } catch (e) {
                    state.myRating = null;
                }
            } else {
                state.myRating = null;
            }
        } catch (err) {
            toast.error('Failed to load show details: ' + err.message);
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('show-detail-content');
        if (!c) return;
        if (state.loading) { c.innerHTML = Comp.pageLoader(); return; }
        if (!state.show) { c.innerHTML = Comp.emptyState('fa-film', 'Show not found'); return; }

        const show = state.show;
        
        let myRatingHtml = '';
        if (store.isAuthenticated()) {
            const rVal = state.myRating ? state.myRating.rating : 0;
            myRatingHtml = `<div class="mt-4 p-4 rounded-xl bg-vs-card/50 border border-vs-border/50">
                <div class="text-sm font-semibold text-vs-text mb-2">Your Rating</div>
                <div class="flex items-center gap-1.5" id="user-star-rating">
                    ${[1, 2, 3, 4, 5].map(star => `<button class="text-xl star-btn transition-colors focus:outline-none ${star <= rVal ? 'text-vs-accent' : 'text-vs-muted hover:text-vs-accent/60'}" data-star="${star}"><i class="fas fa-star"></i></button>`).join('')}
                    ${rVal > 0 ? `<button class="text-xs text-vs-error hover:underline ml-3" id="delete-rating-btn">Remove</button>` : ''}
                </div>
            </div>`;
        }

        let availHtml = '';
        if (state.availabilities && state.availabilities.length > 0) {
            availHtml = `<section class="mt-10">
                <h2 class="font-display font-bold text-xl text-vs-text mb-4">Available Regions</h2>
                <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                    ${state.availabilities.map(av => {
                        const from = av.availableFrom ? new Date(av.availableFrom).toLocaleDateString() : 'Now';
                        const until = av.availableUntil ? new Date(av.availableUntil).toLocaleDateString() : 'Indefinitely';
                        return `<div class="p-4 rounded-xl bg-vs-card border border-vs-border flex items-center gap-3">
                            <div class="w-8 h-8 rounded-lg bg-vs-accent/15 text-vs-accent flex items-center justify-center font-bold text-xs uppercase">${toast.esc(av.countryCode)}</div>
                            <div>
                                <div class="text-sm font-semibold text-vs-text">${toast.esc(av.countryName || av.countryCode)}</div>
                                <div class="text-xs text-vs-dim mt-0.5">${from} â€“ ${until}</div>
                            </div>
                        </div>`;
                    }).join('')}
                </div>
            </section>`;
        }

        let h = `<div class="hero-gradient">
            <div class="relative h-[300px] sm:h-[400px]">
                ${show.backdropUrl ? `<img src="${toast.esc(show.backdropUrl)}" alt="" class="w-full h-full object-cover">` : ''}
                <div class="absolute inset-0 bg-gradient-to-t from-vs-bg via-vs-bg/60 to-transparent"></div>
            </div>
            <div class="px-4 sm:px-6 -mt-32 relative z-10 max-w-5xl">
                <div class="flex flex-col lg:flex-row gap-8">
                    <div class="flex-shrink-0">
                        <img src="${toast.esc(show.posterUrl)}" alt="${toast.esc(show.title)}" class="w-48 lg:w-56 rounded-xl shadow-2xl shadow-black/50">
                    </div>
                    <div class="flex-1 min-w-0">
                        <h1 class="font-display font-black text-3xl sm:text-4xl text-vs-text mb-3">${toast.esc(show.title)}</h1>
                        <div class="flex flex-wrap items-center gap-3 mb-4">
                            ${show.releaseYear ? `<span class="text-sm text-vs-dim">${show.releaseYear}</span>` : ''}
                            ${show.maturityRating ? `<span class="px-2 py-0.5 rounded bg-white/10 text-sm text-vs-dim">${toast.esc(show.maturityRating)}</span>` : ''}
                            ${show.runtimeMinutes ? `<span class="text-sm text-vs-dim">${show.runtimeMinutes} min</span>` : ''}
                            ${show.imdbRating ? `<span class="flex items-center gap-1 text-sm text-vs-accent font-semibold"><i class="fas fa-star text-xs"></i>${show.imdbRating} IMDb</span>` : ''}
                            ${show.rottenTomatoesScore != null ? `<span class="flex items-center gap-1 text-sm text-vs-error font-semibold"><i class="fas fa-apple-whole text-xs"></i>${show.rottenTomatoesScore}% Tomatoes</span>` : ''}
                        </div>
                        <div class="flex flex-wrap gap-2 mb-4">
                            ${(show.genres || []).map(g => `<span class="px-3 py-1 rounded-full bg-vs-accent/10 text-vs-accent text-xs font-medium">${toast.esc(g)}</span>`).join('')}
                        </div>
                        <p class="text-vs-dim leading-relaxed mb-6 max-w-2xl">${toast.esc(show.description)}</p>
                        
                        <div class="flex items-center gap-3 mb-6">
                            ${state.firstEpisodeId ? `<a href="#/episode/${state.firstEpisodeId}" class="inline-flex items-center gap-2 px-6 py-3 bg-vs-accent hover:bg-vs-accentHover text-vs-bg font-bold rounded-xl transition-colors"><i class="fas fa-play text-sm"></i> Play</a>` : ''}
                            <button id="add-to-library-btn" class="inline-flex items-center gap-2 px-5 py-3 bg-vs-card hover:bg-vs-elevated text-vs-text font-semibold rounded-xl border border-vs-border transition-colors"><i class="fas fa-bookmark"></i> My Library</button>
                        </div>

                        ${state.rating ? `<div class="flex items-center gap-4 p-4 rounded-xl bg-vs-card/50 border border-vs-border/50 mb-6">
                            <div class="text-center">
                                <div class="text-3xl font-display font-black text-vs-accent">${state.rating.averageRating.toFixed(1)}</div>
                                <div class="text-xs text-vs-muted mt-1">${state.rating.totalRatings} ratings</div>
                            </div>
                            <div class="flex-1">${[5, 4, 3, 2, 1].map(s => {
                                const count = state.rating.ratingDistribution ? (state.rating.ratingDistribution[s] || 0) : 0;
                                const pct = state.rating.totalRatings ? Math.round((count / state.rating.totalRatings) * 100) : 0;
                                return `<div class="flex items-center gap-2 mb-1">
                                    <span class="text-xs text-vs-muted w-3">${s}</span>
                                    <div class="flex-1 h-2 rounded-full bg-vs-border"><div class="h-full rounded-full bg-vs-accent/60" style="width:${pct}%"></div></div>
                                    <span class="text-xs text-vs-muted w-8 text-right">${count}</span>
                                </div>`;
                            }).join('')}</div>
                        </div>` : ''}

                        ${myRatingHtml}

                        <div class="grid grid-cols-3 gap-4 text-center mt-6">
                            <div class="p-3 rounded-lg bg-vs-card/30"><div class="text-xl font-bold text-vs-text">${show.seasonCount}</div><div class="text-xs text-vs-muted">Seasons</div></div>
                            <div class="p-3 rounded-lg bg-vs-card/30"><div class="text-xl font-bold text-vs-text">${show.episodeCount}</div><div class="text-xs text-vs-muted">Episodes</div></div>
                            <div class="p-3 rounded-lg bg-vs-card/30"><div class="text-xl font-bold text-vs-text">${state.awards?.length || 0}</div><div class="text-xs text-vs-muted">Awards</div></div>
                        </div>
                    </div>
                </div>

                ${availHtml}

                ${state.seasons?.length ? `<section class="mt-10">
                    <h2 class="font-display font-bold text-xl text-vs-text mb-4">Seasons & Episodes</h2>
                    <div class="space-y-2" id="seasons-container">
                        ${state.seasons.map(season => `
                            <div class="rounded-xl border border-vs-border overflow-hidden">
                                <button class="w-full flex items-center justify-between px-5 py-4 bg-vs-card hover:bg-vs-elevated text-left season-toggle-btn" data-season-id="${season.id}">
                                    <div class="flex items-center gap-4">
                                        <span class="font-display font-semibold text-vs-text">Season ${season.seasonNumber}</span>
                                        <span class="text-sm text-vs-muted">${season.episodeCount} episodes</span>
                                    </div>
                                    <i class="fas fa-chevron-down text-vs-muted transition-transform" id="arrow-${season.id}"></i>
                                </button>
                                <div class="hidden" id="season-eps-${season.id}">
                                    <div class="divide-y divide-vs-border" id="season-eps-list-${season.id}"><div class="p-4 text-sm text-vs-muted">Loading...</div></div>
                                </div>
                            </div>`).join('')}
                    </div>
                </section>` : ''}

                ${state.credits?.length ? `<section class="mt-10">
                    <h2 class="font-display font-bold text-xl text-vs-text mb-4">Cast & Crew</h2>
                    <div class="scroll-row">
                        ${state.credits.map(c => `<a href="#/persons/${c.personId}" class="flex-shrink-0 w-32 text-center group block">
                            <img src="${toast.esc(c.personPhotoUrl)}" alt="${toast.esc(c.personName)}" class="w-24 h-24 mx-auto rounded-full object-cover border-2 border-transparent group-hover:border-vs-accent mb-2" loading="lazy">
                            <p class="text-sm font-medium text-vs-text truncate">${toast.esc(c.personName)}</p>
                            <p class="text-xs text-vs-muted truncate">${toast.esc(c.characterName || c.role)}</p>
                        </a>`).join('')}
                    </div>
                </section>` : ''}

                ${state.awards?.length ? `<section class="mt-10 mb-10">
                    <h2 class="font-display font-bold text-xl text-vs-text mb-4">Awards</h2>
                    <div class="grid gap-3 font-body">
                        ${state.awards.map(a => `<div class="flex items-center gap-4 p-4 rounded-xl bg-vs-card border border-vs-border">
                            <div class="w-10 h-10 rounded-full ${a.won ? 'bg-vs-accent/20' : 'bg-vs-muted/10'} flex items-center justify-center"><i class="fas fa-trophy ${a.won ? 'text-vs-accent' : 'text-vs-muted'}"></i></div>
                            <div class="flex-1 min-w-0">
                                <p class="text-sm font-medium text-vs-text">${toast.esc(a.awardName)} ${a.won ? '<span class="text-vs-accent text-xs ml-1 font-semibold">WINNER</span>' : ''}</p>
                                <p class="text-xs text-vs-muted">${toast.esc(a.awardCategory)} &middot; ${a.awardYear || ''}</p>
                            </div>
                        </div>`).join('')}
                    </div>
                </section>` : ''}
            </div>
        </div>`;
        
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        // Season toggle click delegation
        document.getElementById('seasons-container')?.addEventListener('click', e => {
            const btn = e.target.closest('.season-toggle-btn');
            if (btn) {
                const seasonId = btn.dataset.seasonId;
                toggleSeason(seasonId);
            }
        });

        // Add to library
        document.getElementById('add-to-library-btn')?.addEventListener('click', async () => {
            if (!store.isAuthenticated()) { router.navigate('/login'); return; }
            let seasonsHtml = '<option value="">Whole Show</option>';
            state.seasons.forEach(s => {
                seasonsHtml += `<option value="${s.id}">Season ${s.seasonNumber} â€“ ${toast.esc(s.title || '')}</option>`;
            });
            modal.open('Add to My Library',
                `<div class="space-y-4">
                    <div>
                        <label class="form-label">Target</label>
                        <select id="lib-target" class="input-field">${seasonsHtml}</select>
                    </div>
                    <div>
                        <label class="form-label">Status</label>
                        <select id="lib-status" class="input-field">
                            <option value="plan_to_watch">Plan to Watch</option>
                            <option value="watching">Watching</option>
                            <option value="completed">Completed</option>
                            <option value="on_hold">On Hold</option>
                            <option value="dropped">Dropped</option>
                        </select>
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-lib-btn">Add</button>`
            );

            document.getElementById('save-lib-btn')?.addEventListener('click', async () => {
                const seasonId = document.getElementById('lib-target').value;
                const status = document.getElementById('lib-status').value;
                try {
                    const payload = { status };
                    if (seasonId) {
                        payload.seasonId = parseInt(seasonId);
                    } else {
                        payload.showId = state.showId;
                    }
                    await api.post('/profiles/me/library', payload);
                    toast.success('Added to library');
                    modal.close();
                } catch (err) {
                    toast.error(err.message);
                }
            });
        });

        // star click listener
        document.querySelectorAll('.star-btn').forEach(btn => {
            btn.addEventListener('click', async () => {
                const rating = parseInt(btn.dataset.star);
                try {
                    await api.post('/ratings', { showId: state.showId, rating });
                    toast.success('Rating saved!');
                    load(state.showId);
                } catch (err) {
                    toast.error('Failed to save rating: ' + err.message);
                }
            });
        });

        // delete rating listener
        document.getElementById('delete-rating-btn')?.addEventListener('click', async () => {
            try {
                await api.del(`/shows/${state.showId}/ratings/me`);
                toast.success('Rating removed!');
                load(state.showId);
            } catch (err) {
                toast.error('Failed to remove rating: ' + err.message);
            }
        });
    }

    async function toggleSeason(seasonId) {
        const container = document.getElementById(`season-eps-${seasonId}`);
        const arrow = document.getElementById(`arrow-${seasonId}`);
        if (!container) return;
        const hidden = container.classList.contains('hidden');
        if (hidden) {
            container.classList.remove('hidden');
            arrow?.classList.add('rotate-180');
            if (!container.dataset.loaded) {
                container.dataset.loaded = 'true';
                try {
                    const episodes = await api.get(`/seasons/${seasonId}/episodes`);
                    const list = document.getElementById(`season-eps-list-${seasonId}`);
                    list.innerHTML = episodes.map(ep => `<div class="flex items-center gap-4 p-4 hover:bg-vs-elevated transition-colors group relative">
                        <a href="#/episode/${ep.id}" class="flex-1 flex items-center gap-4 min-w-0">
                            <span class="text-lg font-display font-bold text-vs-muted w-8 text-center">${ep.episodeNumber}</span>
                            <img src="${toast.esc(ep.thumbnailUrl)}" alt="" class="w-28 h-16 rounded-lg object-cover flex-shrink-0" loading="lazy">
                            <div class="flex-1 min-w-0">
                                <p class="text-sm font-medium text-vs-text group-hover:text-vs-accent truncate">${toast.esc(ep.title)}</p>
                                <p class="text-xs text-vs-muted mt-0.5">${ep.runtimeSeconds ? Math.floor(ep.runtimeSeconds / 60) + ':' + String(ep.runtimeSeconds % 60).padStart(2, '0') : ''} ${ep.releaseDate ? '&middot; ' + ep.releaseDate : ''}</p>
                            </div>
                        </a>
                        <div class="flex items-center gap-2 z-20">
                            ${store.isAuthenticated() ? `<button class="btn btn-ghost btn-sm create-watch-party-btn" data-episode-id="${ep.id}" title="Start Watch Party"><i class="fas fa-users text-vs-muted hover:text-vs-accent"></i></button>` : ''}
                            <a href="#/episode/${ep.id}" class="btn btn-ghost btn-sm"><i class="fas fa-play text-xs text-vs-muted group-hover:text-vs-accent"></i></a>
                        </div>
                    </div>`).join('');
                    
                    // Bind Watch Party buttons inside this season's lists
                    list.querySelectorAll('.create-watch-party-btn').forEach(wpBtn => {
                        wpBtn.addEventListener('click', async (e) => {
                            e.preventDefault();
                            e.stopPropagation();
                            const epId = parseInt(wpBtn.dataset.episodeId);
                            try {
                                const res = await api.post('/watch-parties', { episodeId: epId });
                                toast.success(`Watch Party created! Share Code: ${res.PartyCode || res.partyCode}`);
                                router.navigate('/watch-parties');
                            } catch (err) {
                                toast.error('Failed to create watch party: ' + err.message);
                            }
                        });
                    });

                } catch (err) { 
                    const list = document.getElementById(`season-eps-list-${seasonId}`);
                    if (list) list.innerHTML = `<div class="p-4 text-sm text-vs-error">Failed to load episodes: ${err.message}</div>`; 
                }
            }
        } else {
            container.classList.add('hidden');
            arrow?.classList.remove('rotate-180');
        }
    }

    return {
        render() { return '<div id="show-detail-content">' + Comp.pageLoader() + '</div>'; },
        init(params) {
            load(params.id);
        },
        toggleSeason
    };
})();