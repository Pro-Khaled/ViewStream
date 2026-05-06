pages.genres = (() => {
    let state = { genres: [], loading: true };

    async function load() {
        state.loading = true;
        render();
        try {
            state.genres = await api.get('/genres/all');
        } catch (err) {
            toast.error('Failed to load genres');
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('genres-content');
        if (!c) return;

        let h = Comp.pageHeader('Browse by Genre');
        if (state.loading) {
            h += Comp.pageLoader();
        } else {
            const colors = ['from-red-900/40 to-red-950/20', 'from-blue-900/40 to-blue-950/20', 'from-green-900/40 to-green-950/20', 'from-purple-900/40 to-purple-950/20', 'from-amber-900/40 to-amber-950/20', 'from-rose-900/40 to-rose-950/20'];
            h += '<div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">';
            state.genres.forEach((g, i) => {
                h += `<a href="#/shows?genreId=${g.id}&page=1" class="relative rounded-xl overflow-hidden bg-gradient-to-br ${colors[i % colors.length]} border border-vs-border/50 hover:border-vs-dim transition-all hover:scale-[1.02] group p-6">
                    <div class="absolute inset-0 bg-black/20 group-hover:bg-black/10 transition-colors"></div>
                    <div class="relative z-10">
                        <h2 class="font-display font-bold text-xl text-white mb-1">${toast.esc(g.name)}</h2>
                        <p class="text-sm text-white/60">${g.showCount} shows</p>
                    </div>
                </a>`;
            });
            h += '</div>';
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="genres-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();