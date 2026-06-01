pages.adminEpisodeComments = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true, sortKey: 'id', sortDir: 'desc', episodeId: '', profileId: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/episodecomments', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                includeDeleted: state.includeDeleted,
                episodeId: state.episodeId ? parseInt(state.episodeId) : undefined,
                profileId: state.profileId ? parseInt(state.profileId) : undefined
            });
        } catch (err) { toast.error('Failed to load comments: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Episode Comments Moderation', 'Moderate user comments on episodes');
        
        h += `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                <div class="form-group mb-0 w-full col-span-2">
                    <label class="form-label">Search Comments</label>
                    <input class="input-field" id="comm-search" value="${toast.esc(state.search)}" placeholder="Search comment content...">
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Episode ID</label>
                    <input type="number" class="input-field" id="comm-episode-id" value="${toast.esc(state.episodeId)}" placeholder="Filter by Episode ID">
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Profile ID</label>
                    <input type="number" class="input-field" id="comm-profile-id" value="${toast.esc(state.profileId)}" placeholder="Filter by Profile ID">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="comm-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="comm-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <div class="col-span-1 md:col-span-3 text-right">
                    <button class="btn btn-primary btn-sm w-full md:w-auto" id="comm-search-btn">Filter Comments</button>
                </div>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-comments', 'No comments found'); }
        else {
            const rows = state.data.items.map(t => {
                let statusBadge = `<span class="badge badge-success">Active</span>`;
                if (t.isDeleted) statusBadge = `<span class="badge badge-danger">Deleted</span>`;

                return `<tr class="${t.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">#${t.id}</td>
                    <td class="font-semibold text-vs-dim">Episode #${t.episodeId}</td>
                    <td class="font-bold text-vs-text">${toast.esc(t.profileName || 'Profile #' + t.profileId)} <span class="text-xs text-vs-muted">(#${t.profileId})</span></td>
                    <td class="text-vs-dim text-sm max-w-[300px] truncate" title="${toast.esc(t.content)}">${toast.esc(t.content)}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(t.createdAt)}</td>
                    <td>${statusBadge}</td>
                    <td class="text-right">
                        <div class="flex items-center justify-end gap-1.5">
                            ${t.isDeleted ? 
                                `<button class="btn btn-ghost btn-sm text-vs-success restore-comm-btn" data-id="${t.id}" title="Restore Comment"><i class="fas fa-trash-restore text-xs"></i></button>` : 
                                `<button class="btn btn-ghost btn-sm text-vs-error delete-comm-btn" data-id="${t.id}" data-episode-id="${t.episodeId}" title="Delete Comment"><i class="fas fa-trash-alt text-xs"></i></button>`
                            }
                        </div>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'episodeId', label: 'Episode ID' },
                    { key: 'profileId', label: 'Profile' },
                    { key: 'content', label: 'Comment Content' },
                    { key: 'createdAt', label: 'Posted At' },
                    { key: 'isDeleted', label: 'Status' },
                    { key: '', label: '' }
                ],
                rows,
                'No comments found',
                {
                    tableId: 'episode-comments-table',
                    sortKey: state.sortKey,
                    sortDir: state.sortDir,
                    onSort: (key, dir) => { if (key) { state.sortKey = key; state.sortDir = dir; loadData(); } }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('comm-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('comm-search').value.trim();
            state.episodeId = document.getElementById('comm-episode-id').value.trim();
            state.profileId = document.getElementById('comm-profile-id').value.trim();
            state.includeDeleted = document.getElementById('comm-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('comm-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.querySelectorAll('.delete-comm-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), parseInt(btn.dataset.episodeId)));
        });

        document.querySelectorAll('.restore-comm-btn').forEach(btn => {
            btn.addEventListener('click', () => showRestoreModal(parseInt(btn.dataset.id)));
        });
    }

    function showDeleteModal(id, episodeId) {
        modal.open('Delete Episode Comment',
            `<p class="text-sm text-vs-dim">Are you sure you want to soft-delete comment <strong class="text-vs-text">#${id}</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-comm">Delete Comment</button>`
        );

        document.getElementById('confirm-delete-comm')?.addEventListener('click', async () => {
            try {
                await api.del(`/episodes/${episodeId}/comments/${id}`);
                toast.success('Comment soft-deleted successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showRestoreModal(id) {
        modal.open('Restore Episode Comment',
            `<p class="text-sm text-vs-dim">Are you sure you want to restore comment <strong class="text-vs-text">#${id}</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-success" id="confirm-restore-comm">Restore Comment</button>`
        );

        document.getElementById('confirm-restore-comm')?.addEventListener('click', async () => {
            try {
                await api.post(`/admin/episodecomments/${id}/restore`);
                toast.success('Comment restored successfully');
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
