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
                    isDescending: state.orderBy ? true : undefined
                }),
                api.get('/genres/all')
            ]);
            state.data = showsRes;
            state.genres = genresRes;
        } catch (err) {
            toast.error('Failed to load shows: ' + err.message);
            state.data = null;
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('shows-content');
        if (!c) return;

        let h = Comp.pageHeader('Shows', 'Browse the catalog');

        // Search & filter bar
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col sm:flex-row items-end sm:items-center gap-4 w-full">
                <div class="relative flex-1 w-full">
                    <label class="form-label">Search</label>
                    <div class="relative mt-1">
                        <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-vs-muted text-sm"></i>
                        <input type="text" id="shows-search" value="${toast.esc(state.search)}" placeholder="Search shows..." class="w-full bg-vs-surface border border-vs-border rounded-lg pl-9 pr-4 py-2 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50 transition-all">
                    </div>
                </div>
                <div class="w-full sm:w-48">
                    <label class="form-label">Genre</label>
                    <select id="shows-genre" class="w-full bg-vs-surface border border-vs-border rounded-lg px-3 py-2 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50 mt-1">
                        <option value="">All Genres</option>
                        ${state.genres.map(g => `<option value="${g.id}" ${state.genreId == g.id ? 'selected' : ''}>${toast.esc(g.name)}</option>`).join('')}
                    </select>
                </div>
                <div class="w-full sm:w-28">
                    <label class="form-label">Release Year</label>
                    <input type="number" id="shows-year" value="${state.year || ''}" placeholder="e.g. 2026" class="w-full bg-vs-surface border border-vs-border rounded-lg px-3 py-2 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50 mt-1">
                </div>
                <div class="w-full sm:w-48">
                    <label class="form-label">Sort By</label>
                    <select id="shows-order-by" class="w-full bg-vs-surface border border-vs-border rounded-lg px-3 py-2 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50 mt-1">
                        <option value="">Default Order</option>
                        <option value="addedAt" ${state.orderBy === 'addedAt' ? 'selected' : ''}>Recently Added</option>
                        <option value="imdbRating" ${state.orderBy === 'imdbRating' ? 'selected' : ''}>IMDb Rating</option>
                        <option value="rottenTomatoesScore" ${state.orderBy === 'rottenTomatoesScore' ? 'selected' : ''}>Tomatoes Score</option>
                        <option value="releaseYear" ${state.orderBy === 'releaseYear' ? 'selected' : ''}>Release Year</option>
                    </select>
                </div>
                <button class="btn btn-primary btn-sm w-full sm:w-auto h-[38px] mt-1" id="shows-filter-btn">Filter</button>
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
        bindEvents();
    }

    function bindEvents() {
        const searchInput = document.getElementById('shows-search');
        const genreSelect = document.getElementById('shows-genre');
        const yearInput = document.getElementById('shows-year');
        const orderBySelect = document.getElementById('shows-order-by');
        const filterBtn = document.getElementById('shows-filter-btn');

        const applyFilters = () => {
            state.search = searchInput?.value.trim() || '';
            state.genreId = genreSelect?.value || '';
            state.year = yearInput?.value.trim() || '';
            state.orderBy = orderBySelect?.value || '';
            state.page = 1;
            load();
        };

        filterBtn?.addEventListener('click', applyFilters);

        // Also allow pressing Enter in inputs to submit
        const pressEnter = e => { if (e.key === 'Enter') applyFilters(); };
        searchInput?.addEventListener('keypress', pressEnter);
        yearInput?.addEventListener('keypress', pressEnter);

        // Automatic reload on dropdown change for UX
        genreSelect?.addEventListener('change', applyFilters);
        orderBySelect?.addEventListener('change', applyFilters);

        // Card click navigates to showDetail page instead of intercepting
        const c = document.getElementById('shows-content');
        c?.addEventListener('click', e => {
            const card = e.target.closest('.show-card');
            if (card) {
                e.preventDefault();
                const path = card.getAttribute('href');
                window.location.hash = path;
            }
        });
    }

    return {
        render() { return '<div id="shows-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();