pages.adminAnalytics = (() => {
    let activeTab = 'playback'; // 'playback' or 'interactions'
    let playbackState = { page: 1, episodeId: '', profileId: '', data: null, loading: true };
    let interactionState = { mode: 'profile', id: '', page: 1, data: null, summary: null, loading: false };

    let playbackSortKey = 'createdAt';
    let playbackSortDir = 'desc';

    async function loadPlayback() {
        playbackState.loading = true; render();
        try {
            playbackState.data = await api.get('/admin/playbackevents', {
                pageNumber: playbackState.page,
                pageSize: CONFIG.PAGE_SIZE,
                episodeId: playbackState.episodeId ? parseInt(playbackState.episodeId) : undefined,
                profileId: playbackState.profileId ? parseInt(playbackState.profileId) : undefined,
                sortBy: playbackSortKey,
                sortDescending: playbackSortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load playback events: ' + err.message); }
        playbackState.loading = false; render();
    }

    async function loadInteractions() {
        if (!interactionState.id) {
            toast.warn('Please enter a Profile or Show ID to load interactions');
            return;
        }

        interactionState.loading = true; render();
        try {
            const params = {
                pageNumber: interactionState.page,
                pageSize: CONFIG.PAGE_SIZE,
                profileId: interactionState.mode === 'profile' ? parseInt(interactionState.id) : undefined,
                showId: interactionState.mode === 'show' ? parseInt(interactionState.id) : undefined
            };
            interactionState.data = await api.get('/admin/userinteractions', params);

            // If profile mode, automatically load summary
            if (interactionState.mode === 'profile') {
                try {
                    interactionState.summary = await api.get(`/admin/userinteractions/profiles/${interactionState.id}/summary`);
                } catch {
                    interactionState.summary = null;
                }
            } else {
                interactionState.summary = null;
            }
        } catch (err) { toast.error('Failed to load interactions: ' + err.message); }
        interactionState.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-analytics-content');
        if (!c) return;

        let h = Comp.pageHeader('Analytics & Telemetry', 'Analyze live playback streaming statistics and profile engagements.');

        h += `<div class="flex gap-1.5 mb-6 bg-vs-surface-2 border border-vs-border rounded-xl p-1.5 w-fit font-body">
            <button id="tab-playback-btn" class="btn btn-sm px-4 py-2 rounded-lg font-bold text-xs ${activeTab === 'playback' ? 'bg-vs-accent text-vs-bg shadow' : 'bg-transparent text-vs-dim hover:bg-vs-surface transition-colors'}">
                <i class="fas fa-play mr-1.5 text-xs"></i> Playback Events
            </button>
            <button id="tab-interactions-btn" class="btn btn-sm px-4 py-2 rounded-lg font-bold text-xs ${activeTab === 'interactions' ? 'bg-vs-accent text-vs-bg shadow' : 'bg-transparent text-vs-dim hover:bg-vs-surface transition-colors'}">
                <i class="fas fa-mouse-pointer mr-1.5 text-xs"></i> User Interactions
            </button>
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
        let h = `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4 items-end">
                <div class="form-group mb-0">
                    <label class="form-label text-xs">Episode ID</label>
                    <input class="input-field" type="number" id="play-episode-id" value="${toast.esc(playbackState.episodeId)}" placeholder="Filter by Episode ID">
                </div>
                <div class="form-group mb-0">
                    <label class="form-label text-xs">Profile ID</label>
                    <input class="input-field" type="number" id="play-profile-id" value="${toast.esc(playbackState.profileId)}" placeholder="Filter by Profile ID">
                </div>
                <button class="btn btn-primary btn-sm w-full" id="play-filter-btn"><i class="fas fa-filter mr-1.5 text-xs"></i>Apply Filters</button>
            </div>
        </div>`;

        if (playbackState.loading) { h += Comp.pageLoader(); }
        else if (!playbackState.data?.items?.length) { h += Comp.emptyState('fa-play-circle', 'No playback events found.'); }
        else {
            const rows = playbackState.data.items.map(e => `<tr>
                <td class="text-muted font-mono text-xs">#${e.id}</td>
                <td class="font-mono text-xs">${e.episodeId || '—'}</td>
                <td class="font-semibold text-xs text-vs-text">${toast.esc(e.profileName || '—')}</td>
                <td><span class="badge ${e.eventType === 'Play' || e.eventType === 'Complete' ? 'badge-success' : (e.eventType === 'Stop' || e.eventType === 'Error' ? 'badge-danger' : 'badge-warning')} text-xxs font-bold uppercase font-mono">${toast.esc(e.eventType || '—')}</span></td>
                <td class="text-muted font-mono text-xs">${e.positionSeconds != null ? utils.formatDuration(e.positionSeconds) : '—'}</td>
                <td class="text-muted text-xs font-mono">${toast.esc(e.quality || 'Auto')}</td>
                <td class="text-muted text-xs">${toast.esc(e.deviceType || '—')}</td>
                <td class="text-muted text-xs font-mono">${utils.formatDate(e.createdAt || e.eventTime)}</td>
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
        let h = `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                <div class="form-group mb-0">
                    <label class="form-label text-xs">Filter By Mode</label>
                    <select id="int-mode-select" class="input-field">
                        <option value="profile" ${isProfile ? 'selected' : ''}>Profile Engagement</option>
                        <option value="show" ${interactionState.mode === 'show' ? 'selected' : ''}>Show Engagement</option>
                    </select>
                </div>
                <div class="form-group mb-0">
                    <label class="form-label text-xs">${isProfile ? 'Profile' : 'Show'} ID</label>
                    <input class="input-field" type="number" id="int-id-input" value="${toast.esc(interactionState.id)}" placeholder="Enter ID...">
                </div>
                <button class="btn btn-primary btn-sm w-full col-span-2" id="int-load-btn"><i class="fas fa-sync mr-1.5 text-xs"></i>Load Engagement Metrics</button>
            </div>
        </div>`;

        if (!interactionState.id && !interactionState.loading) {
            h += Comp.emptyState('fa-chart-bar', 'Enter a Profile ID or Show ID above to pull interaction logs.');
            return h;
        }

        if (interactionState.loading) { h += Comp.pageLoader(); return h; }

        // Summary Card for Profile Engagement
        if (interactionState.summary && isProfile) {
            const s = interactionState.summary;
            
            // Map types into nice badge rows
            const breakdown = Object.entries(s.interactionsByType || {}).map(([type, val]) => `
                <div class="flex items-center justify-between p-2 rounded bg-vs-surface border border-vs-border">
                    <span class="text-xs font-semibold text-vs-text">${toast.esc(type)}</span>
                    <span class="badge badge-info text-xxs font-bold font-mono">${val} clicks</span>
                </div>
            `).join('');

            h += `<div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6 font-body">
                <div class="card p-5 border border-vs-border shadow-md col-span-1 space-y-4">
                    <h3 class="font-bold text-sm text-vs-text border-b border-vs-border pb-2"><i class="fas fa-tachometer-alt text-vs-accent mr-1"></i> Aggregate Counts</h3>
                    <div class="grid grid-cols-1 gap-3">
                        ${Comp.statCard(s.totalInteractions, 'Total Actions', 'fa-mouse-pointer', 'text-vs-accent')}
                    </div>
                </div>
                <div class="card p-5 border border-vs-border shadow-md col-span-1 space-y-3">
                    <h3 class="font-bold text-sm text-vs-text border-b border-vs-border pb-2"><i class="fas fa-chart-pie text-vs-accent mr-1"></i> Type Breakdown</h3>
                    <div class="space-y-1.5 max-h-40 overflow-y-auto">
                        ${breakdown || '<span class="text-xs text-muted">No actions tracked</span>'}
                    </div>
                </div>
                <div class="card p-5 border border-vs-border shadow-md col-span-1 space-y-3">
                    <h3 class="font-bold text-sm text-vs-text border-b border-vs-border pb-2"><i class="fas fa-fire text-vs-accent mr-1"></i> Top Engaged Content</h3>
                    <div class="space-y-1.5 max-h-40 overflow-y-auto">
                        ${s.topShows?.length ? s.topShows.map((sh, idx) => `
                            <div class="flex items-center justify-between py-1.5 border-b border-vs-border/50 text-xs">
                                <span class="truncate font-semibold text-vs-text max-w-[150px]">${idx + 1}. ${toast.esc(sh.showTitle)}</span>
                                <span class="text-vs-muted font-mono font-semibold">${sh.interactionCount} clicks</span>
                            </div>
                        `).join('') : '<span class="text-xs text-muted">No content history</span>'}
                    </div>
                </div>
            </div>`;
        }

        // Data table
        if (!interactionState.data?.items?.length) { h += Comp.emptyState('fa-mouse-pointer', 'No user interaction records found.'); return h; }
        const rows = interactionState.data.items.map(i => `<tr>
            <td class="text-muted font-mono text-xs">#${i.id}</td>
            <td class="font-semibold text-xs text-vs-text">${toast.esc(isProfile ? i.showTitle : i.profileName)}</td>
            <td><span class="badge badge-info text-xxs font-bold uppercase font-mono">${toast.esc(i.interactionType)}</span></td>
            <td class="text-muted text-xs font-mono">${utils.formatDate(i.createdAt || i.interactedAt)}</td>
        </tr>`).join('');
        
        h += Comp.dataTable(
            [
                { key: 'id', label: 'ID' },
                { key: isProfile ? 'showTitle' : 'profileName', label: isProfile ? 'Engaged Show' : 'Profile' },
                { key: 'interactionType', label: 'Action Type' },
                { key: 'createdAt', label: 'Timestamp' }
            ],
            rows,
            'No interactions found.',
            {
                tableId: 'interactions-table'
            }
        );
        h += Comp.pagination(interactionState.data.pageNumber, interactionState.data.totalPages, p => { interactionState.page = p; loadInteractions(); });
        return h;
    }

    function bindEvents() {
        document.getElementById('tab-playback-btn')?.addEventListener('click', () => { activeTab = 'playback'; render(); });
        document.getElementById('tab-interactions-btn')?.addEventListener('click', () => { activeTab = 'interactions'; render(); });
        
        // Playback filter
        document.getElementById('play-filter-btn')?.addEventListener('click', () => {
            playbackState.episodeId = document.getElementById('play-episode-id').value.trim();
            playbackState.profileId = document.getElementById('play-profile-id').value.trim();
            playbackState.page = 1; loadPlayback();
        });

        // Interactions filter
        document.getElementById('int-load-btn')?.addEventListener('click', () => {
            const val = document.getElementById('int-id-input').value.trim();
            if (val) {
                interactionState.id = val;
                interactionState.page = 1;
                loadInteractions();
            } else {
                toast.warn('Please enter an ID');
            }
        });
        
        document.getElementById('int-mode-select')?.addEventListener('change', e => {
            interactionState.mode = e.target.value;
            interactionState.data = null; 
            interactionState.summary = null;
            render();
        });
    }

    return {
        render() { return '<div id="admin-analytics-content">' + Comp.pageLoader() + '</div>'; },
        init() { activeTab = 'playback'; loadPlayback(); }
    };
})();