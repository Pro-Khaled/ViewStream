pages.adminAnalytics = (() => {
    let activeTab = 'playback'; // or 'interactions'
    let playbackState = { page: 1, episodeId: '', profileId: '', data: null, loading: true };
    let interactionState = { mode: 'profile', id: '', page: 1, data: null, summary: null, loading: false };

    let playbackSortKey = 'createdAt';
    let playbackSortDir = 'desc';

    async function loadPlayback() {
        playbackState.loading = true; render();
        try {
            playbackState.data = await api.get('/admin/playback/events', {
                page: playbackState.page,
                pageSize: CONFIG.ANALYTICS_PAGE_SIZE,
                episodeId: playbackState.episodeId || undefined,
                profileId: playbackState.profileId || undefined,
                orderBy: playbackSortKey,
                isDescending: playbackSortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load playback events'); }
        playbackState.loading = false; render();
    }

    async function loadInteractions() {
        interactionState.loading = true; render();
        try {
            if (interactionState.mode === 'profile' && interactionState.id) {
                interactionState.data = await api.get(`/api/profiles/${interactionState.id}/interactions`, {
                    page: interactionState.page,
                    pageSize: CONFIG.ANALYTICS_PAGE_SIZE
                });
            } else if (interactionState.mode === 'show' && interactionState.id) {
                interactionState.data = await api.get(`/api/shows/${interactionState.id}/interactions`, {
                    page: interactionState.page,
                    pageSize: CONFIG.ANALYTICS_PAGE_SIZE
                });
            } else {
                interactionState.data = null;
            }
        } catch (err) { toast.error('Failed to load interactions'); }
        interactionState.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-analytics-content');
        if (!c) return;
        let h = `<div class="flex gap-1 mb-6 bg-vs-card rounded-lg p-1 w-fit">
            <button id="tab-playback-btn" class="px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'playback' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}">Playback Events</button>
            <button id="tab-interactions-btn" class="px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'interactions' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}">Interactions</button>
        </div>`;

        if (activeTab === 'playback') {
            h += renderPlayback();
        } else {
            h += renderInteractions();
        }
        c.innerHTML = h;
        bindEvents();
    }

    function renderPlayback() {
        let h = Comp.pageHeader('Playback Events', 'View streaming playback activity');
        h += Comp.filterBar([
            { key: 'episodeId', label: 'Episode ID', type: 'number' },
            { key: 'profileId', label: 'Profile ID', type: 'number' },
        ], vals => { playbackState.episodeId = vals.episodeId; playbackState.profileId = vals.profileId; playbackState.page = 1; loadPlayback(); });

        if (playbackState.loading) { h += Comp.pageLoader(); }
        else if (!playbackState.data?.items?.length) { h += Comp.emptyState('fa-play-circle', 'No playback events found.'); }
        else {
            const rows = playbackState.data.items.map(e => `<tr>
                <td class="text-muted">${e.id}</td>
                <td>${e.episodeId || '—'}</td>
                <td>${toast.esc(e.profileName || '—')}</td>
                <td><span class="badge badge-info">${toast.esc(e.eventType || '—')}</span></td>
                <td class="text-muted">${e.positionSeconds != null ? Math.floor(e.positionSeconds) + 's' : '—'}</td>
                <td class="text-muted">${toast.esc(e.quality)}</td>
                <td class="text-muted text-sm">${toast.esc(e.deviceType)}</td>
                <td class="text-muted text-sm">${utils.formatDate(e.createdAt)}</td>
            </tr>`).join('');
            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'episodeId', label: 'Episode' },
                    { key: 'profileName', label: 'Profile' },
                    { key: 'eventType', label: 'Type' },
                    { key: 'positionSeconds', label: 'Position' },
                    { key: 'quality', label: 'Quality' },
                    { key: 'deviceType', label: 'Device' },
                    { key: 'createdAt', label: 'Timestamp' }
                ],
                rows,
                'No playback events found.',
                {
                    tableId: 'playback-table',
                    sortKey: playbackSortKey,
                    sortDir: playbackSortDir,
                    onSort: (key, dir) => { playbackSortKey = key; playbackSortDir = dir; loadPlayback(); }
                }
            );
            h += Comp.pagination(playbackState.data.pageNumber, playbackState.data.totalPages, p => { playbackState.page = p; loadPlayback(); });
        }
        return h;
    }

    function renderInteractions() {
        const isProfile = interactionState.mode === 'profile';
        let h = Comp.pageHeader(
            isProfile ? 'Profile Interactions' : 'Show Interactions',
            isProfile ? 'View interaction history and summary for a profile' : 'View interaction history for a show'
        );
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-wrap items-end gap-4">
                <div class="form-group mb-0">
                    <label class="form-label">Mode</label>
                    <select id="int-mode-select" class="input-field" style="width:auto">
                        <option value="profile" ${isProfile ? 'selected' : ''}>Profile</option>
                        <option value="show" ${interactionState.mode === 'show' ? 'selected' : ''}>Show</option>
                    </select>
                </div>
                <div class="form-group mb-0" style="min-width:200px">
                    <label class="form-label">${isProfile ? 'Profile' : 'Show'} ID</label>
                    <input class="input-field" type="number" id="int-id-input" value="${toast.esc(interactionState.id)}" placeholder="Enter ID…">
                </div>
                <button class="btn btn-primary btn-sm" id="int-load-btn">Load</button>
                ${isProfile ? '<button class="btn btn-secondary btn-sm" id="int-summary-btn">Summary</button>' : ''}
            </div>
        </div>`;

        if (!interactionState.id && !interactionState.loading) {
            h += Comp.emptyState('fa-chart-bar', 'Enter an ID to load interactions.');
            return h;
        }

        if (interactionState.loading) { h += Comp.pageLoader(); return h; }

        // Summary if available
        if (interactionState.summary && isProfile) {
            const s = interactionState.summary;
            h += `<div class="card p-6 mb-6"><h3 class="font-display font-semibold mb-4">Interaction Summary</h3>
                <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
                    ${Comp.statCard(s.totalInteractions, 'Total', 'fa-mouse-pointer', 'text-vs-accent')}
                    ${Object.entries(s.interactionsByType || {}).sort((a, b) => b[1] - a[1]).slice(0, 3).map(([type, count]) => Comp.statCard(count, type, 'fa-tag', 'text-info')).join('')}
                </div>
                ${s.topShows?.length ? `<div class="mt-4"><div class="detail-label mb-2">Top Shows</div>${s.topShows.map((sh, i) => `<div class="flex items-center justify-between py-2 border-b border-vs-border"><span class="text-sm">${i + 1}. ${toast.esc(sh.showTitle)}</span><span class="text-muted text-sm">${sh.interactionCount} interactions</span></div>`).join('')}</div>` : ''}
            </div>`;
        }

        // Data table
        if (!interactionState.data?.items?.length) { h += Comp.emptyState('fa-chart-bar', 'No interactions found.'); return h; }
        const rows = interactionState.data.items.map(i => `<tr>
            <td class="text-muted">${i.id}</td>
            <td class="font-medium">${toast.esc(isProfile ? i.showTitle : i.profileName)}</td>
            <td><span class="badge badge-info">${toast.esc(i.interactionType)}</span></td>
            <td class="text-muted text-sm">${utils.formatDateShort(i.createdAt)}</td>
        </tr>`).join('');
        h += Comp.dataTable(
            [
                { key: 'id', label: 'ID' },
                { key: isProfile ? 'showTitle' : 'profileName', label: isProfile ? 'Show' : 'Profile' },
                { key: 'interactionType', label: 'Type' },
                { key: 'createdAt', label: 'Timestamp' }
            ],
            rows,
            'No interactions found.',
            {
                tableId: 'interactions-table',
                // No sorting for interactions because the data comes from a specific profile/show endpoint
                // You can add local sorting later if needed
            }
        );
        h += Comp.pagination(interactionState.data.pageNumber, interactionState.data.totalPages, p => { interactionState.page = p; loadInteractions(); });
        return h;
    }

    function bindEvents() {
        document.getElementById('tab-playback-btn')?.addEventListener('click', () => { activeTab = 'playback'; render(); });
        document.getElementById('tab-interactions-btn')?.addEventListener('click', () => { activeTab = 'interactions'; render(); });
        document.getElementById('int-load-btn')?.addEventListener('click', () => {
            const id = parseInt(document.getElementById('int-id-input').value);
            if (id) { interactionState.id = id; interactionState.page = 1; loadInteractions(); }
        });
        document.getElementById('int-mode-select')?.addEventListener('change', e => {
            interactionState.mode = e.target.value;
            interactionState.data = null; interactionState.summary = null;
            if (interactionState.id) { interactionState.page = 1; loadInteractions(); }
        });
        document.getElementById('int-summary-btn')?.addEventListener('click', async () => {
            if (!interactionState.id) return;
            try {
                interactionState.summary = await api.get(`/api/profiles/${interactionState.id}/interactions/summary`);
            } catch { toast.error('Failed to load summary'); }
            render();
        });
    }

    return {
        render() { return '<div id="admin-analytics-content">' + Comp.pageLoader() + '</div>'; },
        init() { activeTab = 'playback'; loadPlayback(); }
    };
})();