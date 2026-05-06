pages.shows = (() => {
    let state = { page: 1, search: '', genreId: '', year: '', orderBy: '', data: null, genres: [], loading: true };

    async function load() {
        state.loading = true;
        render();
        try {
            const [showsRes, genresRes] = await Promise.all([
                api.get('/shows', {
                    page: state.page,
                    pageSize: CONFIG.PAGE_SIZE,
                    search: state.search || undefined,
                    genreId: state.genreId || undefined,
                    year: state.year || undefined,
                    orderBy: state.orderBy || undefined,
                    isDescending: true
                }),
                api.get('/genres/all')
            ]);
            state.data = showsRes;
            state.genres = genresRes;
        } catch (err) { toast.error('Failed to load shows'); state.data = null; }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('shows-content');
        if (!c) return;

        let h = Comp.pageHeader('Shows', 'Browse the catalog');

        // Search & filter bar
        h += `<div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-6">
            <div class="flex items-center gap-3 w-full sm:w-auto">
                <div class="relative flex-1 sm:w-56">
                    <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-vs-muted text-sm"></i>
                    <input type="text" id="shows-search" value="${toast.esc(state.search)}" placeholder="Search..." class="w-full bg-vs-card border border-vs-border rounded-lg pl-9 pr-4 py-2 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50 transition-all">
                </div>
                <select id="shows-genre" class="bg-vs-card border border-vs-border rounded-lg px-3 py-2 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50">
                    <option value="">All Genres</option>
                    ${state.genres.map(g => `<option value="${g.id}" ${state.genreId == g.id ? 'selected' : ''}>${toast.esc(g.name)}</option>`).join('')}
                </select>
            </div>
        </div>`;

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data?.items?.length) {
            h += Comp.emptyState('fa-film', 'No shows found');
        } else {
            h += `<p class="text-sm text-vs-muted mb-6">${state.data.totalCount} shows found</p>`;
            h += `<div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-4">
                ${state.data.items.map(s => Comp.showCard(s)).join('')}
            </div>`;
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; load(); });
        }

        c.innerHTML = h;

        // Bind events
        const searchInput = document.getElementById('shows-search');
        const genreSelect = document.getElementById('shows-genre');
        let searchTimeout;
        searchInput?.addEventListener('input', () => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                state.search = searchInput.value.trim();
                state.genreId = genreSelect.value;
                state.page = 1;
                load();
            }, 400);
        });
        genreSelect?.addEventListener('change', () => {
            state.genreId = genreSelect.value;
            state.search = searchInput.value.trim();
            state.page = 1;
            load();
        });

        // Detail modal on card click (delegated)
        c.addEventListener('click', e => {
            const card = e.target.closest('.show-card');
            if (card) {
                e.preventDefault();
                const id = card.getAttribute('href').split('/').pop();
                showDetail(parseInt(id));
            }
        });
    }

    async function showDetail(id) {
        modal.open('Show Details', Comp.pageLoader(), { large: true });
        try {
            const show = await api.get(`/shows/${id}`);
            const genres = (show.genres || []).map(g => `<span class="badge badge-info">${toast.esc(g)}</span>`).join(' ');
            const tags = (show.tags || []).map(t => `<span class="badge badge-muted">${toast.esc(t)}</span>`).join(' ');
            modal.open(`Show #${show.id}`,
                `<div class="detail-grid">
                    ${Comp.detailRow('ID', show.id)}
                    ${Comp.detailRow('Title', toast.esc(show.title))}
                    ${Comp.detailRow('Description', show.description ? utils.truncate(show.description, 200) : '—')}
                    ${Comp.detailRow('Release Year', show.releaseYear || '—')}
                    ${Comp.detailRow('Maturity', show.maturityRating || '—')}
                    ${Comp.detailRow('Runtime', show.runtimeMinutes ? show.runtimeMinutes + ' min' : '—')}
                    ${Comp.detailRow('IMDb', show.imdbRating || '—')}
                    ${Comp.detailRow('Tomatoes', show.rottenTomatoesScore != null ? show.rottenTomatoesScore + '%' : '—')}
                    ${Comp.detailRow('Seasons', show.seasonCount)}
                    ${Comp.detailRow('Episodes', show.episodeCount)}
                    ${Comp.detailRow('Added', utils.formatDate(show.addedAt))}
                    ${Comp.detailRow('Updated', utils.formatDate(show.updatedAt))}
                </div>
                <div class="mt-4"><div class="detail-label mb-2">Genres</div><div class="flex flex-wrap gap-2">${genres}</div></div>
                ${tags ? `<div class="mt-4"><div class="detail-label mb-2">Tags</div><div class="flex flex-wrap gap-2">${tags}</div></div>` : ''}`,
                { large: true });
        } catch (err) { toast.error('Failed to load show details'); modal.close(); }
    }

    return {
        render() { return '<div id="shows-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();