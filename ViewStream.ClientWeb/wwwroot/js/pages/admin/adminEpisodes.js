pages.adminEpisodes = (() => {
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
            state.data = await api.get('/admin/episodes', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) { toast.error('Failed to load episodes: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Episode Management', '', '<button class="btn btn-primary btn-sm" id="add-episode-btn"><i class="fas fa-plus mr-1.5"></i> Add Episode</button>');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="episodes-search" value="${toast.esc(state.search)}" placeholder="Search by show, season, or episode title...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="episodes-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="episodes-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="episodes-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-play-circle', 'No episodes found'); }
        else {
            const rows = state.data.items.map(e => {
                let statusBadge = `<span class="badge badge-success">Active</span>`;
                if (e.isDeleted) {
                    statusBadge = '<span class="badge badge-danger">Deleted</span>';
                }
                const runtime = e.runtimeSeconds ? `${Math.floor(e.runtimeSeconds / 60)}m` : 'â€”';
                const thumb = e.thumbnailUrl ? `<img src="${toast.esc(e.thumbnailUrl)}" class="w-12 h-8 object-cover rounded border border-vs-border">` : `<div class="w-12 h-8 bg-vs-card rounded border border-vs-border flex items-center justify-center text-vs-muted text-[10px]"><i class="fas fa-image"></i></div>`;
                return `<tr data-episode-id="${e.id}" class="${e.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">${e.id}</td>
                    <td>${thumb}</td>
                    <td class="font-bold text-vs-text max-w-[150px] truncate">${toast.esc(e.showTitle)}</td>
                    <td class="text-vs-dim text-sm">S${e.seasonNumber} E${e.episodeNumber}</td>
                    <td class="font-medium text-vs-accent max-w-[200px] truncate">${toast.esc(e.title)}</td>
                    <td>${runtime}</td>
                    <td>${statusBadge}</td>
                    <td class="text-muted text-sm">${e.releaseDate ? utils.formatDateShort(e.releaseDate) : 'â€”'}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm edit-episode-btn" data-id="${e.id}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm upload-thumb-btn" data-id="${e.id}" title="Upload Thumbnail"><i class="fas fa-image text-xs"></i></button>
                            ${e.isDeleted ? 
                                `<button class="btn btn-ghost btn-sm text-vs-success restore-episode-btn" data-id="${e.id}" data-title="S${e.seasonNumber}E${e.episodeNumber} of ${toast.esc(e.showTitle)}" title="Restore"><i class="fas fa-trash-restore text-xs"></i></button>` : 
                                `<button class="btn btn-ghost btn-sm text-vs-error delete-episode-btn" data-id="${e.id}" data-title="S${e.seasonNumber}E${e.episodeNumber} of ${toast.esc(e.showTitle)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>`
                            }
                        </div>
                    </td>
                </tr>`;
            }).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: '', label: 'Thumb' },
                    { key: 'showTitle', label: 'Show' },
                    { key: 'episodeNumber', label: 'Season/Episode' },
                    { key: 'title', label: 'Episode Title' },
                    { key: 'runtimeSeconds', label: 'Runtime' },
                    { key: 'isDeleted', label: 'Status' },
                    { key: 'releaseDate', label: 'Release Date' },
                    { key: '', label: '' }
                ],
                rows,
                'No episodes found',
                {
                    tableId: 'episodes-table',
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
        document.getElementById('episodes-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('episodes-search').value.trim();
            state.includeDeleted = document.getElementById('episodes-include-deleted').checked;
            state.page = 1; loadData();
        });
        document.getElementById('episodes-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });
        document.getElementById('add-episode-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.edit-episode-btn').forEach(btn => {
            btn.addEventListener('click', () => showEditModal(parseInt(btn.dataset.id)));
        });

        document.querySelectorAll('.upload-thumb-btn').forEach(btn => {
            btn.addEventListener('click', () => showUploadThumbModal(parseInt(btn.dataset.id)));
        });

        document.querySelectorAll('.delete-episode-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), btn.dataset.title));
        });

        document.querySelectorAll('.restore-episode-btn').forEach(btn => {
            btn.addEventListener('click', () => showRestoreModal(parseInt(btn.dataset.id), btn.dataset.title));
        });
    }

    function showCreateModal() {
        const options = showsList.map(show => `<option value="${show.id}">${toast.esc(show.title)}</option>`).join('');
        modal.open('Add Episode',
            `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                <div class="form-group">
                    <label class="form-label">Show <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-episode-show-id">
                        <option value="">-- Select Show --</option>
                        ${options}
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">Season <span class="text-vs-error">*</span></label>
                    <select class="input-field" id="new-episode-season-id" disabled>
                        <option value="">-- Select Season --</option>
                    </select>
                </div>
                <div class="grid grid-cols-2 gap-4">
                    <div class="form-group">
                        <label class="form-label">Episode Number <span class="text-vs-error">*</span></label>
                        <input type="number" class="input-field" id="new-episode-number" min="1" value="1">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Runtime (seconds)</label>
                        <input type="number" class="input-field" id="new-episode-runtime" min="1" placeholder="e.g. 1800">
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Episode Title <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="new-episode-title" placeholder="e.g. Pilot">
                </div>
                <div class="form-group">
                    <label class="form-label">Description</label>
                    <textarea class="input-field" id="new-episode-desc" rows="3" placeholder="Episode synopsis..."></textarea>
                </div>
                <div class="form-group">
                    <label class="form-label">Video URL <span class="text-vs-error">*</span></label>
                    <input type="text" class="input-field" id="new-episode-video-url" placeholder="e.g. https://storage.com/video.mp4">
                </div>
                <div class="form-group">
                    <label class="form-label">Thumbnail URL</label>
                    <input type="text" class="input-field" id="new-episode-thumb-url" placeholder="e.g. https://storage.com/thumb.jpg">
                </div>
                <div class="form-group">
                    <label class="form-label">Release Date</label>
                    <input type="date" class="input-field" id="new-episode-release-date">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-episode"><i class="fas fa-plus mr-1"></i> Create</button>`,
            { large: true }
        );

        const showSelect = document.getElementById('new-episode-show-id');
        const seasonSelect = document.getElementById('new-episode-season-id');

        showSelect?.addEventListener('change', async () => {
            const showId = showSelect.value;
            if (!showId) {
                seasonSelect.innerHTML = '<option value="">-- Select Season --</option>';
                seasonSelect.disabled = true;
                return;
            }
            seasonSelect.disabled = true;
            seasonSelect.innerHTML = '<option value="">Loading seasons...</option>';
            try {
                const seasons = await api.get(`/shows/${showId}/seasons`) || [];
                let optHtml = '<option value="">-- Select Season --</option>';
                seasons.forEach(s => {
                    optHtml += `<option value="${s.id}">Season ${s.seasonNumber} ${s.title ? '- ' + s.title : ''}</option>`;
                });
                seasonSelect.innerHTML = optHtml;
                seasonSelect.disabled = false;
            } catch (err) {
                toast.error('Failed to load seasons for this show');
                seasonSelect.innerHTML = '<option value="">-- Select Season --</option>';
            }
        });

        document.getElementById('save-new-episode')?.addEventListener('click', async () => {
            const seasonId = parseInt(seasonSelect.value);
            const episodeNumber = parseInt(document.getElementById('new-episode-number').value);
            const title = document.getElementById('new-episode-title').value.trim();
            const description = document.getElementById('new-episode-desc').value.trim();
            const runtimeSeconds = parseInt(document.getElementById('new-episode-runtime').value) || null;
            const videoUrl = document.getElementById('new-episode-video-url').value.trim();
            const thumbnailUrl = document.getElementById('new-episode-thumb-url').value.trim();
            const releaseDate = document.getElementById('new-episode-release-date').value || null;

            if (!seasonId) { toast.warning('Please select a season'); return; }
            if (!episodeNumber || episodeNumber < 1) { toast.warning('Episode number must be 1 or greater'); return; }
            if (!title) { toast.warning('Episode title is required'); return; }
            if (!videoUrl) { toast.warning('Video URL is required'); return; }

            try {
                // Prepare form data
                const fd = new FormData();
                fd.append('SeasonId', seasonId);
                fd.append('EpisodeNumber', episodeNumber);
                fd.append('Title', title);
                if (description) fd.append('Description', description);
                if (runtimeSeconds) fd.append('RuntimeSeconds', runtimeSeconds);
                fd.append('VideoUrl', videoUrl);
                if (thumbnailUrl) fd.append('ThumbnailUrl', thumbnailUrl);
                if (releaseDate) fd.append('ReleaseDate', releaseDate);

                await api.upload('/episodes', fd);
                toast.success('Episode created successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    async function showEditModal(id) {
        modal.open('Edit Episode', Comp.pageLoader());
        try {
            const e = await api.get(`/episodes/${id}`);
            modal.open(`Edit Episode - S${e.seasonNumber}E${e.episodeNumber} of ${toast.esc(e.showTitle)}`,
                `<div class="space-y-4 max-h-[65vh] overflow-y-auto px-1">
                    <div class="form-group">
                        <label class="form-label">Show & Season</label>
                        <input class="input-field bg-vs-card text-vs-muted cursor-not-allowed" value="${toast.esc(e.showTitle)} (Season ${e.seasonNumber})" readonly disabled>
                    </div>
                    <div class="grid grid-cols-2 gap-4">
                        <div class="form-group">
                            <label class="form-label">Episode Number <span class="text-vs-error">*</span></label>
                            <input type="number" class="input-field" id="edit-episode-number" min="1" value="${e.episodeNumber}">
                        </div>
                        <div class="form-group">
                            <label class="form-label">Runtime (seconds)</label>
                            <input type="number" class="input-field" id="edit-episode-runtime" min="1" value="${e.runtimeSeconds || ''}">
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Episode Title <span class="text-vs-error">*</span></label>
                        <input type="text" class="input-field" id="edit-episode-title" value="${toast.esc(e.title)}">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Description</label>
                        <textarea class="input-field" id="edit-episode-desc" rows="3">${toast.esc(e.description || '')}</textarea>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Video URL <span class="text-vs-error">*</span></label>
                        <input type="text" class="input-field" id="edit-episode-video-url" value="${toast.esc(e.videoUrl || '')}">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Thumbnail URL</label>
                        <input type="text" class="input-field" id="edit-episode-thumb-url" value="${toast.esc(e.thumbnailUrl || '')}">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Release Date</label>
                        <input type="date" class="input-field" id="edit-episode-release-date" value="${e.releaseDate || ''}">
                    </div>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-edit-episode"><i class="fas fa-save mr-1"></i> Save Changes</button>`,
                { large: true }
            );

            document.getElementById('save-edit-episode')?.addEventListener('click', async () => {
                const episodeNumber = parseInt(document.getElementById('edit-episode-number').value);
                const title = document.getElementById('edit-episode-title').value.trim();
                const description = document.getElementById('edit-episode-desc').value.trim();
                const runtimeSeconds = parseInt(document.getElementById('edit-episode-runtime').value) || null;
                const videoUrl = document.getElementById('edit-episode-video-url').value.trim();
                const thumbnailUrl = document.getElementById('edit-episode-thumb-url').value.trim();
                const releaseDate = document.getElementById('edit-episode-release-date').value || null;

                if (!episodeNumber || episodeNumber < 1) { toast.warning('Episode number must be 1 or greater'); return; }
                if (!title) { toast.warning('Episode title is required'); return; }
                if (!videoUrl) { toast.warning('Video URL is required'); return; }

                try {
                    await api.put(`/episodes/${id}`, {
                        episodeNumber,
                        title,
                        description: description || null,
                        runtimeSeconds,
                        videoUrl,
                        thumbnailUrl: thumbnailUrl || null,
                        releaseDate
                    });
                    toast.success('Episode updated successfully');
                    modal.close();
                    loadData();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) { toast.error('Failed to load episode details: ' + err.message); modal.close(); }
    }

    function showUploadThumbModal(id) {
        modal.open('Upload Thumbnail',
            `<div class="space-y-4">
                <p class="text-sm text-vs-dim">Select an image file (PNG, JPG) to upload as the thumbnail for this episode.</p>
                <div class="border-2 border-dashed border-vs-border rounded-xl p-6 flex flex-col items-center justify-center hover:border-vs-accent/50 cursor-pointer" onclick="document.getElementById('thumb-file-input').click()">
                    <i class="fas fa-cloud-upload-alt text-4xl text-vs-muted mb-3"></i>
                    <span class="text-sm text-vs-text font-medium">Click to choose image file</span>
                    <input type="file" id="thumb-file-input" class="hidden" accept="image/*">
                    <span class="text-xs text-vs-dim mt-1.5" id="thumb-file-name">No file chosen</span>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="start-thumb-upload" disabled>Upload</button>`
        );

        const fileInput = document.getElementById('thumb-file-input');
        const uploadBtn = document.getElementById('start-thumb-upload');
        const fileName = document.getElementById('thumb-file-name');

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
                fd.append('thumbnailFile', file);
                const res = await api.upload(`/episodes/${id}/upload-thumbnail`, fd);
                toast.success('Thumbnail uploaded successfully');
                modal.close();
                loadData();
            } catch (err) {
                toast.error(err.message);
                uploadBtn.disabled = false;
                uploadBtn.textContent = 'Upload';
            }
        });
    }

    function showDeleteModal(id, title) {
        modal.open('Delete Episode',
            `<p class="text-sm text-vs-dim">Are you sure you want to soft-delete <strong class="text-vs-text">"${toast.esc(title)}"</strong>? This will soft-delete associated subtitles and audio tracks.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-episode">Delete</button>`
        );

        document.getElementById('confirm-delete-episode')?.addEventListener('click', async () => {
            try {
                await api.del(`/episodes/${id}`);
                toast.success('Episode soft-deleted successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showRestoreModal(id, title) {
        modal.open('Restore Episode',
            `<p class="text-sm text-vs-dim">Are you sure you want to restore <strong class="text-vs-text">"${toast.esc(title)}"</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-success" id="confirm-restore-episode">Restore</button>`
        );

        document.getElementById('confirm-restore-episode')?.addEventListener('click', async () => {
            try {
                await api.post(`/admin/episodes/${id}/restore`);
                toast.success('Episode restored successfully');
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