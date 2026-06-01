pages.library = (() => {
    let state = { page: 1, status: '', data: null, loading: true };

    async function load() {
        state.loading = true; render();
        try {
            const params = { page: state.page, pageSize: CONFIG.PAGE_SIZE };
            if (state.status) params.status = state.status;
            state.data = await api.get('/profiles/me/library', params);
        } catch (err) {
            toast.error('Failed to load library: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('library-content');
        if (!c) return;
        
        let headerText = `${state.data?.totalCount || 0} items`;
        let h = Comp.pageHeader('My Library', headerText);
        
        h += `<div class="flex gap-2 mb-6 overflow-x-auto pb-1" id="status-filters">
            ${['all', 'watching', 'completed', 'plan_to_watch', 'on_hold', 'dropped'].map(s => {
                const isActive = (s === 'all' && !state.status) || (s === state.status);
                return `<button class="px-4 py-2 rounded-lg text-sm font-medium whitespace-nowrap transition-colors ${isActive ? 'bg-vs-accent text-vs-bg' : 'bg-vs-card text-vs-dim hover:bg-vs-elevated border border-vs-border'}" data-status="${s}">${s.replace('_', ' ')}</button>`;
            }).join('')}
        </div>`;

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data?.items?.length) {
            h += Comp.emptyState('fa-bookmark', 'Your library is empty')
                .replace('Your library is empty', 'Your library is empty<p class="mt-2"><a href="#/shows" class="text-vs-accent font-semibold hover:underline">Browse Shows</a></p>');
        } else {
            h += `<div class="grid gap-4" id="library-grid">
                ${state.data.items.map(item => {
                    const poster = item.posterUrl || item.showPosterUrl || '';
                    const title = item.title || item.showTitle || '';
                    return `<div class="flex gap-4 p-4 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors library-item" data-item-id="${item.id}">
                        <a href="#/shows/${item.showId}" class="flex-shrink-0">
                            <img src="${toast.esc(poster)}" alt="" class="w-24 h-36 rounded-lg object-cover">
                        </a>
                        <div class="flex-1 min-w-0">
                            <div class="flex items-start justify-between gap-2">
                                <div>
                                    <a href="#/shows/${item.showId}" class="font-display font-semibold text-vs-text hover:text-vs-accent block truncate max-w-xs sm:max-w-md">${toast.esc(title)}</a>
                                    <div class="flex items-center gap-2 mt-1">
                                        ${utils.statusBadge(item.status)}
                                        ${item.userScore ? `<span class="text-xs text-vs-accent"><i class="fas fa-star mr-0.5"></i>${item.userScore}</span>` : ''}
                                    </div>
                                </div>
                                <div class="flex items-center gap-1">
                                    <select class="bg-vs-card border border-vs-border rounded-lg px-2 py-1 text-xs text-vs-text focus:outline-none library-status-select">
                                        ${['watching', 'completed', 'plan_to_watch', 'on_hold', 'dropped'].map(s => `<option value="${s}" ${s === item.status ? 'selected' : ''}>${s.replace('_', ' ')}</option>`).join('')}
                                    </select>
                                    <button class="p-1.5 text-vs-muted hover:text-vs-error library-delete-btn" title="Remove"><i class="fas fa-trash-alt text-xs"></i></button>
                                </div>
                            </div>
                            ${item.episodesWatched ? `<p class="text-xs text-vs-muted mt-2"><i class="far fa-clock mr-1"></i>${item.episodesWatched} episodes watched</p>` : ''}
                            <div class="mt-2">${Comp.starRating(item.userScore || 0, true, `pages.library.rateItem.bind(null,${item.id})`)}</div>
                        </div>
                    </div>`;
                }).join('')}
            </div>`;
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; load(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('status-filters')?.addEventListener('click', e => {
            const btn = e.target.closest('button');
            if (btn) {
                const status = btn.dataset.status;
                state.status = status === 'all' ? '' : status;
                state.page = 1;
                load();
            }
        });

        document.getElementById('library-grid')?.addEventListener('change', e => {
            if (e.target.classList.contains('library-status-select')) {
                const id = parseInt(e.target.closest('.library-item').dataset.itemId);
                api.put(`/profiles/me/library/${id}`, { status: e.target.value })
                    .then(() => {
                        toast.success('Status updated');
                        load();
                    })
                    .catch(err => toast.error(err.message));
            }
        });

        document.getElementById('library-grid')?.addEventListener('click', e => {
            if (e.target.closest('.library-delete-btn')) {
                const id = parseInt(e.target.closest('.library-item').dataset.itemId);
                api.del(`/profiles/me/library/${id}`)
                    .then(() => {
                        toast.success('Removed from library');
                        load();
                    })
                    .catch(err => toast.error(err.message));
            }
        });
    }

    return {
        render() { return '<div id="library-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); },
        rateItem: async function (id, rating) {
            try {
                await api.put(`/profiles/me/library/${id}`, { userScore: rating });
                toast.success(`Rated ${rating}/5`);
                load();
            } catch (err) {
                toast.error(err.message);
            }
        }
    };
})();