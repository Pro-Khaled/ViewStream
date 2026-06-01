pages.persons = (() => {
    let state = { persons: [], loading: true };

    async function load() {
        state.loading = true;
        render();
        try {
            state.persons = await api.get('/persons/all'); // backend returns List<PersonListItemDto>
        } catch (err) {
            toast.error('Failed to load persons');
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('persons-content');
        if (!c) return;

        let h = Comp.pageHeader('Cast & Crew');
        if (state.loading) {
            h += Comp.pageLoader();
        } else {
            h += '<div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-6">';
            state.persons.forEach(p => {
                h += `<a href="#/persons/${p.id}" class="text-center group">
                    <img src="${toast.esc(p.photoUrl)}" alt="${toast.esc(p.name)}" class="w-32 h-32 mx-auto rounded-full object-cover border-3 border-transparent group-hover:border-vs-accent transition-all shadow-lg" loading="lazy">
                    <h3 class="font-display font-semibold text-sm text-vs-text mt-3 group-hover:text-vs-accent">${toast.esc(p.name)}</h3>
                    <p class="text-xs text-vs-muted">${p.creditCount} credits ${p.awardCount ? '&middot; ' + p.awardCount + ' awards' : ''}</p>
                </a>`;
            });
            h += '</div>';
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="persons-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();