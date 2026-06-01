// Admin Content Management: Shows (tabs for Seasons and Episodes handled in their own pages)
pages.adminContent = (() => {
    let state = { page: 1, data: null, loading: true, search: '', includeDeleted: false };
    let sortKey = 'title', sortDir = 'asc';
    let _genres = [], _tags = [];

    async function loadMeta() {
        try { _genres = await api.get('/genres/all'); } catch { _genres = []; }
        try { _tags = await api.get('/contenttags/all'); } catch { _tags = []; }
    }

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/shows', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error(err.message || 'Failed to load shows'); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-content-root');
        if (!c) return;
        let h = Comp.pageHeader('Content Management', 'Manage shows, seasons and episodes', `
            <div class="flex items-center gap-2">
                <a href="#/admin/seasons" class="btn btn-secondary btn-sm"><i class="fas fa-layer-group mr-1 text-xs"></i>Seasons</a>
                <a href="#/admin/episodes" class="btn btn-secondary btn-sm"><i class="fas fa-play-circle mr-1 text-xs"></i>Episodes</a>
                <button class="btn btn-primary btn-sm" id="admin-create-show-btn"><i class="fas fa-plus mr-1 text-xs"></i>Add Show</button>
            </div>`);

        // Filter bar
        h += `<div class="card p-4 mb-6 font-body">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Shows</label>
                    <input class="input-field" id="show-search" placeholder="Search shows by title..." value="${toast.esc(state.search)}">
                </div>
                <div class="flex items-center gap-2 mb-2 flex-shrink-0">
                    <input type="checkbox" id="show-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="show-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="show-search-btn"><i class="fas fa-search mr-1.5"></i>Search</button>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-film', 'No shows found'); }
        else {
            const rows = state.data.items.map(s => `<tr class="${s.isDeleted ? 'opacity-50' : ''}">
                <td class="text-vs-muted text-xs font-mono">#${s.id}</td>
                <td>
                    <div class="flex items-center gap-3">
                        ${s.posterUrl ? `<img src="${toast.esc(s.posterUrl)}" class="w-8 h-11 object-cover rounded shadow-sm border border-vs-border" onerror="this.style.display='none'">` : '<div class="w-8 h-11 bg-vs-surface-2 rounded border border-vs-border flex items-center justify-center"><i class="fas fa-film text-xs text-vs-muted"></i></div>'}
                        <div>
                            <div class="font-medium text-sm text-vs-text">${toast.esc(s.title)}</div>
                            <div class="text-xs text-vs-muted mt-0.5">${(s.genres||[]).slice(0,3).join(', ')||'—'}</div>
                        </div>
                    </div>
                </td>
                <td class="text-vs-muted text-sm font-mono">${s.releaseYear||'—'}</td>
                <td class="text-vs-accent text-sm font-bold"><i class="fas fa-star text-xs text-yellow-400 mr-1"></i>${s.imdbRating||'—'}</td>
                <td class="text-sm font-mono">${s.seasonCount||0} Seasons &middot; ${s.episodeCount||0} Episodes</td>
                <td>${s.isDeleted ? '<span class="badge badge-danger">Deleted</span>' : '<span class="badge badge-success">Active</span>'}</td>
                <td class="text-right whitespace-nowrap">
                    <button class="btn btn-ghost btn-sm upload-show-poster" data-id="${s.id}" title="Upload Poster"><i class="fas fa-image text-vs-info text-xs"></i></button>
                    <button class="btn btn-ghost btn-sm edit-show-btn" data-id="${s.id}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                    ${s.isDeleted
                        ? `<button class="btn btn-ghost btn-sm text-vs-success restore-show-btn" data-id="${s.id}" title="Restore"><i class="fas fa-undo text-xs"></i></button>`
                        : `<button class="btn btn-ghost btn-sm text-vs-error delete-show-btn" data-id="${s.id}" title="Delete"><i class="fas fa-trash text-xs"></i></button>`}
                </td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key:'Id',label:'ID'},{key:'Title',label:'Title'},{key:'ReleaseYear',label:'Year'},{key:'ImdbRating',label:'IMDb'},{label:'Structure'},{label:'Status'},{label:''}],
                rows, 'No shows', { tableId:'admin-shows-tbl', sortKey, sortDir, onSort:(k,d)=>{ sortKey=k; sortDir=d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p=>{ state.page=p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function renderChipSelector(items, selectedNamesOrIds, prefix) {
        return `<div class="flex flex-wrap gap-1.5 max-h-40 overflow-y-auto p-3 bg-vs-surface-2 border border-vs-border rounded-xl">
            ${items.map(item => {
                const isSelected = selectedNamesOrIds.includes(item.Id || item.id) || selectedNamesOrIds.includes(item.Name || item.name);
                const activeClass = isSelected ? 'bg-vs-accent text-vs-bg border-vs-accent shadow-md' : 'bg-vs-surface border-vs-border text-vs-dim hover:border-vs-dim';
                return `<button type="button" class="btn btn-xs px-2.5 py-1 rounded-full border text-xxs font-semibold chip-select transition-all ${activeClass}" data-prefix="${prefix}" data-id="${item.Id || item.id}">
                    ${toast.esc(item.Name || item.name)}
                </button>`;
            }).join('')}
        </div>`;
    }

    function bindChipSelectorEvents() {
        document.querySelectorAll('.chip-select').forEach(btn => btn.addEventListener('click', () => {
            btn.classList.toggle('bg-vs-accent');
            btn.classList.toggle('text-vs-bg');
            btn.classList.toggle('border-vs-accent');
            btn.classList.toggle('shadow-md');
            btn.classList.toggle('bg-vs-surface');
            btn.classList.toggle('border-vs-border');
            btn.classList.toggle('text-vs-dim');
            btn.classList.toggle('hover:border-vs-dim');
        }));
    }

    function getSelectedChipIds(prefix) {
        const selected = [];
        document.querySelectorAll(`.chip-select[data-prefix="${prefix}"]`).forEach(btn => {
            if (btn.classList.contains('bg-vs-accent')) {
                selected.push(parseInt(btn.dataset.id));
            }
        });
        return selected;
    }

    function showCreateModal() {
        modal.open('Create Show', `<div class="space-y-4 max-h-[70vh] overflow-y-auto pr-2 font-body">
            <div class="grid grid-cols-2 gap-4">
                <div class="col-span-2"><label class="form-label">Title <span class="text-vs-danger">*</span></label><input class="input-field" id="cs-title"></div>
                <div class="col-span-2"><label class="form-label">Description</label><textarea class="input-field" id="cs-desc" rows="3" placeholder="Enter full show description..."></textarea></div>
                <div><label class="form-label">Release Year</label><input class="input-field" type="number" id="cs-year" min="1888" max="2100" placeholder="e.g. 2026"></div>
                <div><label class="form-label">Maturity Rating</label><select class="input-field" id="cs-maturity"><option value="">None</option><option>TV-G</option><option>TV-PG</option><option>TV-14</option><option>TV-MA</option><option>PG</option><option>PG-13</option><option>R</option></select></div>
                <div><label class="form-label">Runtime (minutes)</label><input class="input-field" type="number" id="cs-runtime" placeholder="e.g. 120"></div>
                <div><label class="form-label">IMDb Rating (0-10)</label><input class="input-field" type="number" step="0.1" min="0" max="10" id="cs-imdb" placeholder="e.g. 8.5"></div>
                <div><label class="form-label">RT Score (0-100)</label><input class="input-field" type="number" min="0" max="100" id="cs-rt" placeholder="e.g. 92"></div>
                <div><label class="form-label">Poster URL</label><input class="input-field" id="cs-poster" placeholder="https://example.com/poster.jpg"></div>
                <div><label class="form-label">Backdrop URL</label><input class="input-field" id="cs-backdrop" placeholder="https://example.com/backdrop.jpg"></div>
                <div><label class="form-label">Trailer URL</label><input class="input-field" id="cs-trailer" placeholder="https://example.com/trailer.mp4"></div>
            </div>
            <div>
                <label class="form-label mb-2">Genres</label>
                ${renderChipSelector(_genres, [], 'cs-genre')}
            </div>
            <div>
                <label class="form-label mb-2">Tags</label>
                ${renderChipSelector(_tags, [], 'cs-tag')}
            </div>
        </div>`,
        { large: true, footer: `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button><button class="btn btn-primary" id="cs-save">Create Show</button>` });
        
        bindChipSelectorEvents();

        document.getElementById('cs-save')?.addEventListener('click', async () => {
            const title = document.getElementById('cs-title').value.trim();
            if (!title) { toast.warning('Title is required'); return; }
            const genreIds = getSelectedChipIds('cs-genre');
            const tagIds = getSelectedChipIds('cs-tag');
            try {
                await api.post('/shows', {
                    title, genreIds, tagIds,
                    description: document.getElementById('cs-desc').value||null,
                    releaseYear: parseInt(document.getElementById('cs-year').value)||null,
                    maturityRating: document.getElementById('cs-maturity').value||null,
                    runtimeMinutes: parseInt(document.getElementById('cs-runtime').value)||null,
                    imdbRating: parseFloat(document.getElementById('cs-imdb').value)||null,
                    rottenTomatoesScore: parseInt(document.getElementById('cs-rt').value)||null,
                    posterUrl: document.getElementById('cs-poster').value||null,
                    backdropUrl: document.getElementById('cs-backdrop').value||null,
                    trailerUrl: document.getElementById('cs-trailer').value||null,
                });
                modal.close(); toast.success('Show created successfully'); loadData();
            } catch(err) { toast.error(err.message); }
        });
    }

    async function showEditModal(id) {
        try {
            const s = await api.get(`/shows/${id}`);
            modal.open(`Edit Show: ${toast.esc(s.title)}`, `<div class="space-y-4 max-h-[70vh] overflow-y-auto pr-2 font-body">
                <div class="grid grid-cols-2 gap-4">
                    <div class="col-span-2"><label class="form-label">Title <span class="text-vs-danger">*</span></label><input class="input-field" id="es-title" value="${toast.esc(s.title)}"></div>
                    <div class="col-span-2"><label class="form-label">Description</label><textarea class="input-field" id="es-desc" rows="3">${toast.esc(s.description||'')}</textarea></div>
                    <div><label class="form-label">Release Year</label><input class="input-field" type="number" id="es-year" value="${s.releaseYear||''}"></div>
                    <div><label class="form-label">Maturity Rating</label><select class="input-field" id="es-maturity"><option value="">None</option><option ${s.maturityRating==='TV-G'?'selected':''}>TV-G</option><option ${s.maturityRating==='TV-PG'?'selected':''}>TV-PG</option><option ${s.maturityRating==='TV-14'?'selected':''}>TV-14</option><option ${s.maturityRating==='TV-MA'?'selected':''}>TV-MA</option><option ${s.maturityRating==='PG'?'selected':''}>PG</option><option ${s.maturityRating==='PG-13'?'selected':''}>PG-13</option><option ${s.maturityRating==='R'?'selected':''}>R</option></select></div>
                    <div><label class="form-label">Runtime (min)</label><input class="input-field" type="number" id="es-runtime" value="${s.runtimeMinutes||''}"></div>
                    <div><label class="form-label">IMDb Rating</label><input class="input-field" type="number" step="0.1" id="es-imdb" value="${s.imdbRating||''}"></div>
                    <div><label class="form-label">RT Score</label><input class="input-field" type="number" id="es-rt" value="${s.rottenTomatoesScore||''}"></div>
                    <div><label class="form-label">Poster URL</label><input class="input-field" id="es-poster" value="${toast.esc(s.posterUrl||'')}"></div>
                    <div><label class="form-label">Backdrop URL</label><input class="input-field" id="es-backdrop" value="${toast.esc(s.backdropUrl||'')}"></div>
                    <div><label class="form-label">Trailer URL</label><input class="input-field" id="es-trailer" value="${toast.esc(s.trailerUrl||'')}"></div>
                </div>
                <div>
                    <label class="form-label mb-2">Genres</label>
                    ${renderChipSelector(_genres, s.genres || [], 'es-genre')}
                </div>
                <div>
                    <label class="form-label mb-2">Tags</label>
                    ${renderChipSelector(_tags, s.tags || [], 'es-tag')}
                </div>
                <div class="border-t border-vs-border pt-4">
                    <p class="text-sm text-vs-text font-bold mb-2">Upload Files (replaces URLs above)</p>
                    <div class="grid grid-cols-3 gap-3">
                        <div><label class="form-label text-xxs font-bold">Poster Image</label><input type="file" class="input-field text-xs p-1" id="es-poster-file" accept="image/*"></div>
                        <div><label class="form-label text-xxs font-bold">Backdrop Image</label><input type="file" class="input-field text-xs p-1" id="es-backdrop-file" accept="image/*"></div>
                        <div><label class="form-label text-xxs font-bold">Trailer Video</label><input type="file" class="input-field text-xs p-1" id="es-trailer-file" accept="video/*"></div>
                    </div>
                </div>
            </div>`,
            { large: true, footer: `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button><button class="btn btn-primary" id="es-save">Save Changes</button>` });

            bindChipSelectorEvents();

            document.getElementById('es-save')?.addEventListener('click', async () => {
                const title = document.getElementById('es-title').value.trim();
                if (!title) { toast.warning('Title is required'); return; }
                const genreIds = getSelectedChipIds('es-genre');
                const tagIds   = getSelectedChipIds('es-tag');
                try {
                    await api.put(`/shows/${id}`, {
                        title, genreIds, tagIds,
                        description: document.getElementById('es-desc').value||null,
                        releaseYear: parseInt(document.getElementById('es-year').value)||null,
                        maturityRating: document.getElementById('es-maturity').value||null,
                        runtimeMinutes: parseInt(document.getElementById('es-runtime').value)||null,
                        imdbRating: parseFloat(document.getElementById('es-imdb').value)||null,
                        rottenTomatoesScore: parseInt(document.getElementById('es-rt').value)||null,
                        posterUrl: document.getElementById('es-poster').value||null,
                        backdropUrl: document.getElementById('es-backdrop').value||null,
                        trailerUrl: document.getElementById('es-trailer').value||null,
                    });
                    // Handle file uploads
                    const posterFile   = document.getElementById('es-poster-file').files[0];
                    const backdropFile = document.getElementById('es-backdrop-file').files[0];
                    const trailerFile  = document.getElementById('es-trailer-file').files[0];
                    if (posterFile)   { const fd=new FormData(); fd.append('posterFile',posterFile);     await api.upload(`/shows/${id}/upload-poster`,fd); }
                    if (backdropFile) { const fd=new FormData(); fd.append('backdropFile',backdropFile); await api.upload(`/shows/${id}/upload-backdrop`,fd); }
                    if (trailerFile)  { const fd=new FormData(); fd.append('trailerFile',trailerFile);   await api.upload(`/shows/${id}/upload-trailer`,fd); }
                    modal.close(); toast.success('Show updated successfully'); loadData();
                } catch(err) { toast.error(err.message); }
            });
        } catch(err) { toast.error(err.message); }
    }

    function bindEvents() {
        document.getElementById('admin-create-show-btn')?.addEventListener('click', showCreateModal);
        document.getElementById('show-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('show-search').value;
            state.includeDeleted = document.getElementById('show-include-deleted').checked;
            state.page = 1; loadData();
        });
        document.getElementById('show-search')?.addEventListener('keydown', e => { if(e.key==='Enter') document.getElementById('show-search-btn').click(); });
        document.querySelectorAll('.edit-show-btn').forEach(b => b.addEventListener('click', () => showEditModal(b.dataset.id)));
        document.querySelectorAll('.delete-show-btn').forEach(b => b.addEventListener('click', async () => {
            if (!await modal.confirm('Delete Show', `Soft-delete show "${b.closest('tr').querySelector('.font-medium').textContent}"?`, 'danger')) return;
            try { await api.del(`/shows/${b.dataset.id}`); toast.success('Show deleted'); loadData(); } catch(err) { toast.error(err.message); }
        }));
        document.querySelectorAll('.restore-show-btn').forEach(b => b.addEventListener('click', async () => {
            if (!await modal.confirm('Restore Show', `Restore deleted show "${b.closest('tr').querySelector('.font-medium').textContent}"?`, 'success')) return;
            try { await api.post(`/admin/shows/${b.dataset.id}/restore`, {}); toast.success('Show restored'); loadData(); } catch(err) { toast.error(err.message); }
        }));
        document.querySelectorAll('.upload-show-poster').forEach(b => b.addEventListener('click', () => {
            const id = b.dataset.id;
            modal.open('Upload Show Poster', `<div class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Select Poster Image File</label>
                    <input type="file" class="input-field" id="poster-upload-file" accept="image/*">
                </div>
                <button class="btn btn-primary w-full" id="do-poster-upload"><i class="fas fa-upload mr-1.5"></i> Upload Image</button>
            </div>`);
            document.getElementById('do-poster-upload')?.addEventListener('click', async () => {
                const f = document.getElementById('poster-upload-file').files[0];
                if (!f) { toast.warning('Please select an image file first'); return; }
                try { 
                    const fd=new FormData(); 
                    fd.append('posterFile',f); 
                    await api.upload(`/shows/${id}/upload-poster`,fd); 
                    modal.close(); 
                    toast.success('Poster uploaded successfully'); 
                    loadData(); 
                } catch(err) { toast.error(err.message); }
            });
        }));
    }

    return {
        render() { return '<div id="admin-content-root">' + Comp.pageLoader() + '</div>'; },
        async init() { await loadMeta(); loadData(); }
    };
})();