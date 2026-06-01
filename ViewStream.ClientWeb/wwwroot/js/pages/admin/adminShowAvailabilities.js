pages.adminShowAvailabilities = (() => {
    let state = { page: 1, search: '', data: null, loading: true };
    let sortKey = 'showId';
    let sortDir = 'desc';
    let showsList = [];
    let countriesList = [];

    async function loadResources() {
        try {
            const [showsRes, countriesRes] = await Promise.all([
                api.get('/shows', { pageSize: 200 }),
                api.get('/countries/all')
            ]);
            showsList = showsRes.items || [];
            countriesList = countriesRes || [];
        } catch (err) {
            console.error('Failed to load availability resources', err);
        }
    }

    async function loadData() {
        state.loading = true; render();
        if (showsList.length === 0 || countriesList.length === 0) {
            await loadResources();
        }
        try {
            state.data = await api.get('/admin/show-availabilities', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load show availabilities: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Content Availability & Licensing', '', '<button class="btn btn-primary btn-sm" id="add-availability-btn"><i class="fas fa-plus mr-1.5"></i> Add Availability</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="availability-search" value="${toast.esc(state.search)}" placeholder="Search by show title, country code, or country name...">
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="availability-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-globe', 'No availability records found'); }
        else {
            const rows = state.data.items.map(a => {
                const dates = (a.availableFrom || a.availableUntil) ? 
                    `<div class="text-xs text-vs-dim">
                        ${a.availableFrom ? 'From: ' + utils.formatDateShort(a.availableFrom) : ''}
                        ${a.availableUntil ? '<br>To: ' + utils.formatDateShort(a.availableUntil) : ''}
                    </div>` : '<span class="text-xs text-vs-muted">Always Available</span>';
                return `<tr data-show-id="${a.showId}" data-country-code="${toast.esc(a.countryCode)}">
                    <td class="font-bold text-vs-text max-w-[200px] truncate">${toast.esc(a.showTitle)}</td>
                    <td class="font-medium text-vs-accent uppercase">${toast.esc(a.countryCode)} - ${toast.esc(a.countryName || 'â€”')}</td>
                    <td>${dates}</td>
                    <td class="text-sm max-w-[200px] truncate text-vs-dim" title="${toast.esc(a.licensingNotes || '')}">${toast.esc(a.licensingNotes || 'â€”')}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-availability-btn" data-show-id="${a.showId}" data-country-code="${toast.esc(a.countryCode)}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm text-vs-error delete-availability-btn" data-show-id="${a.showId}" data-country-code="${toast.esc(a.countryCode)}" data-show-title="${toast.esc(a.showTitle)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'showTitle', label: 'Show' },
                    { key: 'countryCode', label: 'Country' },
                    { key: '', label: 'Availability Period' },
                    { key: 'licensingNotes', label: 'Licensing Notes' },
                    { key: '', label: '' }
                ],
                rows,
                'No availability records found',
                {
                    tableId: 'availabilities-table',
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
        document.getElementById('availability-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('availability-search').value.trim();
            state.page = 1; loadData();
        });
        document.getElementById('add-availability-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-availability-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showEditModal(parseInt(btn.dataset.showId), btn.dataset.countryCode);
            });
        });

        document.querySelectorAll('.delete-availability-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showDeleteModal(parseInt(btn.dataset.showId), btn.dataset.countryCode, btn.dataset.showTitle);
            });
        });
    }

    function showCreateModal() {
        const showsOpts = showsList.map(s => `<option value="${s.id}">${toast.esc(s.title)}</option>`).join('');
        const countriesOpts = countriesList.map(c => `<option value="${c.code}">${toast.esc(c.code)} - ${toast.esc(c.name)}</option>`).join('');

        modal.open('Add Show Availability',
            `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                <div class="form-group">
                    <label class="form-label">Show <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-avail-show-id">
                        <option value="">-- Select Show --</option>
                        ${showsOpts}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Country <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-avail-country">
                        <option value="">-- Select Country --</option>
                        ${countriesOpts}
                    </select>
                </div>
                <div class="grid grid-cols-2 gap-4">
                    <div class="form-group">
                        <label class="form-label">Available From</label>
                        <input type="date" class="input-field" id="new-avail-from">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Available Until</label>
                        <input type="date" class="input-field" id="new-avail-until">
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Licensing Notes</label>
                    <textarea class="input-field" id="new-avail-notes" rows="3" placeholder="e.g. Content licensed for 2 years..."></textarea>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-avail"><i class="fas fa-plus mr-1"></i> Add Availability</button>`
        );

        document.getElementById('save-new-avail')?.addEventListener('click', async () => {
            const showId = parseInt(document.getElementById('new-avail-show-id').value);
            const countryCode = document.getElementById('new-avail-country').value;
            const availableFrom = document.getElementById('new-avail-from').value || null;
            const availableUntil = document.getElementById('new-avail-until').value || null;
            const licensingNotes = document.getElementById('new-avail-notes').value.trim();

            if (!showId) { toast.warning('Please select a show'); return; }
            if (!countryCode) { toast.warning('Please select a country'); return; }

            try {
                await api.post(`/shows/${showId}/availabilities`, {
                    showId,
                    countryCode,
                    availableFrom,
                    availableUntil,
                    licensingNotes: licensingNotes || null
                });
                toast.success('Availability record created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    async function showEditModal(showId, countryCode) {
        modal.open('Edit Availability', Comp.pageLoader());
        try {
            const a = await api.get(`/shows/${showId}/availabilities/${countryCode}`);
            modal.open(`Edit Availability - ${toast.esc(a.countryCode)}`,
                `<div class="space-y-4">
                    <div class="form-group">
                        <label class="form-label">Show</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="Show ID #${showId}" readonly disabled>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Country</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(a.countryCode)}" readonly disabled>
                    </div>
                    <div class="grid grid-cols-2 gap-4">
                        <div class="form-group">
                            <label class="form-label">Available From</label>
                            <input type="date" class="input-field" id="edit-avail-from" value="${a.availableFrom || ''}">
                        </div>
                        <div class="form-group">
                            <label class="form-label">Available Until</label>
                            <input type="date" class="input-field" id="edit-avail-until" value="${a.availableUntil || ''}">
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Licensing Notes</label>
                        <textarea class="input-field" id="edit-avail-notes" rows="3">${toast.esc(a.licensingNotes || '')}</textarea>
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-edit-avail"><i class="fas fa-save mr-1"></i> Save Changes</button>`
            );

            document.getElementById('save-edit-avail')?.addEventListener('click', async () => {
                const availableFrom = document.getElementById('edit-avail-from').value || null;
                const availableUntil = document.getElementById('edit-avail-until').value || null;
                const licensingNotes = document.getElementById('edit-avail-notes').value.trim();

                try {
                    await api.put(`/shows/${showId}/availabilities/${countryCode}`, {
                        availableFrom,
                        availableUntil,
                        licensingNotes: licensingNotes || null
                    });
                    toast.success('Availability updated successfully');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) { toast.error('Failed to load details: ' + err.message); modal.close(); }
    }

    function showDeleteModal(showId, countryCode, showTitle) {
        modal.open('Delete Availability Record',
            `<p class="text-sm text-vs-dim">Are you sure you want to remove the availability of <strong class="text-vs-text">"${toast.esc(showTitle)}"</strong> in country <strong class="text-vs-text">${toast.esc(countryCode)}</strong>? This will restrict access for users in that region.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-avail">Remove Availability</button>`
        );

        document.getElementById('confirm-delete-avail')?.addEventListener('click', async () => {
            try {
                await api.del(`/shows/${showId}/availabilities/${countryCode}`);
                toast.success('Availability deleted successfully');
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