pages.adminPersonAwards = (() => {
    let state = { page: 1, search: '', data: null, loading: true };
    let sortKey = 'personId';
    let sortDir = 'desc';
    let personsList = [];
    let awardsList = [];

    async function loadResources() {
        try {
            const [personsRes, awardsRes] = await Promise.all([
                api.get('/persons/all'),
                api.get('/awards/all')
            ]);
            personsList = personsRes || [];
            awardsList = awardsRes || [];
        } catch (err) {
            console.error('Failed to load person award setup resources', err);
        }
    }

    async function loadData() {
        state.loading = true; render();
        if (personsList.length === 0 || awardsList.length === 0) {
            await loadResources();
        }
        try {
            state.data = await api.get('/admin/person-awards', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load person awards: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Person Award Associations', '', '<button class="btn btn-primary btn-sm" id="add-person-award-btn"><i class="fas fa-plus mr-1.5"></i> Assign Award</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="person-award-search" value="${toast.esc(state.search)}" placeholder="Search by person name, award name, or category...">
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="person-award-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-trophy', 'No person awards found'); }
        else {
            const rows = state.data.items.map(a => {
                const wonBadge = a.won ? `<span class="badge badge-success text-xs font-semibold">Won</span>` : `<span class="badge badge-secondary text-xs">Nominated</span>`;
                return `<tr data-person-id="${a.personId}" data-award-id="${a.awardId}">
                    <td class="font-bold text-vs-text max-w-[200px] truncate">${toast.esc(a.personName)}</td>
                    <td class="font-medium text-vs-accent">${toast.esc(a.awardName)}</td>
                    <td class="text-vs-dim">${toast.esc(a.category || 'â€”')}</td>
                    <td class="text-center">${wonBadge}</td>
                    <td class="text-vs-dim text-sm text-center">${a.awardYear || 'â€”'}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm text-vs-error delete-person-award-btn" data-person-id="${a.personId}" data-award-id="${a.awardId}" data-person-name="${toast.esc(a.personName)}" data-award-name="${toast.esc(a.awardName)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'personName', label: 'Person' },
                    { key: 'awardName', label: 'Award Name' },
                    { key: 'category', label: 'Category' },
                    { key: 'won', label: 'Result' },
                    { key: 'awardYear', label: 'Year' },
                    { key: '', label: '' }
                ],
                rows,
                'No person awards found',
                {
                    tableId: 'person-awards-table',
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
        document.getElementById('person-award-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('person-award-search').value.trim();
            state.page = 1; loadData();
        });
        document.getElementById('add-person-award-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.delete-person-award-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showDeleteModal(parseInt(btn.dataset.personId), parseInt(btn.dataset.awardId), btn.dataset.personName, btn.dataset.awardName);
            });
        });
    }

    function showCreateModal() {
        const personsOpts = personsList.map(p => `<option value="${p.id}">${toast.esc(p.name)}</option>`).join('');
        const awardsOpts = awardsList.map(a => `<option value="${a.id}">${toast.esc(a.name)} ${a.category ? '(' + toast.esc(a.category) + ')' : ''}</option>`).join('');

        modal.open('Assign Award to Person',
            `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                <div class="form-group">
                    <label class="form-label">Person <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-pa-person-id">
                        <option value="">-- Select Person --</option>
                        ${personsOpts}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Award <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-pa-award-id">
                        <option value="">-- Select Award --</option>
                        ${awardsOpts}
                    </select>
                </div>
                <div class="form-group flex items-center gap-2 py-2">
                    <input type="checkbox" id="new-pa-won" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent" checked>
                    <label for="new-pa-won" class="text-sm text-vs-dim cursor-pointer select-none">Person won this award (Uncheck if nominated only)</label>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-pa"><i class="fas fa-plus mr-1"></i> Assign Award</button>`
        );

        document.getElementById('save-new-pa')?.addEventListener('click', async () => {
            const personId = parseInt(document.getElementById('new-pa-person-id').value);
            const awardId = parseInt(document.getElementById('new-pa-award-id').value);
            const won = document.getElementById('new-pa-won').checked;

            if (!personId) { toast.warning('Please select a person'); return; }
            if (!awardId) { toast.warning('Please select an award'); return; }

            try {
                await api.post('/admin/person-awards', { personId, awardId, won });
                toast.success('Award assigned to person successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(personId, awardId, personName, awardName) {
        modal.open('Delete Person Award Assignment',
            `<p class="text-sm text-vs-dim">Are you sure you want to remove the award assignment of <strong class="text-vs-text">"${toast.esc(awardName)}"</strong> from <strong class="text-vs-text">"${toast.esc(personName)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-pa">Delete Assignment</button>`
        );

        document.getElementById('confirm-delete-pa')?.addEventListener('click', async () => {
            try {
                await api.del(`/persons/${personId}/awards/${awardId}`);
                toast.success('Award assignment deleted successfully');
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