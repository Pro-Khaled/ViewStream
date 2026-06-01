pages.adminItemVectors = (() => {
    let state = { page: 1, search: '', showId: '', includeDeleted: false, data: null, loading: true };
    let sortKey = 'ShowTitle';
    let sortDir = 'asc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/itemvectors', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: sortKey,
                sortDescending: sortDir === 'desc',
                includeDeleted: state.includeDeleted,
                showId: state.showId ? parseInt(state.showId) : undefined
            });
        } catch (err) {
            toast.error('Failed to load item vectors: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Item Vector Management', '', '');
        h += `<div class="card p-4 mb-6">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search</label>
                    <input class="input-field" id="vector-search" value="${toast.esc(state.search)}" placeholder="Search by show title...">
                </div>
                <div class="w-full md:w-32 form-group mb-0">
                    <label class="form-label">Show ID</label>
                    <input type="number" class="input-field" id="vector-show-id" value="${state.showId || ''}" placeholder="Show ID">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="vector-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="vector-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="vector-search-btn">Search</button>
            </div>
        </div>`;

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data?.items?.length) {
            h += Comp.emptyState('fa-database', 'No item vectors found');
        } else {
            const rows = state.data.items.map(a => {
                const preview = a.EmbeddingJson ? (a.EmbeddingJson.length > 80 ? a.EmbeddingJson.substring(0, 80) + '...' : a.EmbeddingJson) : 'â€”';
                const formattedDate = a.LastUpdated ? new Date(a.LastUpdated).toLocaleString() : 'â€”';
                return `<tr>
                    <td class="text-muted">${a.ShowId}</td>
                    <td class="font-bold text-vs-text">${toast.esc(a.ShowTitle)}</td>
                    <td class="text-sm text-vs-dim font-mono max-w-[300px] truncate" title="${toast.esc(a.EmbeddingJson || '')}">${toast.esc(preview)}</td>
                    <td class="text-vs-dim text-sm">${formattedDate}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            <button class="btn btn-ghost btn-sm view-vector-btn" data-id="${a.ShowId}" data-title="${toast.esc(a.ShowTitle)}" data-json="${toast.esc(a.EmbeddingJson || '')}" title="View Full JSON"><i class="fas fa-eye text-xs"></i></button>
                            <button class="btn btn-ghost btn-sm edit-vector-btn" data-id="${a.ShowId}" data-title="${toast.esc(a.ShowTitle)}" data-json="${toast.esc(a.EmbeddingJson || '')}" title="Update Vector"><i class="fas fa-edit text-xs"></i></button>
                        </div>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'ShowId', label: 'Show ID' },
                    { key: 'ShowTitle', label: 'Show Title' },
                    { key: 'EmbeddingJson', label: 'Embedding Vector' },
                    { key: 'LastUpdated', label: 'Last Updated' },
                    { key: '', label: '' }
                ],
                rows,
                'No item vectors found',
                {
                    tableId: 'item-vectors-table',
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
        document.getElementById('vector-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('vector-search').value.trim();
            state.showId = document.getElementById('vector-show-id').value.trim();
            state.includeDeleted = document.getElementById('vector-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('vector-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.querySelectorAll('.view-vector-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.dataset.id;
                const title = btn.dataset.title;
                const rawJson = btn.dataset.json;
                showViewModal(id, title, rawJson);
            });
        });

        document.querySelectorAll('.edit-vector-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.dataset.id;
                const title = btn.dataset.title;
                const rawJson = btn.dataset.json;
                showUpdateModal(id, title, rawJson);
            });
        });
    }

    function showViewModal(id, title, rawJson) {
        let formatted = rawJson;
        try {
            if (rawJson) {
                const parsed = JSON.parse(rawJson);
                formatted = JSON.stringify(parsed, null, 2);
            }
        } catch (e) {}

        modal.open(`View Vector: ${title} (ID: ${id})`,
            `<div class="space-y-4">
                <div class="form-group mb-0">
                    <label class="form-label">Embedding Vector JSON</label>
                    <pre class="bg-vs-surface border border-vs-border p-3 rounded text-xs font-mono overflow-auto max-h-[50vh] text-vs-dim whitespace-pre-wrap">${toast.esc(formatted || '[]')}</pre>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Close</button>`
        );
    }

    function showUpdateModal(id, title, rawJson) {
        let formatted = rawJson;
        try {
            if (rawJson) {
                const parsed = JSON.parse(rawJson);
                formatted = JSON.stringify(parsed, null, 2);
            }
        } catch (e) {}

        modal.open(`Update Vector: ${title}`,
            `<div class="space-y-4">
                <p class="text-sm text-vs-dim">Provide a new embedding vector (JSON array of floats) for <strong class="text-vs-text">${toast.esc(title)}</strong>.</p>
                <div class="form-group">
                    <label class="form-label">Embedding JSON <span class="text-vs-error">*</span></label>
                    <textarea class="input-field font-mono text-xs h-48" id="edit-vector-json" placeholder="e.g. [0.012, -0.415, 0.982, ...]">${toast.esc(formatted || '')}</textarea>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-vector"><i class="fas fa-save mr-1"></i> Save Vector</button>`
        );

        document.getElementById('save-vector')?.addEventListener('click', async () => {
            const rawInput = document.getElementById('edit-vector-json').value.trim();
            if (!rawInput) {
                toast.warning('Embedding JSON is required');
                return;
            }

            // Simple validation that it is a valid JSON array or object
            try {
                JSON.parse(rawInput);
            } catch (e) {
                toast.error('Invalid JSON format. Please ensure it is a valid JSON array or object.');
                return;
            }

            try {
                await api.post(`/shows/${id}/vector`, {
                    embeddingJson: rawInput
                });
                toast.success('Show vector updated successfully');
                modal.close();
                loadData();
            } catch (err) {
                toast.error('Failed to update vector: ' + err.message);
            }
        });
    }

    return {
        render() { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();