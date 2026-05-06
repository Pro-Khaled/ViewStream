pages.showDetail = (() => {
    let state = { showId: null, show: null, seasons: null, credits: null, awards: null, rating: null, loading: true };

    async function load(id) {
        state.showId = id; state.loading = true; render();
        try {
            const [show, seasons, credits, awards, rating] = await Promise.all([
                api.get(`/shows/${id}`),
                api.get(`/shows/${id}/seasons`),
                api.get(`/shows/${id}/credits`),
                api.get(`/shows/${id}/awards`),
                api.get(`/shows/${id}/ratings/summary`)
            ]);
            state.show = show; state.seasons = seasons; state.credits = credits; state.awards = awards; state.rating = rating;
        } catch (err) { toast.error('Failed to load show'); }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('show-detail-content');
        if (!c) return;
        if (state.loading) { c.innerHTML = Comp.pageLoader(); return; }
        if (!state.show) { c.innerHTML = Comp.emptyState('fa-film', 'Show not found'); return; }

        const show = state.show;
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
                        </div>
                        <div class="flex flex-wrap gap-2 mb-4">
                            ${(show.genres || []).map(g => `<span class="px-3 py-1 rounded-full bg-vs-accent/10 text-vs-accent text-xs font-medium">${toast.esc(g)}</span>`).join('')}
                        </div>
                        <p class="text-vs-dim leading-relaxed mb-6 max-w-2xl">${toast.esc(show.description)}</p>
                        <div class="flex items-center gap-3 mb-6">
                            ${state.seasons?.length ? `<a href="#/episode/${state.seasons[0].id}01" class="inline-flex items-center gap-2 px-6 py-3 bg-vs-accent hover:bg-vs-accentHover text-vs-bg font-bold rounded-xl transition-colors"><i class="fas fa-play text-sm"></i> Play</a>` : ''}
                            <button id="add-to-library-btn" class="inline-flex items-center gap-2 px-5 py-3 bg-vs-card hover:bg-vs-elevated text-vs-text font-semibold rounded-xl border border-vs-border transition-colors"><i class="fas fa-bookmark"></i> My Library</button>
                        </div>
                        ${state.rating ? `<div class="flex items-center gap-4 p-4 rounded-xl bg-vs-card/50 border border-vs-border/50 mb-6">
                            <div class="text-center"><div class="text-3xl font-display font-black text-vs-accent">${state.rating.averageRating.toFixed(1)}</div><div class="text-xs text-vs-muted mt-1">${state.rating.totalRatings} ratings</div></div>
                            <div class="flex-1">${[5, 4, 3, 2, 1].map(s => `<div class="flex items-center gap-2 mb-1"><span class="text-xs text-vs-muted w-3">${s}</span><div class="flex-1 h-2 rounded-full bg-vs-border"><div class="h-full rounded-full bg-vs-accent/60" style="width:${20 * s}%"></div></div></div>`).join('')}</div>
                        </div>` : ''}
                        <div class="grid grid-cols-3 gap-4 text-center">
                            <div class="p-3 rounded-lg bg-vs-card/30"><div class="text-xl font-bold text-vs-text">${show.seasonCount}</div><div class="text-xs text-vs-muted">Seasons</div></div>
                            <div class="p-3 rounded-lg bg-vs-card/30"><div class="text-xl font-bold text-vs-text">${show.episodeCount}</div><div class="text-xs text-vs-muted">Episodes</div></div>
                            <div class="p-3 rounded-lg bg-vs-card/30"><div class="text-xl font-bold text-vs-text">${state.awards.length}</div><div class="text-xs text-vs-muted">Awards</div></div>
                        </div>
                    </div>
                </div>
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
                        ${state.credits.map(c => `<a href="#/persons/${c.personId}" class="flex-shrink-0 w-32 text-center group">
                            <img src="${toast.esc(c.personPhotoUrl)}" alt="${toast.esc(c.personName)}" class="w-24 h-24 mx-auto rounded-full object-cover border-2 border-transparent group-hover:border-vs-accent mb-2" loading="lazy">
                            <p class="text-sm font-medium text-vs-text truncate">${toast.esc(c.personName)}</p>
                            <p class="text-xs text-vs-muted truncate">${toast.esc(c.characterName || c.role)}</p>
                        </a>`).join('')}
                    </div>
                </section>` : ''}
                ${state.awards?.length ? `<section class="mt-10">
                    <h2 class="font-display font-bold text-xl text-vs-text mb-4">Awards</h2>
                    <div class="grid gap-3">
                        ${state.awards.map(a => `<div class="flex items-center gap-4 p-4 rounded-xl bg-vs-card border border-vs-border">
                            <div class="w-10 h-10 rounded-full ${a.won ? 'bg-vs-accent/20' : 'bg-vs-muted/10'} flex items-center justify-center"><i class="fas fa-trophy ${a.won ? 'text-vs-accent' : 'text-vs-muted'}"></i></div>
                            <div class="flex-1 min-w-0"><p class="text-sm font-medium text-vs-text">${toast.esc(a.awardName)} ${a.won ? '<span class="text-vs-accent text-xs ml-1">WINNER</span>' : ''}</p><p class="text-xs text-vs-muted">${toast.esc(a.awardCategory)} &middot; ${a.awardYear || ''}</p></div>
                        </div>`).join('')}
                    </div>
                </section>` : ''}
            </div>
        </div>`;
        c.innerHTML = h;
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
                    list.innerHTML = episodes.map(ep => `<a href="#/episode/${ep.id}" class="flex items-center gap-4 p-4 hover:bg-vs-elevated transition-colors group">
                        <span class="text-lg font-display font-bold text-vs-muted w-8 text-center">${ep.episodeNumber}</span>
                        <img src="${toast.esc(ep.thumbnailUrl)}" alt="" class="w-28 h-16 rounded-lg object-cover flex-shrink-0" loading="lazy">
                        <div class="flex-1 min-w-0">
                            <p class="text-sm font-medium text-vs-text group-hover:text-vs-accent">${toast.esc(ep.title)}</p>
                            <p class="text-xs text-vs-muted mt-0.5">${ep.runtimeSeconds ? Math.floor(ep.runtimeSeconds / 60) + ':' + String(ep.runtimeSeconds % 60).padStart(2, '0') : ''} ${ep.releaseDate ? '&middot; ' + ep.releaseDate : ''}</p>
                        </div>
                        <i class="fas fa-play text-xs text-vs-muted group-hover:text-vs-accent"></i>
                    </a>`).join('');
                } catch (err) { list.innerHTML = `<div class="p-4 text-sm text-vs-error">Failed to load episodes</div>`; }
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
            // Bind season toggles
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
                try {
                    await api.post('/profiles/me/library', { showId: state.showId, status: 'plan_to_watch' });
                    toast.success('Added to library');
                } catch (err) { toast.error(err.message); }
            });
        },
        toggleSeason
    };
})();