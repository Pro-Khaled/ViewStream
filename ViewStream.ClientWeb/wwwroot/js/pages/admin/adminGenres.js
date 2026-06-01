pages.adminGenres = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true, sortKey: 'id', sortDir: 'asc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/genres', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load genres: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Genres Management', 'Manage show genres', 
            '<button class="btn btn-primary btn-sm" id="add-genre-btn"><i class="fas fa-plus mr-1.5"></i> Add Genre</button>');
        
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4 font-body">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Genres</label>
                    <input class="input-field" id="genre-search" value="${toast.esc(state.search)}" placeholder="Search by genre name...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="genre-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="genre-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="genre-search-btn">Search</button>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-tags', 'No genres found'); }
        else {
            const rows = state.data.items.map(g => {
                let statusBadge = `<span class="badge badge-success">Active</span>`;
                if (g.isDeleted) statusBadge = `<span class="badge badge-danger">Deleted</span>`;

                return `<tr class="${g.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">#${g.id}</td>
                    <td class="font-bold text-vs-text text-lg">${toast.esc(g.name)}</td>
                    <td class="text-center font-bold text-vs-accent">${g.showCount ?? 0}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(g.createdAt)}</td>
                    <td>${statusBadge}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-genre-btn" data-id="${g.id}" data-name="${toast.esc(g.name)}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            ${!g.isDeleted ? `<button class="btn btn-ghost btn-sm text-vs-error delete-genre-btn" data-id="${g.id}" data-name="${toast.esc(g.name)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>` : ''}
                        </div>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'name', label: 'Genre Name' },
                    { key: 'showCount', label: 'Show Count' },
                    { key: 'createdAt', label: 'Created At' },
                    { key: 'isDeleted', label: 'Status' },
                    { key: '', label: '' }
                ],
                rows,
                'No genres found',
                {
                    tableId: 'genres-table',
                    sortKey: state.sortKey,
                    sortDir: state.sortDir,
                    onSort: (key, dir) => { if (key) { state.sortKey = key; state.sortDir = dir; loadData(); } }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('genre-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('genre-search').value.trim();
            state.includeDeleted = document.getElementById('genre-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('genre-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.getElementById('add-genre-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-genre-btn').forEach(btn => {
            btn.addEventListener('click', () => showEditModal(parseInt(btn.dataset.id), btn.dataset.name));
        });

        document.querySelectorAll('.delete-genre-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), btn.dataset.name));
        });
    }

    function showCreateModal() {
        modal.open('Add Genre',
            `<div class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Genre Name <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="new-genre-name" placeholder="e.g. Sci-Fi, Thriller, Anime">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-genre"><i class="fas fa-plus mr-1"></i> Create Genre</button>`
        );

        document.getElementById('save-new-genre')?.addEventListener('click', async () => {
            const name = document.getElementById('new-genre-name').value.trim();
            if (!name) { toast.warning('Genre name is required'); return; }

            try {
                await api.post('/genres', { name });
                toast.success('Genre created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showEditModal(id, currentName) {
        modal.open('Edit Genre',
            `<div class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Genre Name <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="edit-genre-name" value="${toast.esc(currentName)}">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-edit-genre"><i class="fas fa-save mr-1"></i> Save Changes</button>`
        );

        document.getElementById('save-edit-genre')?.addEventListener('click', async () => {
            const name = document.getElementById('edit-genre-name').value.trim();
            if (!name) { toast.warning('Genre name is required'); return; }

            try {
                await api.put(`/genres/${id}`, { name });
                toast.success('Genre updated successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(id, name) {
        modal.open('Delete Genre',
            `<p class="text-sm text-vs-dim">Are you sure you want to delete genre <strong class="text-vs-text">"${toast.esc(name)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-genre">Delete</button>`
        );

        document.getElementById('confirm-delete-genre')?.addEventListener('click', async () => {
            try {
                await api.del(`/genres/${id}`);
                toast.success('Genre deleted successfully');
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
