pages.adminSubtitles = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true };
    let sortKey = 'id';
    let sortDir = 'desc';
    let showsList = [];

    async function loadShows() {
        try {
            const res = await api.get('/shows', { pageSize: 200 }) || {};
            showsList = res.items || [];
        } catch (err) {
            console.error('Failed to load shows', err);
        }
    }

    async function loadData() {
        state.loading = true; render();
        if (showsList.length === 0) {
            await loadShows();
        }
        try {
            state.data = await api.get('/admin/subtitles', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load subtitles: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Subtitle Management', '', '<button class="btn btn-primary btn-sm" id="add-subtitle-btn"><i class="fas fa-plus mr-1.5"></i> Add Subtitle</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="subtitle-search" value="${toast.esc(state.search)}" placeholder="Search by episode title or language code...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="subtitle-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="subtitle-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="subtitle-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-closed-captioning', 'No subtitles found'); }
        else {
            const rows = state.data.items.map(s => {
                let statusBadge = `<span class="badge badge-success">Active</span>`;
                if (s.isDeleted) {
                    statusBadge = '<span class="badge badge-danger">Deleted</span>';
                }
                const ccBadge = s.isCc ? `<span class="badge badge-info text-xs font-semibold">CC</span>` : '<span class="text-vs-muted text-xs">No</span>';
                return `<tr data-subtitle-id="${s.id}" class="${s.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">${s.id}</td>
                    <td class="font-bold text-vs-text max-w-[200px] truncate">${toast.esc(s.episodeTitle || 'Episode #' + s.episodeId)}</td>
                    <td class="font-medium text-vs-accent text-center uppercase">${toast.esc(s.languageCode)}</td>
                    <td>${ccBadge}</td>
                    <td class="text-sm max-w-[250px] truncate text-vs-dim" title="${toast.esc(s.subtitleUrl)}">${toast.esc(s.subtitleUrl)}</td>
                    <td>${statusBadge}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-subtitle-btn" data-id="${s.id}" data-episode-id="${s.episodeId}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm upload-sub-file-btn" data-id="${s.id}" data-episode-id="${s.episodeId}" title="Upload Subtitle File"><i class="fas fa-file-upload text-xs"></i></button>
                            ${s.isDeleted ? 
                                `<button class="btn btn-ghost btn-sm text-vs-success restore-subtitle-btn" data-id="${s.id}" data-title="Subtitle #${s.id}" title="Restore"><i class="fas fa-trash-restore text-xs"></i></button>` : 
                                `<button class="btn btn-ghost btn-sm text-vs-error delete-subtitle-btn" data-id="${s.id}" data-episode-id="${s.episodeId}" data-title="Subtitle #${s.id}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>`
                            }
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'episodeTitle', label: 'Episode' },
                    { key: 'languageCode', label: 'Language' },
                    { key: 'isCc', label: 'CC' },
                    { key: 'subtitleUrl', label: 'Subtitle URL' },
                    { key: 'isDeleted', label: 'Status' },
                    { key: '', label: '' }
                ],
                rows,
                'No subtitles found',
                {
                    tableId: 'subtitles-table',
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
        document.getElementById('subtitle-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('subtitle-search').value.trim();
            state.includeDeleted = document.getElementById('subtitle-include-deleted').checked;
            state.page = 1; loadData();
        });
        document.getElementById('subtitle-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });
        document.getElementById('add-subtitle-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-subtitle-btn').forEach(btn => {
            btn.addEventListener('click', () => showEditModal(parseInt(btn.dataset.id), parseInt(btn.dataset.episodeId)));
        });

        document.querySelectorAll('.upload-sub-file-btn').forEach(btn => {
            btn.addEventListener('click', () => showUploadSubModal(parseInt(btn.dataset.id), parseInt(btn.dataset.episodeId)));
        });

        document.querySelectorAll('.delete-subtitle-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), parseInt(btn.dataset.episodeId), btn.dataset.title));
        });

        document.querySelectorAll('.restore-subtitle-btn').forEach(btn => {
            btn.addEventListener('click', () => showRestoreModal(parseInt(btn.dataset.id), btn.dataset.title));
        });
    }

    function showCreateModal() {
        const options = showsList.map(show => `<option value="${show.id}">${toast.esc(show.title)}</option>`).join('');
        modal.open('Add Subtitle',
            `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                <div class="form-group">
                    <label class="form-label">Show <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-sub-show-id">
                        <option value="">-- Select Show --</option>
                        ${options}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Season <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-sub-season-id" disabled>
                        <option value="">-- Select Season --</option>
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Episode <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-sub-episode-id" disabled>
                        <option value="">-- Select Episode --</option>
                    </select>
                </div>
                <div class="grid grid-cols-2 gap-4">
                    <div class="form-group">
                        <label class="form-label">Language Code (2 letters) <span class="text-vs-error">*</span></label>
                        <input type="text" class="input-field" id="new-sub-lang" maxlength="2" placeholder="e.g. en, es, fr">
                    </div>
                    <div class="form-group flex items-center gap-2 py-6">
                        <input type="checkbox" id="new-sub-cc" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                        <label for="new-sub-cc" class="text-sm text-vs-dim cursor-pointer select-none">Closed Captions (CC)</label>
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Subtitle URL <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="new-sub-url" placeholder="e.g. https://storage.com/sub.vtt">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-sub"><i class="fas fa-plus mr-1"></i> Create</button>`
        );

        const showSelect = document.getElementById('new-sub-show-id');
        const seasonSelect = document.getElementById('new-sub-season-id');
        const episodeSelect = document.getElementById('new-sub-episode-id');

        showSelect?.addEventListener('change', async () => {
            const showId = showSelect.value;
            seasonSelect.innerHTML = '<option value="">-- Select Season --</option>';
            seasonSelect.disabled = true;
            episodeSelect.innerHTML = '<option value="">-- Select Episode --</option>';
            episodeSelect.disabled = true;
            if (!showId) return;

            try {
                const seasons = await api.get(`/shows/${showId}/seasons`) || [];
                let optHtml = '<option value="">-- Select Season --</option>';
                seasons.forEach(s => {
                    optHtml += `<option value="${s.id}">Season ${s.seasonNumber} ${s.title ? '- ' + s.title : ''}</option>`;
                });
                seasonSelect.innerHTML = optHtml;
                seasonSelect.disabled = false;
            } catch (err) { toast.error('Failed to load seasons'); }
        });

        seasonSelect?.addEventListener('change', async () => {
            const seasonId = seasonSelect.value;
            episodeSelect.innerHTML = '<option value="">-- Select Episode --</option>';
            episodeSelect.disabled = true;
            if (!seasonId) return;

            try {
                const episodes = await api.get(`/seasons/${seasonId}/episodes`) || [];
                let optHtml = '<option value="">-- Select Episode --</option>';
                episodes.forEach(e => {
                    optHtml += `<option value="${e.id}">Episode ${e.episodeNumber} - ${e.title}</option>`;
                });
                episodeSelect.innerHTML = optHtml;
                episodeSelect.disabled = false;
            } catch (err) { toast.error('Failed to load episodes'); }
        });

        document.getElementById('save-new-sub')?.addEventListener('click', async () => {
            const episodeId = parseInt(episodeSelect.value);
            const languageCode = document.getElementById('new-sub-lang').value.trim().toLowerCase();
            const isCc = document.getElementById('new-sub-cc').checked;
            const subtitleUrl = document.getElementById('new-sub-url').value.trim();

            if (!episodeId) { toast.warning('Please select an episode'); return; }
            if (languageCode.length !== 2) { toast.warning('Language code must be exactly 2 characters'); return; }
            if (!subtitleUrl) { toast.warning('Subtitle URL is required'); return; }

            try {
                await api.post(`/episodes/${episodeId}/subtitles`, {
                    episodeId,
                    languageCode,
                    subtitleUrl,
                    isCc
                });
                toast.success('Subtitle created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    async function showEditModal(id, episodeId) {
        modal.open('Edit Subtitle', Comp.pageLoader());
        try {
            const s = await api.get(`/episodes/${episodeId}/subtitles/${id}`);
            modal.open(`Edit Subtitle #${s.id}`,
                `<div class="space-y-4">
                    <div class="form-group">
                        <label class="form-label">Episode</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(s.episodeTitle)}" readonly disabled>
                    </div>
                    <div class="grid grid-cols-2 gap-4">
                        <div class="form-group">
                            <label class="form-label">Language Code (2 letters) <span class="text-vs-error">*</span></label>
                            <input type="text" class="input-field" id="edit-sub-lang" maxlength="2" value="${toast.esc(s.languageCode)}">
                        </div>
                        <div class="form-group flex items-center gap-2 py-6">
                            <input type="checkbox" id="edit-sub-cc" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent" ${s.isCc ? 'checked' : ''}>
                            <label for="edit-sub-cc" class="text-sm text-vs-dim cursor-pointer select-none">Closed Captions (CC)</label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Subtitle URL <span class="text-vs-error">*</span></label>
                        <input type="text" class="input-field" id="edit-sub-url" value="${toast.esc(s.subtitleUrl)}">
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-edit-sub"><i class="fas fa-save mr-1"></i> Save Changes</button>`
            );

            document.getElementById('save-edit-sub')?.addEventListener('click', async () => {
                const languageCode = document.getElementById('edit-sub-lang').value.trim().toLowerCase();
                const isCc = document.getElementById('edit-sub-cc').checked;
                const subtitleUrl = document.getElementById('edit-sub-url').value.trim();

                if (languageCode.length !== 2) { toast.warning('Language code must be exactly 2 characters'); return; }
                if (!subtitleUrl) { toast.warning('Subtitle URL is required'); return; }

                try {
                    await api.put(`/episodes/${episodeId}/subtitles/${id}`, {
                        languageCode,
                        subtitleUrl,
                        isCc
                    });
                    toast.success('Subtitle updated successfully');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) { toast.error('Failed to load subtitle: ' + err.message); modal.close(); }
    }

    function showUploadSubModal(id, episodeId) {
        modal.open('Upload Subtitle File',
            `<div class="space-y-4">
                <p class="text-sm text-vs-dim">Select a subtitle file (VTT or SRT) to upload for this subtitle record.</p>
                <div class="border-2 border-dashed border-vs-border rounded-xl p-6 flex flex-col items-center justify-center hover:border-vs-accent/50 cursor-pointer" onclick="document.getElementById('sub-file-input').click()">
                    <i class="fas fa-cloud-upload-alt text-4xl text-vs-muted mb-3"></i>
                    <span class="text-sm text-vs-text font-medium">Click to choose subtitle file</span>
                    <input type="file" id="sub-file-input" class="hidden" accept=".vtt,.srt">
                    <span class="text-xs text-vs-dim mt-1.5" id="sub-file-name">No file chosen</span>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="start-sub-upload" disabled>Upload</button>`
        );

        const fileInput = document.getElementById('sub-file-input');
        const uploadBtn = document.getElementById('start-sub-upload');
        const fileName = document.getElementById('sub-file-name');

        fileInput?.addEventListener('change', () => {
            if (fileInput.files.length > 0) {
                fileName.textContent = fileInput.files[0].name;
                uploadBtn.disabled = false;
            } else {
                fileName.textContent = 'No file chosen';
                uploadBtn.disabled = true;
            }
        });

        uploadBtn?.addEventListener('click', async () => {
            const file = fileInput.files[0];
            if (!file) return;
            uploadBtn.disabled = true;
            uploadBtn.textContent = 'Uploading...';
            try {
                const fd = new FormData();
                fd.append('file', file);
                const res = await api.upload(`/episodes/${episodeId}/subtitles/${id}/upload-file`, fd);
                toast.success('Subtitle file uploaded successfully');
                modal.close();
                loadData();
            } catch (err) {
                toast.error(err.message);
                uploadBtn.disabled = false;
                uploadBtn.textContent = 'Upload';
            }
        });
    }

    function showDeleteModal(id, episodeId, title) {
        modal.open('Delete Subtitle',
            `<p class="text-sm text-vs-dim">Are you sure you want to soft-delete <strong class="text-vs-text">"${toast.esc(title)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-sub">Delete</button>`
        );

        document.getElementById('confirm-delete-sub')?.addEventListener('click', async () => {
            try {
                await api.del(`/episodes/${episodeId}/subtitles/${id}`);
                toast.success('Subtitle soft-deleted successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showRestoreModal(id, title) {
        modal.open('Restore Subtitle',
            `<p class="text-sm text-vs-dim">Are you sure you want to restore <strong class="text-vs-text">"${toast.esc(title)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-success" id="confirm-restore-sub">Restore</button>`
        );

        document.getElementById('confirm-restore-sub')?.addEventListener('click', async () => {
            try {
                await api.post(`/admin/subtitles/${id}/restore`);
                toast.success('Subtitle restored successfully');
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