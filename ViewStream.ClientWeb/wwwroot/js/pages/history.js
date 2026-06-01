pages.history = (() => {
    let state = { page: 1, data: null, loading: true };

    async function load() {
        state.loading = true; render();
        try {
            state.data = await api.get('/profiles/me/history', {
                page: state.page,
                pageSize: CONFIG.PAGE_SIZE
            });
        } catch (err) {
            toast.error('Failed to load watch history: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('history-content');
        if (!c) return;
        
        let h = Comp.pageHeader('Watch History');
        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data?.items?.length) {
            h += Comp.emptyState('fa-history', 'No watch history yet');
        } else {
            h += `<div class="space-y-3 font-body">
                ${state.data.items.map(item => {
                    const pct = item.totalSeconds ? Math.round((item.progressSeconds / item.totalSeconds) * 100) : 0;
                    const watchedDate = item.watchedAt ? new Date(item.watchedAt).toLocaleDateString() : 'â€”';
                    return `<a href="#/episode/${item.episodeId}" class="flex items-center gap-4 p-3 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors group block">
                        <div class="relative w-40 flex-shrink-0">
                            <img src="${toast.esc(item.showPosterUrl)}" alt="" class="w-full aspect-video rounded-lg object-cover">
                            <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                                <div class="w-10 h-10 rounded-full bg-vs-accent/90 flex items-center justify-center"><i class="fas fa-play text-vs-bg text-sm ml-0.5"></i></div>
                            </div>
                            <div class="absolute bottom-0 left-0 right-0 h-1 bg-black/30 rounded-b-lg">
                                <div class="h-full bg-vs-accent rounded-bl-lg" style="width:${pct}%"></div>
                            </div>
                        </div>
                        <div class="flex-1 min-w-0">
                            <p class="text-sm font-semibold text-vs-text group-hover:text-vs-accent truncate">${toast.esc(item.episodeTitle)}</p>
                            <p class="text-xs text-vs-muted mt-0.5">${toast.esc(item.showTitle)} &middot; S${item.seasonNumber} &middot; Watched on ${watchedDate}</p>
                            <p class="text-xs text-vs-muted mt-1">
                                ${item.completed ? 
                                    '<span class="text-vs-success font-semibold flex items-center gap-1"><i class="fas fa-check-circle text-xs"></i> Completed</span>' : 
                                    `<span class="flex items-center gap-1"><i class="far fa-circle text-xs"></i> ${Math.floor(item.progressSeconds / 60)}m / ${Math.floor(item.totalSeconds / 60)}m watched</span>`
                                }
                            </p>
                        </div>
                    </a>`;
                }).join('')}
            </div>`;
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; load(); });
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="history-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();