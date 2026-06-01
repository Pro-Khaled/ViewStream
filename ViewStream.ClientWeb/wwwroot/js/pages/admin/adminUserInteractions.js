pages.adminUserInteractions = (() => {
    let state = { page: 1, search: '', data: null, loading: true, sortKey: 'interactedAt', sortDir: 'desc', typeFilter: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/user-interactions', {
                pageNumber: state.page, pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                interactionType: state.typeFilter || undefined,
                sortBy: state.sortKey, sortDescending: state.sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load interactions: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function typeBadge(type) {
        const map = { Like: 'badge-success', Dislike: 'badge-danger', Share: 'badge-info', Save: 'badge-warning', View: 'badge-muted' };
        return `<span class="badge ${map[type] || 'badge-muted'}">${utils.esc(type || 'Unknown')}</span>`;
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('User Interactions', 'Track likes, shares, saves, and other user engagement');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Interactions</label>
                    <input class="input-field" id="ui-search" value="${utils.esc(state.search)}" placeholder="Search by user or content...">
                </div>
                <div class="form-group mb-0">
                    <label class="form-label">Type</label>
                    <select class="input-field" id="ui-type-filter">
                        <option value="">All Types</option>
                        ${['Like','Dislike','Share','Save','View'].map(t => `<option value="${t}" ${state.typeFilter === t ? 'selected' : ''}>${t}</option>`).join('')}
                    </select>
                </div>
                <button class="btn btn-primary btn-sm" id="ui-search-btn">Apply</button>
            </div>
        </div>`;
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-heart', 'No interactions found'); }
        else {
            const rows = state.data.items.map(i => `<tr>
                <td class="font-medium">${utils.esc(i.userEmail || '—')}</td>
                <td>${utils.esc(i.contentTitle || '—')}</td>
                <td><span class="badge badge-info">${utils.esc(i.contentType || 'Unknown')}</span></td>
                <td>${typeBadge(i.interactionType)}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(i.interactedAt)}</td>
            </tr>`).join('');
            h += Comp.dataTable(
                [{ key: 'userEmail', label: 'User' }, { key: 'contentTitle', label: 'Content' }, { key: 'contentType', label: 'Content Type' }, { key: 'interactionType', label: 'Action' }, { key: 'interactedAt', label: 'Time' }],
                rows, 'No interactions',
                { tableId: 'ui-table', sortKey: state.sortKey, sortDir: state.sortDir, onSort: (k, d) => { state.sortKey = k; state.sortDir = d; loadData(); } }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        document.getElementById('ui-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('ui-search').value.trim();
            state.typeFilter = document.getElementById('ui-type-filter').value;
            state.page = 1; loadData();
        });
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();
