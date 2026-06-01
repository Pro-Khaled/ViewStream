pages.adminCredits = (() => {
    let state = { page: 1, search: '', data: null, loading: true };
    let sortKey = 'id';
    let sortDir = 'desc';
    let showsList = [];
    let personsList = [];

    async function loadResources() {
        try {
            const [showsRes, personsRes] = await Promise.all([
                api.get('/shows', { pageSize: 200 }),
                api.get('/persons/all')
            ]);
            showsList = showsRes.items || [];
            personsList = personsRes || [];
        } catch (err) {
            console.error('Failed to load credit setup resources', err);
        }
    }

    async function loadData() {
        state.loading = true; render();
        if (showsList.length === 0 || personsList.length === 0) {
            await loadResources();
        }
        try {
            state.data = await api.get('/admin/credits', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load credits: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Credit Assignment', '', '<button class="btn btn-primary btn-sm" id="add-credit-btn"><i class="fas fa-plus mr-1.5"></i> Add Credit</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="credit-search" value="${toast.esc(state.search)}" placeholder="Search by person name, role, character, or title...">
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="credit-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-id-badge', 'No credits found'); }
        else {
            const rows = state.data.items.map(t => {
                const charName = t.characterName ? `<span class="text-xs text-vs-dim">as <strong>${toast.esc(t.characterName)}</strong></span>` : '';
                const targetBadge = `<span class="badge text-xs ${t.targetType === 'Show' ? 'badge-primary' : t.targetType === 'Season' ? 'badge-info' : 'badge-warning'}">${t.targetType}</span>`;
                const photo = t.personPhotoUrl ? `<img src="${toast.esc(t.personPhotoUrl)}" class="w-8 h-8 rounded-full object-cover border border-vs-border">` : `<div class="w-8 h-8 rounded-full bg-vs-card border border-vs-border flex items-center justify-center text-vs-muted text-xs"><i class="fas fa-user"></i></div>`;
                return `<tr data-credit-id="${t.id}">
                    <td class="text-muted">${t.id}</td>
                    <td>
                        <div class="flex items-center gap-2">
                            ${photo}
                            <span class="font-medium text-vs-text">${toast.esc(t.personName || 'Person #' + t.personId)}</span>
                        </div>
                    </td>
                    <td>
                        <div class="flex flex-col">
                            <span class="font-medium text-vs-accent">${toast.esc(t.role)}</span>
                            ${charName}
                        </div>
                    </td>
                    <td>${targetBadge}</td>
                    <td class="font-bold text-vs-dim max-w-[250px] truncate">${toast.esc(t.targetTitle || 'â€”')}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-credit-btn" data-id="${t.id}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm text-vs-error delete-credit-btn" data-id="${t.id}" data-name="${toast.esc(t.personName)} (${toast.esc(t.role)})" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'personName', label: 'Person' },
                    { key: 'role', label: 'Role / Character' },
                    { key: 'targetType', label: 'Target Type' },
                    { key: 'targetTitle', label: 'Assigned Content' },
                    { key: '', label: '' }
                ],
                rows,
                'No credits found',
                {
                    tableId: 'credits-table',
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
        document.getElementById('credit-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('credit-search').value.trim();
            state.page = 1; loadData();
        });
        document.getElementById('add-credit-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-credit-btn').forEach(btn => {
            btn.addEventListener('click', () => showEditModal(parseInt(btn.dataset.id)));
        });

        document.querySelectorAll('.delete-credit-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), btn.dataset.name));
        });
    }

    function showCreateModal() {
        const personsOpts = personsList.map(p => `<option value="${p.id}">${toast.esc(p.name)}</option>`).join('');
        const showsOpts = showsList.map(s => `<option value="${s.id}">${toast.esc(s.title)}</option>`).join('');

        modal.open('Add Credit',
            `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                <div class="form-group">
                    <label class="form-label">Person <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-credit-person-id">
                        <option value="">-- Select Person --</option>
                        ${personsOpts}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Target Type <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-credit-target-type">
                        <option value="Show">Show</option>
                        <option value="Season">Season</option>
                        <option value="Episode">Episode</option>
                    </select>
                </div>

                <!-- Show Select (Required for all types as initial layer) -->
                <div class="form-group" id="group-show-select">
                    <label class="form-label">Show <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-credit-show-id">
                        <option value="">-- Select Show --</option>
                        ${showsOpts}
                    </select>
                </div>

                <!-- Season Select -->
                <div class="form-group hidden" id="group-season-select">
                    <label class="form-label">Season <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-credit-season-id">
                        <option value="">-- Select Season --</option>
                    </select>
                </div>

                <!-- Episode Select -->
                <div class="form-group hidden" id="group-episode-select">
                    <label class="form-label">Episode <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-credit-episode-id">
                        <option value="">-- Select Episode --</option>
                    </select>
                </div>

                <div class="grid grid-cols-2 gap-4">
                    <div class="form-group">
                        <label class="form-label">Role <span class="text-vs-error">*</span></label>
                        <input type="text" class="input-field" id="new-credit-role" placeholder="e.g. Actor, Director, Writer">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Character Name (Actor only)</label>
                        <input type="text" class="input-field" id="new-credit-character" placeholder="e.g. Jon Snow">
                    </div>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-credit"><i class="fas fa-plus mr-1"></i> Add Credit</button>`
        );

        const typeSelect = document.getElementById('new-credit-target-type');
        const showSelect = document.getElementById('new-credit-show-id');
        const seasonSelect = document.getElementById('new-credit-season-id');
        const episodeSelect = document.getElementById('new-credit-episode-id');

        const gSeason = document.getElementById('group-season-select');
        const gEpisode = document.getElementById('group-episode-select');

        function updateFieldsVisibility() {
            const t = typeSelect.value;
            if (t === 'Show') {
                gSeason.classList.add('hidden');
                gEpisode.classList.add('hidden');
            } else if (t === 'Season') {
                gSeason.classList.remove('hidden');
                gEpisode.classList.add('hidden');
            } else if (t === 'Episode') {
                gSeason.classList.remove('hidden');
                gEpisode.classList.remove('hidden');
            }
        }

        typeSelect?.addEventListener('change', updateFieldsVisibility);

        showSelect?.addEventListener('change', async () => {
            const showId = showSelect.value;
            seasonSelect.innerHTML = '<option value="">-- Select Season --</option>';
            episodeSelect.innerHTML = '<option value="">-- Select Episode --</option>';
            if (!showId) return;

            try {
                const seasons = await api.get(`/shows/${showId}/seasons`) || [];
                let optHtml = '<option value="">-- Select Season --</option>';
                seasons.forEach(s => {
                    optHtml += `<option value="${s.id}">Season ${s.seasonNumber} ${s.title ? '- ' + s.title : ''}</option>`;
                });
                seasonSelect.innerHTML = optHtml;
            } catch (err) { toast.error('Failed to load seasons'); }
        });

        seasonSelect?.addEventListener('change', async () => {
            const seasonId = seasonSelect.value;
            episodeSelect.innerHTML = '<option value="">-- Select Episode --</option>';
            if (!seasonId) return;

            try {
                const episodes = await api.get(`/seasons/${seasonId}/episodes`) || [];
                let optHtml = '<option value="">-- Select Episode --</option>';
                episodes.forEach(e => {
                    optHtml += `<option value="${e.id}">Episode ${e.episodeNumber} - ${e.title}</option>`;
                });
                episodeSelect.innerHTML = optHtml;
            } catch (err) { toast.error('Failed to load episodes'); }
        });

        document.getElementById('save-new-credit')?.addEventListener('click', async () => {
            const personId = parseInt(document.getElementById('new-credit-person-id').value);
            const targetType = typeSelect.value;
            const showId = parseInt(showSelect.value) || null;
            const seasonId = parseInt(seasonSelect.value) || null;
            const episodeId = parseInt(episodeSelect.value) || null;
            const role = document.getElementById('new-credit-role').value.trim();
            const characterName = document.getElementById('new-credit-character').value.trim();

            if (!personId) { toast.warning('Please select a person'); return; }
            if (!role) { toast.warning('Role is required'); return; }

            // Validate targets
            const payload = { personId, role, characterName: characterName || null };
            if (targetType === 'Show') {
                if (!showId) { toast.warning('Please select a show'); return; }
                payload.showId = showId;
            } else if (targetType === 'Season') {
                if (!seasonId) { toast.warning('Please select a season'); return; }
                payload.seasonId = seasonId;
            } else if (targetType === 'Episode') {
                if (!episodeId) { toast.warning('Please select an episode'); return; }
                payload.episodeId = episodeId;
            }

            try {
                await api.post('/credits', payload);
                toast.success('Credit assigned successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    async function showEditModal(id) {
        modal.open('Edit Credit', Comp.pageLoader());
        try {
            const t = await api.get(`/credits/${id}`);
            modal.open(`Edit Credit for ${toast.esc(t.personName)}`,
                `<div class="space-y-4">
                    <div class="form-group">
                        <label class="form-label">Person</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(t.personName)}" readonly disabled>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Target Content (${t.targetType})</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(t.targetTitle)}" readonly disabled>
                    </div>
                    <div class="grid grid-cols-2 gap-4">
                        <div class="form-group">
                            <label class="form-label">Role <span class="text-vs-error">*</span></label>
                            <input type="text" class="input-field" id="edit-credit-role" value="${toast.esc(t.role)}">
                        </div>
                        <div class="form-group">
                            <label class="form-label">Character Name (Actor only)</label>
                            <input type="text" class="input-field" id="edit-credit-character" value="${toast.esc(t.characterName || '')}">
                        </div>
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-edit-credit"><i class="fas fa-save mr-1"></i> Save Changes</button>`
            );

            document.getElementById('save-edit-credit')?.addEventListener('click', async () => {
                const role = document.getElementById('edit-credit-role').value.trim();
                const characterName = document.getElementById('edit-credit-character').value.trim();

                if (!role) { toast.warning('Role is required'); return; }

                try {
                    await api.put(`/credits/${id}`, {
                        role,
                        characterName: characterName || null
                    });
                    toast.success('Credit updated successfully');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) { toast.error('Failed to load credit details: ' + err.message); modal.close(); }
    }

    function showDeleteModal(id, name) {
        modal.open('Delete Credit Assignment',
            `<p class="text-sm text-vs-dim">Are you sure you want to delete the credit assignment for <strong class="text-vs-text">"${toast.esc(name)}"</strong>? This is permanent.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-credit">Delete</button>`
        );

        document.getElementById('confirm-delete-credit')?.addEventListener('click', async () => {
            try {
                await api.del(`/credits/${id}`);
                toast.success('Credit assignment deleted successfully');
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