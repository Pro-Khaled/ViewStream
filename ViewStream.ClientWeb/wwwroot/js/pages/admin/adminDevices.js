pages.adminDevices = (() => {
    let state = { page: 1, search: '', includeDeleted: false, data: null, loading: true, sortKey: 'id', sortDir: 'asc', userId: '' };

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/devices', {
                pageNumber: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                searchTerm: state.search || undefined,
                sortBy: state.sortKey,
                sortDescending: state.sortDir === 'desc',
                includeDeleted: state.includeDeleted,
                userId: state.userId ? parseInt(state.userId) : undefined
            });
        } catch (err) { toast.error('Failed to load devices: ' + err.message); state.data = null; }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        let h = Comp.pageHeader('Registered Devices Management', 'Monitor and manage user login devices');
        
        h += `<div class="card p-4 mb-6 font-body">
            <div class="flex flex-col md:flex-row items-end gap-4">
                <div class="flex-1 form-group mb-0 w-full">
                    <label class="form-label">Search Devices</label>
                    <input class="input-field" id="device-search" value="${toast.esc(state.search)}" placeholder="Search by device name, platform, browser...">
                </div>
                <div class="form-group mb-0 w-full md:w-48">
                    <label class="form-label">User ID</label>
                    <input type="number" class="input-field" id="device-user-id" value="${toast.esc(state.userId)}" placeholder="Filter by User ID">
                </div>
                <div class="flex items-center gap-2 mb-2">
                    <input type="checkbox" id="device-include-deleted" ${state.includeDeleted ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                    <label for="device-include-deleted" class="text-sm text-vs-dim cursor-pointer select-none">Include Deleted</label>
                </div>
                <button class="btn btn-primary btn-sm w-full md:w-auto" id="device-search-btn">Filter Devices</button>
            </div>
        </div>`;

        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-tablet-alt', 'No devices found'); }
        else {
            const rows = state.data.items.map(d => {
                let statusBadge = d.isDeleted 
                    ? `<span class="badge badge-danger">Deleted</span>`
                    : d.isActive !== false ? `<span class="badge badge-success">Active</span>` : `<span class="badge badge-secondary">Inactive</span>`;
                
                const platIcon = d.platform?.toLowerCase().includes('win') ? 'fab fa-windows' :
                               d.platform?.toLowerCase().includes('mac') || d.platform?.toLowerCase().includes('ios') ? 'fab fa-apple' :
                               d.platform?.toLowerCase().includes('android') ? 'fab fa-android' : 'fas fa-laptop';

                return `<tr class="${d.isDeleted ? 'opacity-60 bg-vs-danger/5' : ''}">
                    <td class="text-muted">#${d.id}</td>
                    <td class="font-semibold text-vs-dim">User #${d.userId}</td>
                    <td class="font-bold text-vs-text"><i class="${platIcon} mr-1.5 text-vs-accent"></i>${toast.esc(d.deviceName || 'Unknown Device')}</td>
                    <td class="font-medium text-vs-accent">${toast.esc(d.platform || '—')}</td>
                    <td class="text-muted text-sm">${utils.formatDate(d.lastActiveAt || d.lastActive)}</td>
                    <td>${statusBadge}</td>
                    <td class="text-right">
                        <button class="btn btn-ghost btn-sm text-vs-error delete-device-btn" data-id="${d.id}" data-name="${toast.esc(d.deviceName)}" title="Revoke & Delete Device"><i class="fas fa-trash-alt text-xs"></i></button>
                    </td>
                </tr>`;
            }).join('');

            h += Comp.dataTable(
                [
                    { key: 'id', label: 'ID' },
                    { key: 'userId', label: 'User ID' },
                    { key: 'deviceName', label: 'Device Name' },
                    { key: 'platform', label: 'Platform' },
                    { key: 'lastActive', label: 'Last Active' },
                    { key: '', label: 'Status' },
                    { key: '', label: '' }
                ],
                rows,
                'No devices found',
                {
                    tableId: 'devices-table',
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
        document.getElementById('device-search-btn')?.addEventListener('click', () => {
            state.search = document.getElementById('device-search').value.trim();
            state.userId = document.getElementById('device-user-id').value.trim();
            state.includeDeleted = document.getElementById('device-include-deleted').checked;
            state.page = 1; loadData();
        });

        document.getElementById('device-include-deleted')?.addEventListener('change', (e) => {
            state.includeDeleted = e.target.checked;
            state.page = 1; loadData();
        });

        document.querySelectorAll('.delete-device-btn').forEach(btn => {
            btn.addEventListener('click', () => showDeleteModal(parseInt(btn.dataset.id), btn.dataset.name));
        });
    }

    function showDeleteModal(id, name) {
        modal.open('Delete Device',
            `<p class="text-sm text-vs-dim">Are you sure you want to permanently delete and revoke device <strong class="text-vs-text">"${toast.esc(name)}"</strong>? The associated session will be invalidated.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-delete-device">Delete Device</button>`
        );

        document.getElementById('confirm-delete-device')?.addEventListener('click', async () => {
            try {
                await api.del(`/admin/devices/${id}`);
                toast.success('Device hard-deleted successfully');
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
