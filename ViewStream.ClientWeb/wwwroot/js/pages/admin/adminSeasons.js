pages.adminSeasons = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true };
    let sortKey = 'id';
    let sortDir = 'desc';
    let showsList = [];

    async function loadShows() {
        try {
            const res = await api.get('/shows', { pageSize: 200 }) || {};
            showsList = res.items || [];
        } catch (err) {
            console.error('Failed to load shows for dropdown', err);
        }
    }

    async function loadData() {
        state.loading = true; render();
        if (showsList.length === 0) {
            await loadShows();
        }
        try {
            state.data = await api.get('/admin/seasons', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load seasons: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Season Management', '', '<button class="btn btn-primary btn-sm" id="add-season-btn"><i class="fas fa-plus mr-1.5"></i> Add Season</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="seasons-search" value="${toast.esc(state.search)}" placeholder="Search by show title or season...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="seasons-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="seasons-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="seasons-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-list-ol', 'No seasons found'); }
        else {
            const rows = state.data.items.map(s => {
                let statusBadge = `<span class="badge badge-success">Active</span>`;
                if (s.isDeleted) {
                    statusBadge = '<span class="badge badge-danger">Deleted</span>';
                }
                return `<tr data-season-id="${s.id}" class="${s.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">${s.id}</td>
                    <td class="font-bold text-vs-text">${toast.esc(s.showTitle)}</td>
                    <td class="font-medium text-vs-accent">Season ${s.seasonNumber}</td>
                    <td>${toast.esc(s.title || 'â€”')}</td>
                    <td><span class="badge badge-info">${s.episodeCount} episodes</span></td>
                    <td>${statusBadge}</td>
                    <td>${s.releaseDate ? utils.formatDateShort(s.releaseDate) : 'â€”'}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-season-btn" data-id="${s.id}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            ${s.isDeleted ? 
                                `<button class="btn btn-ghost btn-sm text-vs-success restore-season-btn" data-id="${s.id}" data-title="Season ${s.seasonNumber} of ${toast.esc(s.showTitle)}" title="Restore"><i class="fas fa-trash-restore text-xs"></i></button>` : 
                                `<button class="btn btn-ghost btn-sm text-vs-error delete-season-btn" data-id="${s.id}" data-title="Season ${s.seasonNumber} of ${toast.esc(s.showTitle)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>`
                            }
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'showTitle', label: 'Show' },
                    { key: 'seasonNumber', label: 'Season' },
                    { key: 'title', label: 'Season Title' },
                    { key: 'episodeCount', label: 'Episodes' },
                    { key: 'isDeleted', label: 'Status' },
                    { key: 'releaseDate', label: 'Release Date' },
                    { key: '', label: '' }
                ],
                rows,
                'No seasons found',
                {
                    tableId: 'seasons-table',
                    sortKey,
                    sortDir,
                    onSort: (key, dir) => { if (key) { sortKey = key; sortDir = dir; loadData(); } }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('seasons-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('seasons-search').value.trim();
            state.includeDeleted = document.getElementById('seasons-include-deleted').checked;
            state.page = 1; loadData();
        });
        document.getElementById('seasons-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });
        document.getElementById('add-season-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-season-btn').forEach(btn => {
            btn.addEventListener('click', () => showEditModal(parseInt(btn.dataset.id)));
        });

        document.querySelectorAll('.delete-season-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), btn.dataset.title));
        });

        document.querySelectorAll('.restore-season-btn').forEach(btn => {
            btn.addEventListener('click', () => showRestoreModal(parseInt(btn.dataset.id), btn.dataset.title));
        });
    }

    function showCreateModal() {
        const options = showsList.map(show => `<option value="${show.id}">${toast.esc(show.title)}</option>`).join('');
        modal.open('Add Season',
            `<div class="space-y-4">
                <div class="form-group">
                    <label class="form-label">Show <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-season-show-id">
                        <option value="">-- Select Show --</option>
                        ${options}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Season Number <span class="text-vs-error">*</span></label>
                    <input type="number" class="input-field" id="new-season-number" min="1" value="1">
                </div>
                <div class="form-group">
                    <label class="form-label">Season Title</label>
                    <input type="text" class="input-field" id="new-season-title" placeholder="e.g. Season 1 or Winter Edition">
                </div>
                <div class="form-group">
                    <label class="form-label">Description</label>
                    <textarea class="input-field" id="new-season-desc" rows="3" placeholder="Season summary..."></textarea>
                </div>
                <div class="form-group">
                    <label class="form-label">Release Date</label>
                    <input type="date" class="input-field" id="new-season-release-date">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-season"><i class="fas fa-plus mr-1"></i> Create</button>`
        );

        document.getElementById('save-new-season')?.addEventListener('click', async () => {
            const showId = parseInt(document.getElementById('new-season-show-id').value);
            const seasonNumber = parseInt(document.getElementById('new-season-number').value);
            const title = document.getElementById('new-season-title').value.trim();
            const description = document.getElementById('new-season-desc').value.trim();
            const releaseDate = document.getElementById('new-season-release-date').value || null;

            if (!showId) { toast.warning('Please select a show'); return; }
            if (!seasonNumber || seasonNumber < 1) { toast.warning('Season number must be 1 or greater'); return; }

            try {
                await api.post('/seasons', {
                    showId,
                    seasonNumber,
                    title: title || null,
                    description: description || null,
                    releaseDate
                });
                toast.success('Season created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    async function showEditModal(id) {
        modal.open('Edit Season', Comp.pageLoader());
        try {
            const s = await api.get(`/seasons/${id}`);
            modal.open(`Edit Season: ${s.seasonNumber} - ${toast.esc(s.showTitle)}`,
                `<div class="space-y-4">
                    <div class="form-group">
                        <label class="form-label">Show</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(s.showTitle)}" readonly disabled>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Season Number <span class="text-vs-error">*</span></label>
                        <input type="number" class="input-field" id="edit-season-number" min="1" value="${s.seasonNumber}">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Season Title</label>
                        <input type="text" class="input-field" id="edit-season-title" value="${toast.esc(s.title || '')}">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Description</label>
                        <textarea class="input-field" id="edit-season-desc" rows="3">${toast.esc(s.description || '')}</textarea>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Release Date</label>
                        <input type="date" class="input-field" id="edit-season-release-date" value="${s.releaseDate || ''}">
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-edit-season"><i class="fas fa-save mr-1"></i> Save Changes</button>`
            );

            document.getElementById('save-edit-season')?.addEventListener('click', async () => {
                const seasonNumber = parseInt(document.getElementById('edit-season-number').value);
                const title = document.getElementById('edit-season-title').value.trim();
                const description = document.getElementById('edit-season-desc').value.trim();
                const releaseDate = document.getElementById('edit-season-release-date').value || null;

                if (!seasonNumber || seasonNumber < 1) { toast.warning('Season number must be 1 or greater'); return; }

                try {
                    await api.put(`/seasons/${id}`, {
                        seasonNumber,
                        title: title || null,
                        description: description || null,
                        releaseDate
                    });
                    toast.success('Season updated successfully');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) { toast.error('Failed to load season details: ' + err.message); modal.close(); }
    }

    function showDeleteModal(id, title) {
        modal.open('Delete Season',
            `<p class="text-sm text-vs-dim">Are you sure you want to delete <strong class="text-vs-text">"${toast.esc(title)}"</strong>? This will soft-delete all episodes inside this season.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-season">Delete</button>`
        );

        document.getElementById('confirm-delete-season')?.addEventListener('click', async () => {
            try {
                await api.del(`/seasons/${id}`);
                toast.success('Season soft-deleted successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showRestoreModal(id, title) {
        modal.open('Restore Season',
            `<p class="text-sm text-vs-dim">Are you sure you want to restore <strong class="text-vs-text">"${toast.esc(title)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-success" id="confirm-restore-season">Restore</button>`
        );

        document.getElementById('confirm-restore-season')?.addEventListener('click', async () => {
            try {
                await api.post(`/admin/seasons/${id}/restore`);
                toast.success('Season restored successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();