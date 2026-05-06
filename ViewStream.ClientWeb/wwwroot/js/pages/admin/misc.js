pages.adminMisc = (() => {
    let activeTab = 'invoices'; // 'invoices', 'notifications', 'deletion'
    let invoiceState = { id: '', invoice: null, loading: false };
    let deletionState = { page: 1, data: null, loading: true };

    async function loadInvoices() {
        // Not implemented as a list, just a lookup by ID
    }
    async function loadDeletions() {
        deletionState.loading = true; render();
        try { deletionState.data = await api.get('/admin/data-deletion-requests', { page: deletionState.page, pageSize: CONFIG.PAGE_SIZE }); }
        catch { toast.error('Failed to load deletion requests'); }
        deletionState.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-misc-content');
        if (!c) return;
        let h = `<div class="flex gap-1 mb-6 bg-vs-card rounded-lg p-1 w-fit">
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'invoices' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="invoices">Invoices</button>
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'notifications' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="notifications">Send Notification</button>
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'deletion' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="deletion">Data Deletion</button>
        </div>`;

        if (activeTab === 'invoices') h += renderInvoices();
        else if (activeTab === 'notifications') h += renderNotificationForm();
        else h += renderDeletions();

        c.innerHTML = h;
        bindEvents();
    }

    function renderInvoices() {
        return `<div class="max-w-lg">
            <h2 class="font-display font-semibold text-xl text-vs-text mb-4">Lookup Invoice</h2>
            <div class="flex gap-3 mb-4">
                <input type="number" id="invoice-id-input" class="input-field" placeholder="Invoice ID" style="flex:1">
                <button class="btn btn-primary" id="lookup-invoice-btn">View</button>
            </div>
            <div id="invoice-result"></div>
        </div>`;
    }

    function renderNotificationForm() {
        return `<div class="max-w-lg">
            <h2 class="font-display font-semibold text-xl text-vs-text mb-4">Send Notification to User</h2>
            <form id="admin-notify-form" class="space-y-4">
                <div><label class="form-label">User Email</label><input type="email" id="an-email" required class="input-field" placeholder="user@example.com"></div>
                <div><label class="form-label">Title</label><input type="text" id="an-title" required class="input-field" placeholder="Notification title"></div>
                <div><label class="form-label">Body</label><textarea id="an-body" rows="4" class="input-field" placeholder="Notification body..."></textarea></div>
                <div><label class="form-label">Type</label><select id="an-type" class="input-field"><option value="general">General</option><option value="new_episode">New Episode</option><option value="subscription">Subscription</option><option value="recommendation">Recommendation</option></select></div>
                <button type="submit" class="btn btn-primary w-full">Send</button>
            </form>
        </div>`;
    }

    function renderDeletions() {
        let h = Comp.pageHeader('Data Deletion Requests', 'Process user data deletion requests');
        h += Comp.filterBar([
            { key: 'status', label: 'Status', type: 'select', options: ['Pending', 'Processing', 'Completed', 'Rejected'] }
        ], vals => { deletionState.filters = vals; deletionState.page = 1; loadDeletions(); });

        if (deletionState.loading) { h += Comp.pageLoader(); }
        else if (!deletionState.data?.items?.length) { h += Comp.emptyState('fa-trash-alt', 'No requests'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Email</th><th>Status</th><th>Requested At</th><th></th></tr></thead><tbody>` +
                deletionState.data.items.map(r => `<tr>
                    <td class="text-muted">${r.id}</td>
                    <td class="font-medium">${toast.esc(r.userEmail)}</td>
                    <td>${utils.statusBadge(r.status)}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(r.requestedAt)}</td>
                    <td><button class="btn btn-ghost btn-sm view-dr-btn" data-id="${r.id}"><i class="fas fa-eye"></i></button></td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(deletionState.data.pageNumber, deletionState.data.totalPages, p => { deletionState.page = p; loadDeletions(); });
        }
        return h;
    }

    function bindEvents() {
        document.querySelectorAll('#admin-misc-content .tab-btn').forEach(b => {
            b.addEventListener('click', () => {
                activeTab = b.dataset.tab;
                if (activeTab === 'deletion' && !deletionState.data) loadDeletions();
                render();
            });
        });

        // Invoice lookup
        document.getElementById('lookup-invoice-btn')?.addEventListener('click', async () => {
            const id = document.getElementById('invoice-id-input').value;
            if (!id) return;
            try {
                const inv = await api.get(`/admin/invoices/${id}`);
                document.getElementById('invoice-result').innerHTML = `<div class="detail-grid mt-4">
                    ${Comp.detailRow('ID', inv.id)}
                    ${Comp.detailRow('Amount', '$' + inv.amount.toFixed(2))}
                    ${Comp.detailRow('Status', utils.statusBadge(inv.status))}
                </div>`;
            } catch (err) { document.getElementById('invoice-result').innerHTML = `<p class="text-vs-error text-sm">${err.message}</p>`; }
        });

        // Notification send
        document.getElementById('admin-notify-form')?.addEventListener('submit', async (e) => {
            e.preventDefault();
            const email = document.getElementById('an-email').value.trim();
            const title = document.getElementById('an-title').value.trim();
            if (!email || !title) { toast.warning('Email and title are required'); return; }
            try {
                await api.post('/admin/notifications', { userId: email, title, body: document.getElementById('an-body').value, notificationType: document.getElementById('an-type').value });
                toast.success('Notification sent');
                e.target.reset();
            } catch (err) { toast.error(err.message); }
        });

        // Deletion detail view
        document.getElementById('admin-misc-content')?.addEventListener('click', e => {
            const btn = e.target.closest('button');
            if (btn?.classList.contains('view-dr-btn')) {
                showDeletionDetail(btn.dataset.id);
            }
        });
    }

    async function showDeletionDetail(id) {
        modal.open('Deletion Request #' + id, Comp.pageLoader());
        try {
            const r = await api.get(`/admin/data-deletion-requests/${id}`);
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
                        <select id="dr-status-select" class="input-field" style="width:auto">
                            ${['Pending', 'Processing', 'Completed', 'Rejected'].map(s => `<option value="${s}" ${r.status === s ? 'selected' : ''}>${s}</option>`).join('')}
                        </select>
                        <button class="btn btn-primary" id="dr-update">Update</button>`
                }
            );
            document.getElementById('dr-update')?.addEventListener('click', async () => {
                await api.put(`/admin/data-deletion-requests/${id}`, { status: document.getElementById('dr-status-select').value });
                toast.success('Request updated');
                modal.close();
                loadDeletions();
            });
        } catch (err) { toast.error('Failed: ' + err.message); modal.close(); }
    }

    return {
        render() { return '<div id="admin-misc-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadDeletions(); }
    };
})();