pages.adminShowAwards = (() => {
    let state = { page: 1, search: '', data: null, loading: true };
    let sortKey = 'showId';
    let sortDir = 'desc';
    let showsList = [];
    let awardsList = [];

    async function loadResources() {
        try {
            const [showsRes, awardsRes] = await Promise.all([
                api.get('/shows', { pageSize: 200 }),
                api.get('/awards/all')
            ]);
            showsList = showsRes.items || [];
            awardsList = awardsRes || [];
        } catch (err) {
            console.error('Failed to load show award setup resources', err);
        }
    }

    async function loadData() {
        state.loading = true; render();
        if (showsList.length === 0 || awardsList.length === 0) {
            await loadResources();
        }
        try {
            state.data = await api.get('/admin/show-awards', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load show awards: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Show Award Associations', '', '<button class="btn btn-primary btn-sm" id="add-show-award-btn"><i class="fas fa-plus mr-1.5"></i> Assign Award</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="show-award-search" value="${toast.esc(state.search)}" placeholder="Search by show title, award name, or category...">
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="show-award-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-trophy', 'No show awards found'); }
        else {
            const rows = state.data.items.map(a => {
                const wonBadge = a.won ? `<span class="badge badge-success text-xs font-semibold">Won</span>` : `<span class="badge badge-secondary text-xs">Nominated</span>`;
                return `<tr data-show-id="${a.showId}" data-award-id="${a.awardId}">
                    <td class="font-bold text-vs-text max-w-[200px] truncate">${toast.esc(a.showTitle)}</td>
                    <td class="font-medium text-vs-accent">${toast.esc(a.awardName)}</td>
                    <td class="text-vs-dim">${toast.esc(a.category || 'â€”')}</td>
                    <td class="text-center">${wonBadge}</td>
                    <td class="text-vs-dim text-sm text-center">${a.awardYear || 'â€”'}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm text-vs-error delete-show-award-btn" data-show-id="${a.showId}" data-award-id="${a.awardId}" data-show-title="${toast.esc(a.showTitle)}" data-award-name="${toast.esc(a.awardName)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'showTitle', label: 'Show' },
                    { key: 'awardName', label: 'Award Name' },
                    { key: 'category', label: 'Category' },
                    { key: 'won', label: 'Result' },
                    { key: 'awardYear', label: 'Year' },
                    { key: '', label: '' }
                ],
                rows,
                'No show awards found',
                {
                    tableId: 'show-awards-table',
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
        document.getElementById('show-award-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('show-award-search').value.trim();
            state.page = 1; loadData();
        });
        document.getElementById('add-show-award-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.delete-show-award-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                showDeleteModal(parseInt(btn.dataset.showId), parseInt(btn.dataset.awardId), btn.dataset.showTitle, btn.dataset.awardName);
            });
        });
    }

    function showCreateModal() {
        const showsOpts = showsList.map(s => `<option value="${s.id}">${toast.esc(s.title)}</option>`).join('');
        const awardsOpts = awardsList.map(a => `<option value="${a.id}">${toast.esc(a.name)} ${a.category ? '(' + toast.esc(a.category) + ')' : ''}</option>`).join('');

        modal.open('Assign Award to Show',
            `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                <div class="form-group">
                    <label class="form-label">Show <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-sa-show-id">
                        <option value="">-- Select Show --</option>
                        ${showsOpts}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Award <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-sa-award-id">
                        <option value="">-- Select Award --</option>
                        ${awardsOpts}
                    </select>
                </div>
                <div class="form-group flex items-center gap-2 py-2">
                    <input type="checkbox" id="new-sa-won" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent" checked>
                    <label for="new-sa-won" class="text-sm text-vs-dim cursor-pointer select-none">Show won this award (Uncheck if nominated only)</label>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-sa"><i class="fas fa-plus mr-1"></i> Assign Award</button>`
        );

        document.getElementById('save-new-sa')?.addEventListener('click', async () => {
            const showId = parseInt(document.getElementById('new-sa-show-id').value);
            const awardId = parseInt(document.getElementById('new-sa-award-id').value);
            const won = document.getElementById('new-sa-won').checked;

            if (!showId) { toast.warning('Please select a show'); return; }
            if (!awardId) { toast.warning('Please select an award'); return; }

            try {
                await api.post('/admin/show-awards', { showId, awardId, won });
                toast.success('Award assigned successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(showId, awardId, showTitle, awardName) {
        modal.open('Delete Show Award Assignment',
            `<p class="text-sm text-vs-dim">Are you sure you want to remove the award assignment of <strong class="text-vs-text">"${toast.esc(awardName)}"</strong> from show <strong class="text-vs-text">"${toast.esc(showTitle)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-sa">Delete Assignment</button>`
        );

        document.getElementById('confirm-delete-sa')?.addEventListener('click', async () => {
            try {
                await api.del(`/shows/${showId}/awards/${awardId}`);
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