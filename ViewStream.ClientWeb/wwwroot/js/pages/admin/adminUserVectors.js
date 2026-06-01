pages.adminUserVectors = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true };
    let sortKey = 'ProfileName';
    let sortDir = 'asc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/uservectors', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted
            });
        } catch (err) {
            toast.error('Failed to load user vectors: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('User Vector Management', '', '');
        
        // Custom info alert banner
        h += `<div class="bg-vs-surface border-l-4 border-vs-info p-4 mb-6 rounded-r">
            <div class="flex items-center gap-3">
                <i class="fas fa-info-circle text-vs-info text-lg"></i>
                <div class="text-sm text-vs-dim">
                    User vectors are auto-generated based on viewing history and interactions. Manual editing is not available.
                </div>
            </div>
        </div>`;

        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="user-vector-search" value="${toast.esc(state.search)}" placeholder="Search by profile name...">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="user-vector-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="user-vector-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="user-vector-search-btn">Search</button>
            </div>
        </div>`;

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data?.items?.length) {
            h += Comp.emptyState('fa-user-tag', 'No user vectors found');
        } else {
            const rows = state.data.items.map(a => {
                const preview = a.EmbeddingJson ? (a.EmbeddingJson.length > 80 ? a.EmbeddingJson.substring(0, 80) + '...' : a.EmbeddingJson) : 'â€”';
                const createdDate = a.CreatedAt ? new Date(a.CreatedAt).toLocaleString() : 'â€”';
                const updatedDate = a.LastUpdated ? new Date(a.LastUpdated).toLocaleString() : 'â€”';
                return `<tr>
                    <td class="text-muted">${a.ProfileId}</td>
                    <td class="font-bold text-vs-text">${toast.esc(a.ProfileName)}</td>
                    <td class="text-sm text-vs-dim font-mono max-w-[300px] truncate" title="${toast.esc(a.EmbeddingJson || '')}">${toast.esc(preview)}</td>
                    <td class="text-vs-dim text-sm">${createdDate}</td>
                    <td class="text-vs-dim text-sm">${updatedDate}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm view-vector-btn" data-id="${a.ProfileId}" data-name="${toast.esc(a.ProfileName)}" data-json="${toast.esc(a.EmbeddingJson || '')}" title="View Full JSON"><i class="fas fa-eye text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'ProfileId', label: 'Profile ID' },
                    { key: 'ProfileName', label: 'Profile Name' },
                    { key: 'EmbeddingJson', label: 'Embedding Vector' },
                    { key: 'CreatedAt', label: 'Created At' },
                    { key: 'LastUpdated', label: 'Last Updated' },
                    { key: '', label: '' }
                ],
                rows,
                'No user vectors found',
                {
                    tableId: 'user-vectors-table',
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
        document.getElementById('user-vector-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('user-vector-search').value.trim();
            state.includeDeleted = document.getElementById('user-vector-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('user-vector-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.querySelectorAll('.view-vector-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.dataset.id;
                const name = btn.dataset.name;
                const rawJson = btn.dataset.json;
                showViewModal(id, name, rawJson);
            });
        });
    }

    function showViewModal(id, name, rawJson) {
        let formatted = rawJson;
        try {
            if (rawJson) {
                const parsed = JSON.parse(rawJson);
                formatted = JSON.stringify(parsed, null, 2);
            }
        } catch (e) {}

        modal.open(`View User Vector: ${name} (Profile ID: ${id})`,
            `<div class="space-y-4">
                <div class="form-group mb-0">
                    <label class="form-label">Embedding Vector JSON</label>
                    <pre class="bg-vs-surface border border-vs-border p-3 rounded text-xs font-mono overflow-auto max-h-[50vh] text-vs-dim whitespace-pre-wrap">${toast.esc(formatted || '[]')}</pre>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Close</button>`
        );
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();