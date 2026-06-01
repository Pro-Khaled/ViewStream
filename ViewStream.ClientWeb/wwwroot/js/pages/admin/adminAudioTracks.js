pages.adminAudioTracks = (() => {
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
            state.data = await api.get('/admin/audiotracks', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load audio tracks: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Audio Track Management', '', '<button class="btn btn-primary btn-sm" id="add-audiotrack-btn"><i class="fas fa-plus mr-1.5"></i> Add Audio Track</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="audiotrack-search" value="${toast.esc(state.search)}" placeholder="Search by episode title, language code, or track type...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="audiotrack-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="audiotrack-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="audiotrack-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-music', 'No audio tracks found'); }
        else {
            const rows = state.data.items.map(t => {
                let statusBadge = `<span class="badge badge-success">Active</span>`;
                if (t.isDeleted) {
                    statusBadge = '<span class="badge badge-danger">Deleted</span>';
                }
                const defaultBadge = t.isDefault ? `<span class="badge badge-warning text-xs font-semibold">Default</span>` : '<span class="text-vs-muted text-xs">No</span>';
                return `<tr data-audiotrack-id="${t.id}" class="${t.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">${t.id}</td>
                    <td class="font-bold text-vs-text max-w-[200px] truncate">${toast.esc(t.episodeTitle || 'Episode #' + t.episodeId)}</td>
                    <td class="font-medium text-vs-accent text-center uppercase">${toast.esc(t.languageCode)}</td>
                    <td>${toast.esc(t.trackType || 'original')}</td>
                    <td>${defaultBadge}</td>
                    <td class="text-sm max-w-[200px] truncate text-vs-dim" title="${toast.esc(t.audioUrl)}">${toast.esc(t.audioUrl)}</td>
                    <td>${statusBadge}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-audiotrack-btn" data-id="${t.id}" data-episode-id="${t.episodeId}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm upload-audio-btn" data-id="${t.id}" data-episode-id="${t.episodeId}" title="Upload Audio File"><i class="fas fa-file-upload text-xs"></i></button>
                            ${t.isDeleted ? 
                                `<button class="btn btn-ghost btn-sm text-vs-success restore-audiotrack-btn" data-id="${t.id}" data-title="Track #${t.id}" title="Restore"><i class="fas fa-trash-restore text-xs"></i></button>` : 
                                `<button class="btn btn-ghost btn-sm text-vs-error delete-audiotrack-btn" data-id="${t.id}" data-episode-id="${t.episodeId}" data-title="Track #${t.id}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>`
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
                    { key: 'trackType', label: 'Type' },
                    { key: 'isDefault', label: 'Default' },
                    { key: 'audioUrl', label: 'URL' },
                    { key: 'isDeleted', label: 'Status' },
                    { key: '', label: '' }
                ],
                rows,
                'No audio tracks found',
                {
                    tableId: 'audiotracks-table',
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
        document.getElementById('audiotrack-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('audiotrack-search').value.trim();
            state.includeDeleted = document.getElementById('audiotrack-include-deleted').checked;
            state.page = 1; loadData();
        });
        document.getElementById('audiotrack-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });
        document.getElementById('add-audiotrack-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-audiotrack-btn').forEach(btn => {
            btn.addEventListener('click', () => showEditModal(parseInt(btn.dataset.id), parseInt(btn.dataset.episodeId)));
        });

        document.querySelectorAll('.upload-audio-btn').forEach(btn => {
            btn.addEventListener('click', () => showUploadAudioModal(parseInt(btn.dataset.id), parseInt(btn.dataset.episodeId)));
        });

        document.querySelectorAll('.delete-audiotrack-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), parseInt(btn.dataset.episodeId), btn.dataset.title));
        });

        document.querySelectorAll('.restore-audiotrack-btn').forEach(btn => {
            btn.addEventListener('click', () => showRestoreModal(parseInt(btn.dataset.id), btn.dataset.title));
        });
    }

    function showCreateModal() {
        const options = showsList.map(show => `<option value="${show.id}">${toast.esc(show.title)}</option>`).join('');
        modal.open('Add Audio Track',
            `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                <div class="form-group">
                    <label class="form-label">Show <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-at-show-id">
                        <option value="">-- Select Show --</option>
                        ${options}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Season <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-at-season-id" disabled>
                        <option value="">-- Select Season --</option>
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Episode <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-at-episode-id" disabled>
                        <option value="">-- Select Episode --</option>
                    </select>
                </div>
                <div class="grid grid-cols-2 gap-4">
                    <div class="form-group">
                        <label class="form-label">Language Code (2 letters) <span class="text-vs-error">*</span></label>
                        <input type="text" class="input-field" id="new-at-lang" maxlength="2" placeholder="e.g. en, ar, es">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Track Type</label>
                        <input type="text" class="input-field" id="new-at-type" placeholder="e.g. original, dubbed">
                    </div>
                </div>
                <div class="form-group flex items-center gap-2 py-2">
                    <input type="checkbox" id="new-at-default" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="new-at-default" class="text-sm text-vs-dim cursor-pointer select-none">Set as Default Audio Track</label>
                </div>
                <div class="form-group">
                    <label class="form-label">Audio URL <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="new-at-url" placeholder="e.g. https://storage.com/audio.mp3">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-at"><i class="fas fa-plus mr-1"></i> Create</button>`
        );

        const showSelect = document.getElementById('new-at-show-id');
        const seasonSelect = document.getElementById('new-at-season-id');
        const episodeSelect = document.getElementById('new-at-episode-id');

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

        document.getElementById('save-new-at')?.addEventListener('click', async () => {
            const episodeId = parseInt(episodeSelect.value);
            const languageCode = document.getElementById('new-at-lang').value.trim().toLowerCase();
            const trackType = document.getElementById('new-at-type').value.trim();
            const isDefault = document.getElementById('new-at-default').checked;
            const audioUrl = document.getElementById('new-at-url').value.trim();

            if (!episodeId) { toast.warning('Please select an episode'); return; }
            if (languageCode.length !== 2) { toast.warning('Language code must be exactly 2 characters'); return; }
            if (!audioUrl) { toast.warning('Audio URL is required'); return; }

            try {
                await api.post(`/episodes/${episodeId}/audio-tracks`, {
                    episodeId,
                    languageCode,
                    trackType: trackType || null,
                    audioUrl,
                    isDefault
                });
                toast.success('Audio track created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    async function showEditModal(id, episodeId) {
        modal.open('Edit Audio Track', Comp.pageLoader());
        try {
            const t = await api.get(`/episodes/${episodeId}/audio-tracks/${id}`);
            modal.open(`Edit Audio Track #${t.id}`,
                `<div class="space-y-4">
                    <div class="form-group">
                        <label class="form-label">Episode</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(t.episodeTitle)}" readonly disabled>
                    </div>
                    <div class="grid grid-cols-2 gap-4">
                        <div class="form-group">
                            <label class="form-label">Language Code (2 letters) <span class="text-vs-error">*</span></label>
                            <input type="text" class="input-field" id="edit-at-lang" maxlength="2" value="${toast.esc(t.languageCode)}">
                        </div>
                        <div class="form-group">
                            <label class="form-label">Track Type</label>
                            <input type="text" class="input-field" id="edit-at-type" value="${toast.esc(t.trackType || '')}">
                        </div>
                    </div>
                    <div class="form-group flex items-center gap-2 py-2">
                        <input type="checkbox" id="edit-at-default" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent" ${t.isDefault ? 'checked' : ''}>
                        <label for="edit-at-default" class="text-sm text-vs-dim cursor-pointer select-none">Set as Default Audio Track</label>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Audio URL <span class="text-vs-error">*</span></label>
                        <input type="text" class="input-field" id="edit-at-url" value="${toast.esc(t.audioUrl)}">
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-edit-at"><i class="fas fa-save mr-1"></i> Save Changes</button>`
            );

            document.getElementById('save-edit-at')?.addEventListener('click', async () => {
                const languageCode = document.getElementById('edit-at-lang').value.trim().toLowerCase();
                const trackType = document.getElementById('edit-at-type').value.trim();
                const isDefault = document.getElementById('edit-at-default').checked;
                const audioUrl = document.getElementById('edit-at-url').value.trim();

                if (languageCode.length !== 2) { toast.warning('Language code must be exactly 2 characters'); return; }
                if (!audioUrl) { toast.warning('Audio URL is required'); return; }

                try {
                    await api.put(`/episodes/${episodeId}/audio-tracks/${id}`, {
                        languageCode,
                        trackType: trackType || null,
                        audioUrl,
                        isDefault
                    });
                    toast.success('Audio track updated successfully');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) { toast.error('Failed to load audio track: ' + err.message); modal.close(); }
    }

    function showUploadAudioModal(id, episodeId) {
        modal.open('Upload Audio File',
            `<div class="space-y-4">
                <p class="text-sm text-vs-dim">Select an audio file (MP3, AAC, M4A) to upload for this audio track.</p>
                <div class="border-2 border-dashed border-vs-border rounded-xl p-6 flex flex-col items-center justify-center hover:border-vs-accent/50 cursor-pointer" onclick="document.getElementById('audio-file-input').click()">
                    <i class="fas fa-cloud-upload-alt text-4xl text-vs-muted mb-3"></i>
                    <span class="text-sm text-vs-text font-medium">Click to choose audio file</span>
                    <input type="file" id="audio-file-input" class="hidden" accept="audio/*">
                    <span class="text-xs text-vs-dim mt-1.5" id="audio-file-name">No file chosen</span>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="start-audio-upload" disabled>Upload</button>`
        );

        const fileInput = document.getElementById('audio-file-input');
        const uploadBtn = document.getElementById('start-audio-upload');
        const fileName = document.getElementById('audio-file-name');

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
                const res = await api.upload(`/episodes/${episodeId}/audio-tracks/${id}/upload-file`, fd);
                toast.success('Audio file uploaded successfully');
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
        modal.open('Delete Audio Track',
            `<p class="text-sm text-vs-dim">Are you sure you want to soft-delete <strong class="text-vs-text">"${toast.esc(title)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-at">Delete</button>`
        );

        document.getElementById('confirm-delete-at')?.addEventListener('click', async () => {
            try {
                await api.del(`/episodes/${episodeId}/audio-tracks/${id}`);
                toast.success('Audio track soft-deleted successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showRestoreModal(id, title) {
        modal.open('Restore Audio Track',
            `<p class="text-sm text-vs-dim">Are you sure you want to restore <strong class="text-vs-text">"${toast.esc(title)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-success" id="confirm-restore-at">Restore</button>`
        );

        document.getElementById('confirm-restore-at')?.addEventListener('click', async () => {
            try {
                await api.post(`/admin/audiotracks/${id}/restore`);
                toast.success('Audio track restored successfully');
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