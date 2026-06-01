pages.adminContentTags = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true, sortKey: 'id', sortDir: 'asc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/contenttags', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load content tags: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Content Tags Management', 'Manage content metadata tags', 
            '<button class="btn btn-primary btn-sm" id="add-tag-btn"><i class="fas fa-plus mr-1.5"></i> Add Content Tag</button>');
        
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4 font-body">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Tags</label>
                    <input class="input-field" id="tag-search" value="${toast.esc(state.search)}" placeholder="Search by tag name or category...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="tag-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="tag-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="tag-search-btn">Search</button>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-tags', 'No content tags found'); }
        else {
            const rows = state.data.items.map(t => {
                let statusBadge = `<span class="badge badge-success">Active</span>`;
                if (t.isDeleted) statusBadge = `<span class="badge badge-danger">Deleted</span>`;

                return `<tr class="${t.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">#${t.id}</td>
                    <td class="font-bold text-vs-text">${toast.esc(t.name)}</td>
                    <td><span class="badge bg-vs-accent/10 text-vs-accent font-semibold">${toast.esc(t.category || 'General')}</span></td>
                    <td class="text-center font-bold text-vs-info">${t.showCount ?? 0}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(t.createdAt)}</td>
                    <td>${statusBadge}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-tag-btn" data-id="${t.id}" data-name="${toast.esc(t.name)}" data-category="${toast.esc(t.category || '')}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            ${!t.isDeleted ? `<button class="btn btn-ghost btn-sm text-vs-error delete-tag-btn" data-id="${t.id}" data-name="${toast.esc(t.name)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>` : ''}
                        </div>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'name', label: 'Tag Name' },
                    { key: 'category', label: 'Category' },
                    { key: 'showCount', label: 'Show Count' },
                    { key: 'createdAt', label: 'Created At' },
                    { key: 'isDeleted', label: 'Status' },
                    { key: '', label: '' }
                ],
                rows,
                'No content tags found',
                {
                    tableId: 'content-tags-table',
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
        document.getElementById('tag-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('tag-search').value.trim();
            state.includeDeleted = document.getElementById('tag-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('tag-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.getElementById('add-tag-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-tag-btn').forEach(btn => {
            btn.addEventListener('click', () => showEditModal(parseInt(btn.dataset.id), btn.dataset.name, btn.dataset.category));
        });

        document.querySelectorAll('.delete-tag-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), btn.dataset.name));
        });
    }

    function showCreateModal() {
        modal.open('Add Content Tag',
            `<div class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Tag Name <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="new-tag-name" placeholder="e.g. Action-Packed, Slow-Burn">
                </div>
                <div class="form-group">
                    <label class="form-label">Category</label>
                    <input type="text" class="input-field" id="new-tag-category" placeholder="e.g. Mood, Theme, Pace">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-tag"><i class="fas fa-plus mr-1"></i> Create Tag</button>`
        );

        document.getElementById('save-new-tag')?.addEventListener('click', async () => {
            const name = document.getElementById('new-tag-name').value.trim();
            const category = document.getElementById('new-tag-category').value.trim();

            if (!name) { toast.warning('Tag name is required'); return; }

            try {
                await api.post('/contenttags', { name, category: category || null });
                toast.success('Content tag created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showEditModal(id, currentName, currentCategory) {
        modal.open('Edit Content Tag',
            `<div class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Tag Name <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="edit-tag-name" value="${toast.esc(currentName)}">
                </div>
                <div class="form-group">
                    <label class="form-label">Category</label>
                    <input type="text" class="input-field" id="edit-tag-category" value="${toast.esc(currentCategory)}">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-edit-tag"><i class="fas fa-save mr-1"></i> Save Changes</button>`
        );

        document.getElementById('save-edit-tag')?.addEventListener('click', async () => {
            const name = document.getElementById('edit-tag-name').value.trim();
            const category = document.getElementById('edit-tag-category').value.trim();

            if (!name) { toast.warning('Tag name is required'); return; }

            try {
                await api.put(`/contenttags/${id}`, { name, category: category || null });
                toast.success('Content tag updated successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(id, name) {
        modal.open('Delete Content Tag',
            `<p class="text-sm text-vs-dim">Are you sure you want to delete content tag <strong class="text-vs-text">"${toast.esc(name)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-tag">Delete</button>`
        );

        document.getElementById('confirm-delete-tag')?.addEventListener('click', async () => {
            try {
                await api.del(`/contenttags/${id}`);
                toast.success('Content tag deleted successfully');
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
