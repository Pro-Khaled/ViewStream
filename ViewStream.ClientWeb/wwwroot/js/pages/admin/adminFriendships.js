pages.adminFriendships = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true, sortKey: 'createdAt', sortDir: 'desc', userId: '', friendId: '', status: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/friendships', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                includeDeleted: state.includeDeleted,
                userId: state.userId ? parseInt(state.userId) : undefined,
                friendId: state.friendId ? parseInt(state.friendId) : undefined,
                status: state.status || undefined
            });
        } catch (err) { toast.error('Failed to load friendships: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Friendships Management', 'Monitor and moderate user friendships');
        
        h += `<div class="card p-4 mb-6 font-body">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
                <div class="form-group mb-0 w-full">
                    <label class="form-label">User ID</label>
                    <input type="number" class="input-field" id="friend-user-id" value="${toast.esc(state.userId)}" placeholder="Filter by User ID">
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Friend ID</label>
                    <input type="number" class="input-field" id="friend-friend-id" value="${toast.esc(state.friendId)}" placeholder="Filter by Friend ID">
                </div>
                <div class="form-group mb-0 w-full">
                    <label class="form-label">Status Filter</label>
                    <select class="input-field" id="friend-status-filter">
                        <option value="">All Statuses</option>
                        <option value="pending" ${state.status === 'pending' ? 'selected' : ''}>Pending</option>
                        <option value="accepted" ${state.status === 'accepted' ? 'selected' : ''}>Accepted</option>
                        <option value="blocked" ${state.status === 'blocked' ? 'selected' : ''}>Blocked</option>
                    </select>
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="friend-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="friend-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <div class="col-span-1 md:col-span-4 text-right">
                    <button class="btn btn-primary btn-sm w-full md:w-auto" id="friend-search-btn">Filter Friendships</button>
                </div>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-users', 'No friendships found'); }
        else {
            const rows = state.data.items.map(f => {
                let statusBadge = `<span class="badge bg-vs-warning/10 text-vs-warning font-semibold uppercase">${toast.esc(f.status)}</span>`;
                if (f.status?.toLowerCase() === 'accepted') {
                    statusBadge = `<span class="badge bg-vs-success/10 text-vs-success font-semibold uppercase">${toast.esc(f.status)}</span>`;
                } else if (f.status?.toLowerCase() === 'blocked') {
                    statusBadge = `<span class="badge bg-vs-error/10 text-vs-error font-semibold uppercase">${toast.esc(f.status)}</span>`;
                }
                
                if (f.isDeleted) statusBadge += ` <span class="badge badge-danger ml-1">Deleted</span>`;

                return `<tr class="${f.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="font-bold text-vs-text">User #${f.userId} <span class="text-xs text-vs-muted">(${toast.esc(f.userEmail || '')})</span></td>
                    <td class="font-bold text-vs-text">Friend #${f.friendId} <span class="text-xs text-vs-muted">(${toast.esc(f.friendEmail || '')})</span></td>
                    <td>${statusBadge}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(f.createdAt)}</td>
                    <td class="text-right">
                        <button class="btn btn-ghost btn-sm text-vs-error delete-friendship-btn" data-user-id="${f.userId}" data-friend-id="${f.friendId}" title="Delete Friendship"><i class="fas fa-trash-alt text-xs"></i></button>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'userId', label: 'User ID' },
                    { key: 'friendId', label: 'Friend ID' },
                    { key: 'status', label: 'Status' },
                    { key: 'createdAt', label: 'Established At' },
                    { key: '', label: '' }
                ],
                rows,
                'No friendships found',
                {
                    tableId: 'friendships-table',
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
        document.getElementById('friend-search-btn')?.addEventListener('click', () => {
            state.userId = document.getElementById('friend-user-id').value.trim();
            state.friendId = document.getElementById('friend-friend-id').value.trim();
            state.status = document.getElementById('friend-status-filter').value;
            state.includeDeleted = document.getElementById('friend-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('friend-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.querySelectorAll('.delete-friendship-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.userId), parseInt(btn.dataset.friendId)));
        });
    }

    function showDeleteModal(userId, friendId) {
        modal.open('Delete Friendship',
            `<p class="text-sm text-vs-dim">Are you sure you want to permanently delete the friendship between User <strong class="text-vs-text">#${userId}</strong> and Friend <strong class="text-vs-text">#${friendId}</strong>?</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-friendship">Delete Friendship</button>`
        );

        document.getElementById('confirm-delete-friendship')?.addEventListener('click', async () => {
            try {
                await api.del(`/admin/friendships/${userId}/${friendId}`);
                toast.success('Friendship deleted successfully');
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
