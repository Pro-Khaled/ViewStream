pages.adminCommentLikes = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc', commentId: '', profileId: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/commentlikes', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                includeDeleted: state.includeDeleted,
                commentId: state.commentId ? parseInt(state.commentId) : undefined,
                profileId: state.profileId ? parseInt(state.profileId) : undefined
            });
        } catch (err) { toast.error('Failed to load comment likes: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Comment Likes & Reactions', 'Manage comment reactions', 
            '<button class="btn btn-primary btn-sm" id="add-comment-like-btn"><i class="fas fa-plus mr-1.5"></i> Add Like/Reaction</button>');
        
        h += `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                <div class="form-group mb-0 w-full col-span-2">
                    <label class="form-label">Search Query</label>
                    <input class="input-field" id="like-search" value="${toast.esc(state.search)}" placeholder="Search reactions...">
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Comment ID</label>
                    <input type="number" class="input-field" id="like-comment-id" value="${toast.esc(state.commentId)}" placeholder="Filter by comment ID">
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Profile ID</label>
                    <input type="number" class="input-field" id="like-profile-id" value="${toast.esc(state.profileId)}" placeholder="Filter by profile ID">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="like-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="like-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <div class="col-span-1 md:col-span-3 text-right">
                    <button class="btn btn-primary btn-sm w-full md:w-auto" id="like-search-btn">Filter Reactions</button>
                </div>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-thumbs-up', 'No reactions found'); }
        else {
            const rows = state.data.items.map(l => {
                let rxBadge = `<span class="badge bg-vs-accent/10 text-vs-accent uppercase font-bold"><i class="fas fa-heart mr-1 text-[10px]"></i> ${toast.esc(l.reactionType)}</span>`;
                if (l.reactionType === 'like') {
                    rxBadge = `<span class="badge bg-vs-info/10 text-vs-info uppercase font-bold"><i class="fas fa-thumbs-up mr-1 text-[10px]"></i> ${toast.esc(l.reactionType)}</span>`;
                } else if (l.reactionType === 'dislike') {
                    rxBadge = `<span class="badge bg-vs-error/10 text-vs-error uppercase font-bold"><i class="fas fa-thumbs-down mr-1 text-[10px]"></i> ${toast.esc(l.reactionType)}</span>`;
                }

                return `<tr class="${l.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted font-semibold">Comment #${l.commentId}</td>
                    <td class="font-bold text-vs-text">${toast.esc(l.profileName || 'Profile #' + l.profileId)} <span class="text-xs text-vs-muted">(#${l.profileId})</span></td>
                    <td>${rxBadge}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(l.createdAt)}</td>
                    <td class="text-right">
                        <button class="btn btn-ghost btn-sm text-vs-error delete-like-btn" data-comment-id="${l.commentId}" data-profile-id="${l.profileId}" title="Delete Reaction"><i class="fas fa-trash-alt text-xs"></i></button>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'commentId', label: 'Comment' },
                    { key: 'profileId', label: 'Profile' },
                    { key: 'reactionType', label: 'Reaction' },
                    { key: 'createdAt', label: 'Created At' },
                    { key: '', label: '' }
                ],
                rows,
                'No reactions found',
                {
                    tableId: 'comment-likes-table',
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
        document.getElementById('like-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('like-search').value.trim();
            state.commentId = document.getElementById('like-comment-id').value.trim();
            state.profileId = document.getElementById('like-profile-id').value.trim();
            state.includeDeleted = document.getElementById('like-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('like-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.getElementById('add-comment-like-btn')?.addEventListener('click', showCreateModal);

        document.querySelectorAll('.delete-like-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.commentId), parseInt(btn.dataset.profileId)));
        });
    }

    function showCreateModal() {
        modal.open('Add Comment Like / Reaction',
            `<div class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Comment ID <span class="text-vs-error">*</span></label>
                    <input type="number" class="input-field" id="new-like-comment-id" placeholder="e.g. 1042">
                </div>
                <div class="form-group">
                    <label class="form-label">Profile ID <span class="text-vs-error">*</span></label>
                    <input type="number" class="input-field" id="new-like-profile-id" placeholder="e.g. 5">
                </div>
                <div class="form-group">
                    <label class="form-label">Reaction Type</label>
                    <select class="input-field" id="new-like-reaction">
                        <option value="like">Like (Thumbs Up)</option>
                        <option value="dislike">Dislike (Thumbs Down)</option>
                        <option value="love">Love (Heart)</option>
                    </select>
                </div>
            </div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-new-like"><i class="fas fa-plus mr-1"></i> Add Reaction</button>`
        );

        document.getElementById('save-new-like')?.addEventListener('click', async () => {
            const commentId = parseInt(document.getElementById('new-like-comment-id').value);
            const profileId = parseInt(document.getElementById('new-like-profile-id').value);
            const reactionType = document.getElementById('new-like-reaction').value;

            if (!commentId) { toast.warning('Comment ID is required'); return; }
            if (!profileId) { toast.warning('Profile ID is required'); return; }

            try {
                await api.post('/admin/commentlikes', { commentId, profileId, reactionType });
                toast.success('Reaction saved successfully');
                modal.close();
                loadData();
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(commentId, profileId) {
        modal.open('Delete Reaction',
            `<p class="text-sm text-vs-dim">Are you sure you want to delete comment reaction for comment <strong class="text-vs-text">#${commentId}</strong> by profile <strong class="text-vs-text">#${profileId}</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-like">Delete</button>`
        );

        document.getElementById('confirm-delete-like')?.addEventListener('click', async () => {
            try {
                await api.del(`/admin/commentlikes/${commentId}/${profileId}`);
                toast.success('Reaction deleted successfully');
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
