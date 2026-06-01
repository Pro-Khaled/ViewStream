pages.home = (() => {
    let state = { data: null, loading: true };

    async function load() {
        state.loading = true; render();
        try {
            const genres = await api.get('/genres/all');
            const genre1 = genres[0] || { id: 1, name: 'Action & Adventure' };
            const genre2 = genres[1] || { id: 2, name: 'Drama' };
            const genre3 = genres[2] || { id: 5, name: 'Sci-Fi & Fantasy' };

            const [trending, top, news, action, drama, scifi, cw] = await Promise.all([
                api.get('/shows', { page: 1, pageSize: 12, orderBy: 'imdbRating', isDescending: true }),
                api.get('/shows', { page: 1, pageSize: 12, orderBy: 'rottenTomatoesScore', isDescending: true }),
                api.get('/shows', { page: 1, pageSize: 8, year: new Date().getFullYear() }),
                api.get('/shows', { page: 1, pageSize: 12, genreId: genre1.id }),
                api.get('/shows', { page: 1, pageSize: 12, genreId: genre2.id }),
                api.get('/shows', { page: 1, pageSize: 12, genreId: genre3.id }),
                store.isAuthenticated() ? api.get('/profiles/me/history/continue', { limit: 10 }) : Promise.resolve([])
            ]);
            state.data = { trending, top, news, action, drama, scifi, cw, genre1, genre2, genre3 };
        } catch (err) {
            toast.error('Failed to load home: ' + err.message);
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('home-content');
        if (!c) return;
        let h = '';
        if (state.loading) {
            h = `<div class="hero-gradient px-4 pt-6">${Comp.skeletonRows(4)}</div>`;
        } else if (state.data) {
            const { trending, top, news, action, drama, scifi, cw, genre1, genre2, genre3 } = state.data;
            const hero = trending.items[0];
            if (hero) {
                h += `<section class="relative rounded-2xl overflow-hidden mb-10 h-[400px] sm:h-[480px] lg:h-[540px]">
                    <img src="${toast.esc(hero.backdropUrl || hero.posterUrl)}" alt="" class="absolute inset-0 w-full h-full object-cover">
                    <div class="absolute inset-0 bg-gradient-to-r from-black/90 via-black/50 to-transparent"></div>
                    <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
                    <div class="relative z-10 h-full flex flex-col justify-end p-6 sm:p-10 max-w-2xl">
                        <div class="flex items-center gap-2 mb-3">
                            <span class="px-2 py-0.5 rounded bg-vs-accent text-vs-bg text-xs font-bold">TRENDING</span>
                            ${hero.imdbRating ? `<span class="flex items-center gap-1 text-sm text-vs-accent font-semibold"><i class="fas fa-star text-xs"></i>${hero.imdbRating}</span>` : ''}
                            <span class="text-sm text-white/60">${hero.releaseYear}</span>
                            <span class="text-sm text-white/60">${hero.maturityRating}</span>
                        </div>
                        <h1 class="font-display font-black text-3xl sm:text-4xl lg:text-5xl text-white mb-3 leading-tight">${toast.esc(hero.title)}</h1>
                        <p class="text-sm sm:text-base text-white/70 mb-5 line-clamp-3">${toast.esc(hero.description)}</p>
                        <div class="flex items-center gap-3">
                            <a href="#/shows/${hero.id}" class="inline-flex items-center gap-2 px-6 py-3 bg-vs-accent hover:bg-vs-accentHover text-vs-bg font-bold rounded-xl transition-colors">
                                <i class="fas fa-play text-sm"></i> Watch Now
                            </a>
                            <a href="#/shows/${hero.id}" class="inline-flex items-center gap-2 px-6 py-3 bg-white/10 hover:bg-white/20 text-white font-semibold rounded-xl backdrop-blur-sm transition-colors">
                                <i class="fas fa-info-circle text-sm"></i> Details
                            </a>
                            ${store.isAuthenticated() ? `<button id="regen-recs-btn" title="Refresh recommendations" class="inline-flex items-center gap-2 px-4 py-3 bg-white/10 hover:bg-white/20 text-white rounded-xl backdrop-blur-sm transition-colors">
                                <i class="fas fa-sync-alt text-sm"></i>
                            </button>` : ''}
                        </div>
                    </div>
                </section>`;
            }
            if (cw && cw.length && store.isAuthenticated()) {
                h += `<section class="mb-8">
                    <h2 class="font-display font-bold text-xl text-vs-text mb-4">Continue Watching</h2>
                    <div class="scroll-row px-1">
                        ${cw.map(item => {
                            const pct = item.totalSeconds ? Math.round((item.progressSeconds / item.totalSeconds) * 100) : 0;
                            return `<a href="#/episode/${item.episodeId}" class="w-64 flex-shrink-0 block rounded-xl overflow-hidden bg-vs-card group cursor-pointer">
                                <div class="relative aspect-video">
                                    <img src="${toast.esc(item.showPosterUrl)}" alt="" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300">
                                    <div class="absolute inset-0 bg-black/20 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                                        <div class="w-12 h-12 rounded-full bg-vs-accent/90 flex items-center justify-center"><i class="fas fa-play text-vs-bg ml-0.5"></i></div>
                                    </div>
                                    <div class="absolute bottom-0 left-0 right-0 h-1 bg-black/30"><div class="progress-bar h-full bg-vs-accent" style="width:${pct}%"></div></div>
                                </div>
                                <div class="p-3">
                                    <p class="text-sm font-medium text-vs-text truncate">${toast.esc(item.episodeTitle)}</p>
                                    <p class="text-xs text-vs-muted mt-0.5">${toast.esc(item.showTitle)} &middot; S${item.seasonNumber}</p>
                                </div>
                            </a>`;
                        }).join('')}
                    </div></section>`;
            }
            h += Comp.showRow('Trending Now', trending.items);
            h += Comp.showRow('Top Rated', top.items);
            h += Comp.showRow('New Releases', news.items);
            h += Comp.showRow(genre1.name, action.items);
            h += Comp.showRow(genre2.name, drama.items);
            h += Comp.showRow(genre3.name, scifi.items);
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('regen-recs-btn')?.addEventListener('click', async (e) => {
            const btn = e.currentTarget;
            btn.disabled = true;
            btn.innerHTML = '<i class="fas fa-sync-alt fa-spin text-sm"></i>';
            try {
                await api.post('/profiles/me/recommendations/regenerate', {});
                toast.success('Recommendations refreshed!');
                load();
            } catch (err) {
                toast.error('Failed to refresh: ' + err.message);
            } finally {
                btn.disabled = false;
                btn.innerHTML = '<i class="fas fa-sync-alt text-sm"></i>';
            }
        });
    }

    return {
        render() { return '<div id="home-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();