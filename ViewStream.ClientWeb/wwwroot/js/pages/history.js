pages.history = (() => {
    let state = { history: [], loading: true };

    async function load() {
        state.loading = true; render();
        try {
            // Get full watch history (using existing endpoint or custom)
            // Assuming GET /profiles/me/history?page=1&pageSize=50
            const res = await api.get('/profiles/me/history', { page: 1, pageSize: 50 });
            state.history = res.items || [];
        } catch (err) { toast.error('Failed to load history'); }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('history-content');
        if (!c) return;
        let h = Comp.pageHeader('Watch History');
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.history.length) { h += Comp.emptyState('fa-history', 'No watch history yet'); }
        else {
            h += `<div class="space-y-3">
                ${state.history.map(item => {
                const pct = item.totalSeconds ? Math.round((item.progressSeconds / item.totalSeconds) * 100) : 0;
                return `<a href="#/episode/${item.episodeId}" class="flex items-center gap-4 p-3 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors group">
                        <div class="relative w-40 flex-shrink-0">
                            <img src="${toast.esc(item.showPosterUrl)}" alt="" class="w-full aspect-video rounded-lg object-cover">
                            <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity"><div class="w-10 h-10 rounded-full bg-vs-accent/90 flex items-center justify-center"><i class="fas fa-play text-vs-bg text-sm ml-0.5"></i></div></div>
                            <div class="absolute bottom-0 left-0 right-0 h-1 bg-black/30 rounded-b-lg"><div class="h-full bg-vs-accent rounded-bl-lg" style="width:${pct}%"></div></div>
                        </div>
                        <div class="flex-1 min-w-0">
                            <p class="text-sm font-medium text-vs-text group-hover:text-vs-accent">${toast.esc(item.episodeTitle)}</p>
                            <p class="text-xs text-vs-muted mt-0.5">${toast.esc(item.showTitle)} &middot; S${item.seasonNumber} &middot; ${utils.formatDate(item.watchedAt)}</p>
                            <p class="text-xs text-vs-muted mt-1">${item.completed ? '<span class="text-vs-success">Completed</span>' : `${Math.floor(item.progressSeconds / 60)}m / ${Math.floor(item.totalSeconds / 60)}m watched`}</p>
                        </div>
                    </a>`;
            }).join('')}
            </div>`;
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="history-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();