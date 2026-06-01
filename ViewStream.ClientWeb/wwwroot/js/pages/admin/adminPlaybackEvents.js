pages.adminPlaybackEvents = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'eventTime', sortDir: 'desc' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/playback-events', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load playback events: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function eventBadge(type) {
        const map = { Play: 'badge-success', Pause: 'badge-warning', Stop: 'badge-danger', Seek: 'badge-info', Complete: 'badge-success', Error: 'badge-danger', BufferStart: 'badge-warning', BufferEnd: 'badge-info' };
        return `<span class="badge ${map[type] || 'badge-muted'}">${utils.esc(type || 'Unknown')}</span>`;
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Playback Events', 'Analyze streaming playback telemetry and events');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Events</label>
                    <input class="input-field" id="pe-search" value="${utils.esc(state.search)}" placeholder="Search by user email or content...">
                </div>
                <button class="btn btn-primary btn-sm" id="pe-search-btn">Search</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-play-circle', 'No playback events found'); }
        else {
            const rows = state.data.items.map(e => `<tr>
                <td class="text-muted font-mono">#${e.id}</td>
                <td><span class="font-medium">${utils.esc(e.userEmail || '—')}</span></td>
                <td class="text-sm">${utils.esc(e.contentTitle || '—')}</td>
                <td>${eventBadge(e.eventType)}</td>
                <td class="text-muted text-sm font-mono">${e.positionSeconds != null ? utils.formatDuration(e.positionSeconds) : '—'}</td>
                <td class="text-muted text-sm">${utils.esc(e.deviceType || '—')}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(e.eventTime)}</td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'id', label: 'ID' }, { key: 'userEmail', label: 'User' }, { key: 'contentTitle', label: 'Content' }, { key: 'eventType', label: 'Event' }, { key: 'positionSeconds', label: 'Position' }, { key: 'deviceType', label: 'Device' }, { key: 'eventTime', label: 'Time' }],
                rows, 'No events found',
                { tableId: 'pe-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('pe-search-btn')?.addEventListener('click', () => { state.search = document.getElementById('pe-search').value.trim(); state.page = 1; loadData(); });
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
