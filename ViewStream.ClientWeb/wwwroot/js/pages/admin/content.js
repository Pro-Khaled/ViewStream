pages.adminContent = (() => {
    let state = { page: 1, data: null, loading: true };
    let sortKey = 'id';
    let sortDir = 'asc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/shows', {
                page: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                includeDeleted: true,
                orderBy: sortKey,
                isDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load shows'); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-content-management-content');
        if (!c) return;
        let h = Comp.pageHeader('Content Management', '', `<div class="flex gap-2">
            <a href="#/admin/content/genres" class="btn btn-secondary btn-sm">Genres</a>
            <a href="#/admin/content/tags" class="btn btn-secondary btn-sm">Tags</a>
            <a href="#/admin/content/persons" class="btn btn-secondary btn-sm">Persons</a>
            <button class="btn btn-primary btn-sm" id="create-show-btn"><i class="fas fa-plus"></i> Add Show</button>
        </div>`);
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-film', 'No shows'); }
        else {
            const rows = state.data.items.map(s => {
                const relationBtns = [
                    `<a href="#/seasons?showId=${s.id}" class="btn btn-sm btn-outline-theme me-1" style="font-size:0.7rem; padding:0.25rem 0.6rem;" title="Seasons"><i class="fas fa-layer-group"></i></a>`,
                    `<a href="#/credits?showId=${s.id}" class="btn btn-sm btn-outline-theme me-1" title="Credits"><i class="fas fa-user-tie"></i></a>`,
                    `<a href="#/showawards?showId=${s.id}" class="btn btn-sm btn-outline-theme me-1" title="Awards"><i class="fas fa-trophy"></i></a>`,
                    `<a href="#/showavailabilities?showId=${s.id}" class="btn btn-sm btn-outline-theme" title="Availability"><i class="fas fa-globe"></i></a>`
                ].join('');
                return `<tr>
                    <td class="text-muted">${s.id}</td>
                    <td class="font-medium">${toast.esc(s.title)}</td>
                    <td class="text-muted">${s.releaseYear || '—'}</td>
                    <td class="text-vs-accent">${s.imdbRating || '—'}</td>
                    <td class="text-xs text-vs-dim">${(s.genres || []).slice(0, 2).join(', ')}</td>
                    <td nowrap>${relationBtns}</td>
                    <td>
                        <button class="btn btn-ghost btn-sm edit-show-btn" data-id="${s.id}"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-ghost btn-sm text-vs-error delete-show-btn" data-id="${s.id}"><i class="fas fa-trash"></i></button>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'title', label: 'Title' },
                    { key: 'releaseYear', label: 'Year' },
                    { key: 'imdbRating', label: 'Rating' },
                    { key: '', label: 'Genres' },       // non-sortable
                    { key: '', label: 'Relations' },    // non-sortable
                    { key: '', label: 'Actions' }       // non-sortable
                ],
                rows,
                'No shows found',
                {
                    tableId: 'shows-table',
                    sortKey,
                    sortDir,
                    onSort: (key, dir) => {
                        if (key) {
                            sortKey = key;
                            sortDir = dir;
                            loadData();
                        }
                    }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('create-show-btn')?.addEventListener('click', () => {
            modal.open('Create Show', `<div class="space-y-4 max-h-[60vh] overflow-y-auto pr-1">
                <div><label class="form-label">Title *</label><input class="input-field" id="cs-title" required></div>
                <div><label class="form-label">Description</label><textarea class="input-field" id="cs-desc" rows="3"></textarea></div>
                <div class="grid grid-cols-2 gap-4">
                    <div><label class="form-label">Release Year</label><input class="input-field" type="number" id="cs-year"></div>
                    <div><label class="form-label">Maturity Rating</label><select class="input-field" id="cs-maturity"><option value="">None</option><option>TV-G</option><option>TV-PG</option><option>TV-14</option><option>TV-MA</option></select></div>
                    <div><label class="form-label">Runtime (min)</label><input class="input-field" type="number" id="cs-runtime"></div>
                    <div><label class="form-label">IMDb Rating</label><input class="input-field" type="number" step="0.1" id="cs-imdb"></div>
                </div></div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-new-show">Create</button>`, 'max-w-2xl');
            document.getElementById('save-new-show')?.addEventListener('click', async () => {
                const title = document.getElementById('cs-title').value.trim();
                if (!title) { toast.warning('Title required'); return; }
                await api.post('/shows', {
                    title,
                    description: document.getElementById('cs-desc').value || null,
                    releaseYear: parseInt(document.getElementById('cs-year').value) || null,
                    maturityRating: document.getElementById('cs-maturity').value || null,
                    runtimeMinutes: parseInt(document.getElementById('cs-runtime').value) || null,
                    imdbRating: parseFloat(document.getElementById('cs-imdb').value) || null,
                    genreIds: [], tagIds: []
                });
                modal.close(); toast.success('Show created'); loadData();
            });
        });
        document.querySelectorAll('.edit-show-btn').forEach(b => b.addEventListener('click', async () => {
            const id = b.dataset.id;
            try {
                const show = await api.get(`/shows/${id}`);
                modal.open('Edit Show', `<div class="space-y-4"><div><label class="form-label">Title</label><input class="input-field" id="es-title" value="${toast.esc(show.title)}"></div></div>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-primary" id="save-edit-show">Save</button>`);
                document.getElementById('save-edit-show')?.addEventListener('click', async () => {
                    await api.put(`/shows/${id}`, { title: document.getElementById('es-title').value });
                    modal.close(); toast.success('Show updated'); loadData();
                });
            } catch (err) { toast.error(err.message); }
        }));
        document.querySelectorAll('.delete-show-btn').forEach(b => b.addEventListener('click', async () => {
            if (confirm('Delete this show?')) {
                await api.del(`/shows/${b.dataset.id}`); toast.success('Show deleted'); loadData();
            }
        }));
    }

    return {
        render() { return '<div id="admin-content-management-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();