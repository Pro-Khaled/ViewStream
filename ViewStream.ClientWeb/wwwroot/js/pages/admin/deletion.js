pages.adminDeletion = (() => {
    let state = { page: 1, data: null, loading: true };
    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/admin/data-deletion-requests', { page: state.page, pageSize: CONFIG.PAGE_SIZE });
        } catch (err) { toast.error('Failed to load deletion requests'); }
        state.loading = false; render();
    }
    function render() {
        const c = document.getElementById('admin-deletion-content');
        if (!c) return;
        let h = Comp.pageHeader('Data Deletion Requests');
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-trash-alt', 'No requests'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Email</th><th>Status</th><th>Requested At</th><th></th></tr></thead><tbody>` +
                state.data.items.map(r => `<tr>
                    <td class="text-muted">${r.id}</td>
                    <td class="font-medium">${toast.esc(r.userEmail)}</td>
                    <td>${utils.statusBadge(r.status)}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(r.requestedAt)}</td>
                    <td><button class="btn btn-ghost btn-sm view-dr-btn" data-id="${r.id}"><i class="fas fa-eye"></i></button></td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }
    function bindEvents() {
        document.querySelectorAll('.view-dr-btn').forEach(b => b.addEventListener('click', () => showDetail(parseInt(b.dataset.id))));
    }
    async function showDetail(id) {
        modal.open('Deletion Request #' + id, Comp.pageLoader());
        try {
            const r = await api.get('/admin/data-deletion-requests/' + id);
            modal.open('Deletion Request #' + r.id,
                `<div class="detail-grid">
                    ${Comp.detailRow('ID', r.id)}
                    ${Comp.detailRow('User Email', toast.esc(r.userEmail))}
                    ${Comp.detailRow('Status', utils.statusBadge(r.status))}
                    ${Comp.detailRow('Requested At', utils.formatDateShort(r.requestedAt))}
                    ${Comp.detailRow('Confirmation Code', '<span class="font-mono">' + toast.esc(r.confirmationCode || '—') + '</span>')}
                </div>`,
                {
                    footer: `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                        <select class="input-field" id="dr-status-select" style="width:auto">
                            ${['Pending', 'Processing', 'Completed', 'Rejected'].map(s => `<option value="${s}" ${r.status === s ? 'selected' : ''}>${s}</option>`).join('')}
                        </select>
                        <button class="btn btn-primary" id="dr-update">Update</button>`
                }
            );
            document.getElementById('dr-update')?.addEventListener('click', async () => {
                await api.put(`/admin/data-deletion-requests/${id}`, { status: document.getElementById('dr-status-select').value });
                toast.success('Request updated');
                modal.close();
                loadData();
            });
        } catch (err) { toast.error('Failed: ' + err.message); modal.close(); }
    }
    return {
        render() { return '<div id="admin-deletion-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();