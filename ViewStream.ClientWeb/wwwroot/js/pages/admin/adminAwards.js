pages.adminAwards = (() => {
    let state = { page: 1, search: '', year: '', data: null, loading: true };
    let sortKey = 'id';
    let sortDir = 'desc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/awards', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load awards: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Award Management', '', '<button class="btn btn-primary btn-sm" id="add-award-btn"><i class="fas fa-plus mr-1.5"></i> Add Award</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="award-search" value="${toast.esc(state.search)}" placeholder="Search by award name or category...">
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="award-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-trophy', 'No awards found'); }
        else {
            const rows = state.data.items.map(a => {
                return `<tr>
                    <td class="text-muted">${a.id}</td>
                    <td class="font-bold text-vs-text">${toast.esc(a.name)}</td>
                    <td class="font-medium text-vs-accent">${toast.esc(a.category || 'â€”')}</td>
                    <td class="text-vs-dim">${a.year || 'â€”'}</td>
                    <td>
                        <span class="badge badge-info">${a.showAwardCount} shows</span>
                        <span class="badge badge-primary ml-1">${a.personAwardCount} persons</span>
                    </td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-award-btn" data-id="${a.id}" data-name="${toast.esc(a.name)}" data-category="${toast.esc(a.category || '')}" data-year="${a.year || ''}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm text-vs-error delete-award-btn" data-id="${a.id}" data-name="${toast.esc(a.name)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'name', label: 'Name' },
                    { key: 'category', label: 'Category' },
                    { key: 'year', label: 'Year' },
                    { key: '', label: 'Associations' },
                    { key: '', label: '' }
                ],
                rows,
                'No awards found',
                {
                    tableId: 'awards-table',
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
        document.getElementById('award-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('award-search').value.trim();
            state.page = 1; loadData();
        });
        document.getElementById('add-award-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-award-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showEditModal({
                    id: parseInt(btn.dataset.id),
                    name: btn.dataset.name,
                    category: btn.dataset.category,
                    year: parseInt(btn.dataset.year) || null
                });
            });
        });

        document.querySelectorAll('.delete-award-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showDeleteModal(parseInt(btn.dataset.id), btn.dataset.name);
            });
        });
    }

    function showCreateModal() {
        modal.open('Add Award',
            `<div class="space-y-4">
                <div class="form-group">
                    <label class="form-label">Award Name <span class="text-vs-error">*</span></label>
                    <input class="input-field" id="new-award-name" placeholder="e.g. Academy Award">
                </div>
                <div class="form-group">
                    <label class="form-label">Category</label>
                    <input class="input-field" id="new-award-category" placeholder="e.g. Best Picture">
                </div>
                <div class="form-group">
                    <label class="form-label">Year</label>
                    <input type="number" class="input-field" id="new-award-year" placeholder="e.g. 2026" min="1900" max="2100">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-award"><i class="fas fa-plus mr-1"></i> Create</button>`
        );

        document.getElementById('save-new-award')?.addEventListener('click', async () => {
            const name = document.getElementById('new-award-name').value.trim();
            const category = document.getElementById('new-award-category').value.trim();
            const yearVal = parseInt(document.getElementById('new-award-year').value);

            if (!name) { toast.warning('Award Name is required'); return; }

            try {
                await api.post('/awards', {
                    name,
                    category: category || null,
                    year: yearVal || null
                });
                toast.success('Award created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showEditModal(award) {
        modal.open(`Edit Award: ${award.name}`,
            `<div class="space-y-4">
                <div class="form-group">
                    <label class="form-label">Award Name <span class="text-vs-error">*</span></label>
                    <input class="input-field" id="edit-award-name" value="${toast.esc(award.name)}">
                </div>
                <div class="form-group">
                    <label class="form-label">Category</label>
                    <input class="input-field" id="edit-award-category" value="${toast.esc(award.category)}">
                </div>
                <div class="form-group">
                    <label class="form-label">Year</label>
                    <input type="number" class="input-field" id="edit-award-year" value="${award.year || ''}">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-edit-award"><i class="fas fa-save mr-1"></i> Save Changes</button>`
        );

        document.getElementById('save-edit-award')?.addEventListener('click', async () => {
            const name = document.getElementById('edit-award-name').value.trim();
            const category = document.getElementById('edit-award-category').value.trim();
            const yearVal = parseInt(document.getElementById('edit-award-year').value);

            if (!name) { toast.warning('Award Name is required'); return; }

            try {
                await api.put(`/awards/${award.id}`, {
                    name,
                    category: category || null,
                    year: yearVal || null
                });
                toast.success('Award updated successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(id, name) {
        modal.open('Delete Award',
            `<p class="text-sm text-vs-dim">Are you sure you want to delete <strong class="text-vs-text">"${toast.esc(name)}"</strong>? This will permanently delete the award and remove it from all shows and people.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-award">Delete</button>`
        );

        document.getElementById('confirm-delete-award')?.addEventListener('click', async () => {
            try {
                await api.del(`/awards/${id}`);
                toast.success('Award deleted successfully');
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