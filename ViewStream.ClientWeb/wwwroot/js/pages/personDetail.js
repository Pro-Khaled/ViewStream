pages.personDetail = (() => {
    let state = { person: null, credits: [], loading: true };

    async function load(id) {
        state.loading = true;
        render();
        try {
            const [person, credits] = await Promise.all([
                api.get(`/persons/${id}`),
                api.get(`/persons/${id}/credits`)
            ]);
            state.person = person;
            state.credits = credits;
        } catch (err) {
            toast.error('Failed to load person');
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('person-detail-content');
        if (!c) return;
        if (state.loading) { c.innerHTML = Comp.pageLoader(); return; }
        if (!state.person) { c.innerHTML = Comp.emptyState('fa-user', 'Person not found'); return; }
        const p = state.person;

        let h = `<div class="px-4 sm:px-6 pt-6 max-w-4xl mx-auto">
            <div class="flex flex-col sm:flex-row gap-8 mb-10">
                <img src="${toast.esc(p.photoUrl)}" alt="${toast.esc(p.name)}" class="w-40 h-40 sm:w-48 sm:h-48 rounded-full object-cover border-4 border-vs-border shadow-2xl mx-auto sm:mx-0">
                <div class="text-center sm:text-left">
                    <h1 class="font-display font-black text-3xl text-vs-text mb-2">${toast.esc(p.name)}</h1>
                    ${p.birthDate ? `<p class="text-sm text-vs-muted mb-3">Born ${utils.formatDate(p.birthDate)}</p>` : ''}
                    <p class="text-vs-dim leading-relaxed mb-4">${toast.esc(p.bio)}</p>
                    <div class="flex items-center justify-center sm:justify-start gap-6 text-sm">
                        <span class="text-vs-dim"><span class="font-bold text-vs-text">${p.creditCount}</span> credits</span>
                        <span class="text-vs-dim"><span class="font-bold text-vs-text">${p.awardCount}</span> awards</span>
                    </div>
                </div>
            </div>
            <h2 class="font-display font-bold text-xl text-vs-text mb-4">Known For</h2>
            <div class="scroll-row mb-8">
                ${state.credits.slice(0, 6).map(credit => {
            return `<a href="#/shows/${credit.showId || credit.targetId}" class="flex-shrink-0 w-32 text-center group">
                        <img src="${toast.esc(credit.showPosterUrl || 'placeholder.jpg')}" alt="${toast.esc(credit.showTitle)}" class="w-24 h-36 rounded-lg object-cover mb-1">
                        <p class="text-xs text-vs-text truncate">${toast.esc(credit.showTitle)}</p>
                        <p class="text-xs text-vs-muted">${toast.esc(credit.characterName || credit.role)}</p>
                    </a>`;
        }).join('')}
            </div>
            <h2 class="font-display font-bold text-xl text-vs-text mb-4">Credits</h2>
            ${Comp.dataTable(
            ['Show', 'Role', 'Character'],
            state.credits.map(c => `<tr class="table-row border-b border-vs-border">
                    <td class="px-4 py-3 text-sm text-vs-accent"><a href="#/shows/${c.showId || c.targetId}">${toast.esc(c.showTitle || c.targetTitle)}</a></td>
                    <td class="px-4 py-3 text-sm text-vs-text">${toast.esc(c.role)}</td>
                    <td class="px-4 py-3 text-sm text-vs-dim">${toast.esc(c.characterName || '—')}</td>
                </tr>`)
        )}
        </div>`;
        c.innerHTML = h;
    }

    return {
        render(params) { return '<div id="person-detail-content">' + Comp.pageLoader() + '</div>'; },
        init(params) { load(params.id); }
    };
})();