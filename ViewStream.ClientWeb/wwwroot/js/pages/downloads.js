pages.downloads = (() => {
    let state = { items: [], loading: true };

    async function load() {
        state.loading = true; render();
        try {
            state.items = await api.get('/profiles/me/downloads') || [];
        } catch (err) { toast.error('Failed to load downloads: ' + err.message); }
        state.loading = false; render();
    }

    function qualityBadge(q) {
        const color = q === '1080p' ? 'bg-vs-accent/15 text-vs-accent'
            : q === '720p' ? 'bg-vs-success/15 text-vs-success'
            : 'bg-vs-card text-vs-dim';
        return `<span class="px-2 py-0.5 rounded-full text-xs font-semibold ${color}">${toast.esc(q || 'SD')}</span>`;
    }

    function render() {
        const c = document.getElementById('downloads-content');
        if (!c) return;
        let h = Comp.pageHeader('Downloads', 'Manage your offline episodes',
            `<button id="add-download-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus mr-1"></i> Add Download</button>`);

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.items.length) {
            h += Comp.emptyState('fa-download', 'No offline downloads yet', 'Start downloading episodes to watch them offline.');
        } else {
            h += `<div class="space-y-3" id="downloads-list">`;
            state.items.forEach(d => {
                const isExpired = d.expiresAt && new Date(d.expiresAt) < new Date();
                h += `<div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border ${isExpired ? 'border-vs-error/30 bg-vs-error/5' : 'border-vs-border'} transition-all hover:border-vs-dim">
                    <div class="flex items-center gap-4 min-w-0">
                        <div class="w-10 h-10 rounded-lg bg-vs-card flex items-center justify-center flex-shrink-0 ${isExpired ? 'text-vs-error' : 'text-vs-accent'}">
                            <i class="fas fa-${isExpired ? 'exclamation-circle' : 'video'}"></i>
                        </div>
                        <div class="min-w-0">
                            <p class="text-sm font-semibold text-vs-text truncate">${toast.esc(d.episodeTitle)}</p>
                            <div class="flex items-center gap-2 mt-1 flex-wrap">
                                ${qualityBadge(d.downloadQuality)}
                                ${d.deviceName ? `<span class="text-xs text-vs-muted"><i class="fas fa-laptop mr-1"></i>${toast.esc(d.deviceName)}</span>` : ''}
                                <span class="text-xs text-vs-muted">${d.downloadedAt ? utils.formatDateShort(d.downloadedAt) : ''}</span>
                                ${isExpired ? `<span class="text-xs text-vs-error font-medium">Expired</span>` :
                                    d.expiresAt ? `<span class="text-xs text-vs-muted">Expires ${utils.formatDateShort(d.expiresAt)}</span>` : ''}
                            </div>
                        </div>
                    </div>
                    <div class="flex items-center gap-2 flex-shrink-0 ml-4">
                        <a href="#/episode/${d.episodeId}" class="btn btn-ghost btn-sm ${isExpired ? 'opacity-40 pointer-events-none' : ''}">
                            <i class="fas fa-play text-xs"></i>
                        </a>
                        <button class="btn btn-ghost btn-sm text-vs-error delete-download-btn" data-id="${d.id}">
                            <i class="fas fa-trash-alt text-xs"></i>
                        </button>
                    </div>
                </div>`;
            });
            h += `</div>`;
        }

        c.innerHTML = h;
        bindEvents();
    }

    function showAddModal() {
        modal.open('Add Offline Download',
            `<div class="space-y-4">
                <div>
                    <label class="form-label">Episode ID</label>
                    <input type="number" id="dl-episode-id" class="input-field" placeholder="Episode ID">
                </div>
                <div>
                    <label class="form-label">Device ID</label>
                    <input type="number" id="dl-device-id" class="input-field" placeholder="Device ID">
                </div>
                <div>
                    <label class="form-label">Quality</label>
                    <select id="dl-quality" class="input-field">
                        <option value="480p">480p</option>
                        <option value="720p" selected>720p</option>
                        <option value="1080p">1080p</option>
                    </select>
                </div>
                <div>
                    <label class="form-label">Expires (optional)</label>
                    <input type="date" id="dl-expires" class="input-field">
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-download-btn">Download</button>`);

        document.getElementById('save-download-btn')?.addEventListener('click', async () => {
            const episodeId = parseInt(document.getElementById('dl-episode-id').value);
            const deviceId = parseInt(document.getElementById('dl-device-id').value);
            const quality = document.getElementById('dl-quality').value;
            const expires = document.getElementById('dl-expires').value;

            if (!episodeId || !deviceId) { toast.warning('Episode ID and Device ID are required'); return; }

            try {
                await api.post('/profiles/me/downloads', {
                    episodeId, deviceId,
                    downloadQuality: quality,
                    expiresAt: expires ? new Date(expires).toISOString() : null
                });
                toast.success('Download added');
                modal.close();
                load();
            } catch (err) { toast.error(err.message); }
        });
    }

    function bindEvents() {
        document.getElementById('add-download-btn')?.addEventListener('click', showAddModal);

        document.getElementById('downloads-list')?.addEventListener('click', async (e) => {
            const btn = e.target.closest('.delete-download-btn');
            if (!btn) return;
            const id = btn.dataset.id;
            modal.open('Remove Download', '<p class="text-vs-dim">Remove this offline download?</p>',
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-danger" id="confirm-delete-dl">Remove</button>`);
            document.getElementById('confirm-delete-dl')?.addEventListener('click', async () => {
                try {
                    await api.del(`/profiles/me/downloads/${id}`);
                    toast.info('Download removed');
                    modal.close();
                    load();
                } catch (err) { toast.error(err.message); }
            });
        });
    }

    return {
        render() { return '<div id="downloads-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();
