pages.adminCountries = (() => {
    let state = { page: 1, search: '', continent: '', data: null, loading: true };
    let sortKey = 'code';
    let sortDir = 'asc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/countries', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load countries: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Country Management', '', '<button class="btn btn-primary btn-sm" id="add-country-btn"><i class="fas fa-plus mr-1.5"></i> Add Country</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="country-search" value="${toast.esc(state.search)}" placeholder="Search by code or name...">
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="country-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-globe', 'No countries found'); }
        else {
            const rows = state.data.items.map(country => {
                return `<tr>
                    <td class="font-bold text-vs-accent">${toast.esc(country.code)}</td>
                    <td class="font-medium">${toast.esc(country.name)}</td>
                    <td>${toast.esc(country.continent || 'â€”')}</td>
                    <td><span class="badge badge-info">${country.availabilityCount} shows</span></td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-country-btn" data-code="${toast.esc(country.code)}" data-name="${toast.esc(country.name)}" data-continent="${toast.esc(country.continent || '')}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm text-vs-error delete-country-btn" data-code="${toast.esc(country.code)}" data-name="${toast.esc(country.name)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'code', label: 'ISO Code' },
                    { key: 'name', label: 'Country Name' },
                    { key: 'continent', label: 'Continent' },
                    { key: 'availabilityCount', label: 'Available Content' },
                    { key: '', label: '' }
                ],
                rows,
                'No countries found',
                {
                    tableId: 'countries-table',
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
        document.getElementById('country-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('country-search').value.trim();
            state.page = 1; loadData();
        });
        document.getElementById('add-country-btn')?.addEventListener('click', showCreateModal);
        
        document.querySelectorAll('.edit-country-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showEditModal({
                    code: btn.dataset.code,
                    name: btn.dataset.name,
                    continent: btn.dataset.continent
                });
            });
        });

        document.querySelectorAll('.delete-country-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showDeleteModal(btn.dataset.code, btn.dataset.name);
            });
        });
    }

    function showCreateModal() {
        modal.open('Add Country', 
            `<div class="space-y-4">
                <div class="form-group">
                    <label class="form-label">ISO Code (2 characters) <span class="text-vs-error">*</span></label>
                    <input class="input-field" id="new-country-code" maxlength="2" placeholder="e.g. US">
                </div>
                <div class="form-group">
                    <label class="form-label">Country Name <span class="text-vs-error">*</span></label>
                    <input class="input-field" id="new-country-name" placeholder="e.g. United States">
                </div>
                <div class="form-group">
                    <label class="form-label">Continent</label>
                    <input class="input-field" id="new-country-continent" placeholder="e.g. North America">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-country"><i class="fas fa-plus mr-1"></i> Create</button>`
        );

        document.getElementById('save-new-country')?.addEventListener('click', async () => {
            const code = document.getElementById('new-country-code').value.trim().toUpperCase();
            const name = document.getElementById('new-country-name').value.trim();
            const continent = document.getElementById('new-country-continent').value.trim();

            if (code.length !== 2) { toast.warning('ISO Code must be exactly 2 characters'); return; }
            if (!name) { toast.warning('Country Name is required'); return; }

            try {
                await api.post('/countries', { code, name, continent: continent || null });
                toast.success('Country created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showEditModal(country) {
        modal.open(`Edit Country: ${country.name}`, 
            `<div class="space-y-4">
                <div class="form-group">
                    <label class="form-label">ISO Code</label>
                    <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(country.code)}" readonly disabled>
                </div>
                <div class="form-group">
                    <label class="form-label">Country Name <span class="text-vs-error">*</span></label>
                    <input class="input-field" id="edit-country-name" value="${toast.esc(country.name)}">
                </div>
                <div class="form-group">
                    <label class="form-label">Continent</label>
                    <input class="input-field" id="edit-country-continent" value="${toast.esc(country.continent)}">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-edit-country"><i class="fas fa-save mr-1"></i> Save Changes</button>`
        );

        document.getElementById('save-edit-country')?.addEventListener('click', async () => {
            const name = document.getElementById('edit-country-name').value.trim();
            const continent = document.getElementById('edit-country-continent').value.trim();

            if (!name) { toast.warning('Country Name is required'); return; }

            try {
                await api.put(`/countries/${country.code}`, { name, continent: continent || null });
                toast.success('Country updated successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(code, name) {
        modal.open('Delete Country',
            `<p class="text-sm text-vs-dim">Are you sure you want to delete <strong class="text-vs-text">"${toast.esc(name)}" (${toast.esc(code)})</strong>? This will delete all associated show availabilities.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-country">Delete</button>`
        );

        document.getElementById('confirm-delete-country')?.addEventListener('click', async () => {
            try {
                await api.del(`/countries/${code}`);
                toast.success('Country deleted successfully');
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