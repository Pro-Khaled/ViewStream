pages.adminReports = (() => {
    let activeTab = 'comments'; // 'comments' or 'content'
    let commentState = { page: 1, status: '', data: null, loading: true };
    let contentState = { page: 1, status: '', targetType: '', data: null, loading: true };

    async function loadComments() {
        commentState.loading = true; render();
        try { commentState.data = await api.get('/admin/reports/comments', { page: commentState.page, pageSize: CONFIG.PAGE_SIZE, status: commentState.status || undefined }); }
        catch { toast.error('Failed to load comment reports'); }
        commentState.loading = false; render();
    }
    async function loadContent() {
        contentState.loading = true; render();
        try { contentState.data = await api.get('/admin/reports/content', { page: contentState.page, pageSize: CONFIG.PAGE_SIZE, status: contentState.status || undefined, targetType: contentState.targetType || undefined }); }
        catch { toast.error('Failed to load content reports'); }
        contentState.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-reports-content');
        if (!c) return;
        let h = `<div class="flex gap-1 mb-6 bg-vs-card rounded-lg p-1 w-fit">
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'comments' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="comments">Comment Reports</button>
            <button class="tab-btn px-4 py-2 rounded-md text-sm font-medium ${activeTab === 'content' ? 'bg-vs-accent text-vs-bg' : 'text-vs-dim hover:bg-vs-elevated'}" data-tab="content">Content Reports</button>
        </div>`;

        if (activeTab === 'comments') h += renderCommentReports();
        else h += renderContentReports();

        c.innerHTML = h;
        bindEvents();
    }

    function renderCommentReports() {
        let h = Comp.pageHeader('Comment Reports', 'Review and manage reported comments');
        h += Comp.filterBar([
            { key: 'status', label: 'Status', type: 'select', options: ['Pending', 'Reviewed', 'Dismissed', 'ActionTaken'] }
        ], vals => { commentState.status = vals.status || ''; commentState.page = 1; loadComments(); });

        if (commentState.loading) { h += Comp.pageLoader(); }
        else if (!commentState.data?.items?.length) { h += Comp.emptyState('fa-flag', 'No comment reports'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Comment</th><th>Reporter</th><th>Reason</th><th>Status</th><th>Date</th><th></th></tr></thead><tbody>` +
                commentState.data.items.map(r => `<tr>
                    <td class="text-muted">${r.id}</td>
                    <td class="max-w-[200px]">${toast.esc(r.commentText)}</td>
                    <td>${toast.esc(r.reportedByProfileName)}</td>
                    <td>${toast.esc(r.reason)}</td>
                    <td>${utils.statusBadge(r.status)}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(r.createdAt)}</td>
                    <td><button class="btn btn-ghost btn-sm view-cr-btn" data-id="${r.id}"><i class="fas fa-eye"></i></button></td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(commentState.data.pageNumber, commentState.data.totalPages, p => { commentState.page = p; loadComments(); });
        }
        return h;
    }

    function renderContentReports() {
        let h = Comp.pageHeader('Content Reports', 'Review reported shows and episodes');
        h += Comp.filterBar([
            { key: 'status', label: 'Status', type: 'select', options: ['Pending', 'Reviewed', 'Dismissed', 'ActionTaken'] },
            { key: 'targetType', label: 'Target Type', type: 'select', options: ['Show', 'Episode'] }
        ], vals => { contentState.status = vals.status || ''; contentState.targetType = vals.targetType || ''; contentState.page = 1; loadContent(); });

        if (contentState.loading) { h += Comp.pageLoader(); }
        else if (!contentState.data?.items?.length) { h += Comp.emptyState('fa-flag', 'No content reports'); }
        else {
            h += `<div class="card table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Type</th><th>Title</th><th>Reporter</th><th>Reason</th><th>Status</th><th>Date</th><th></th></tr></thead><tbody>` +
                contentState.data.items.map(r => `<tr>
                    <td class="text-muted">${r.id}</td>
                    <td><span class="badge badge-info">${toast.esc(r.targetType)}</span></td>
                    <td class="font-medium max-w-[200px]">${toast.esc(r.targetTitle)}</td>
                    <td>${toast.esc(r.profileName)}</td>
                    <td>${toast.esc(r.reason)}</td>
                    <td>${utils.statusBadge(r.status)}</td>
                    <td class="text-muted text-sm">${utils.formatDateShort(r.reportedAt)}</td>
                    <td><button class="btn btn-ghost btn-sm view-ctr-btn" data-id="${r.id}"><i class="fas fa-eye"></i></button></td>
                </tr>`).join('') +
                `</tbody></table></div>`;
            h += Comp.pagination(contentState.data.pageNumber, contentState.data.totalPages, p => { contentState.page = p; loadContent(); });
        }
        return h;
    }

    function bindEvents() {
        document.querySelectorAll('#admin-reports-content .tab-btn').forEach(b => {
            b.addEventListener('click', () => {
                activeTab = b.dataset.tab;
                if (activeTab === 'comments' && !commentState.data) loadComments();
                else if (activeTab === 'content' && !contentState.data) loadContent();
                render();
            });
        });

        // Detail views
        document.getElementById('admin-reports-content')?.addEventListener('click', e => {
            const btn = e.target.closest('button');
            if (!btn) return;
            if (btn.classList.contains('view-cr-btn')) showCommentDetail(btn.dataset.id);
            else if (btn.classList.contains('view-ctr-btn')) showContentDetail(btn.dataset.id);
        });
    }

    async function showCommentDetail(id) {
        modal.open('Comment Report #' + id, Comp.pageLoader());
        try {
            const r = await api.get(`/admin/reports/comments/${id}`);
            modal.open('Comment Report #' + r.id,
                `<div class="detail-grid">
                    ${Comp.detailRow('ID', r.id)}
                    ${Comp.detailRow('Comment', toast.esc(r.commentText))}
                    ${Comp.detailRow('Status', utils.statusBadge(r.status))}
                </div>`,
                {
                    footer: `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                    <select id="cr-status-select" class="input-field" style="width:auto">
                        ${['Pending', 'Reviewed', 'Dismissed', 'ActionTaken'].map(s => `<option value="${s}" ${r.status === s ? 'selected' : ''}>${s}</option>`).join('')}
                    </select>
                    <button class="btn btn-primary" id="cr-update">Update Status</button>` }
            );
            document.getElementById('cr-update')?.addEventListener('click', async () => {
                await api.put(`/admin/reports/comments/${id}/status`, { status: document.getElementById('cr-status-select').value });
                toast.success('Status updated');
                modal.close();
                loadComments();
            });
        } catch (err) { toast.error('Failed: ' + err.message); modal.close(); }
    }

    async function showContentDetail(id) {
        modal.open('Content Report #' + id, Comp.pageLoader());
        try {
            const r = await api.get(`/admin/reports/content/${id}`);
            modal.open('Content Report #' + r.id,
                `<div class="detail-grid">
                    ${Comp.detailRow('ID', r.id)}
                    ${Comp.detailRow('Target', toast.esc(r.targetTitle))}
                    ${Comp.detailRow('Status', utils.statusBadge(r.status))}
                </div>`,
                {
                    footer: `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                    <select id="ctr-status-select" class="input-field" style="width:auto">
                        ${['Pending', 'Reviewed', 'Dismissed', 'ActionTaken'].map(s => `<option value="${s}" ${r.status === s ? 'selected' : ''}>${s}</option>`).join('')}
                    </select>
                    <button class="btn btn-primary" id="ctr-update">Update Status</button>` }
            );
            document.getElementById('ctr-update')?.addEventListener('click', async () => {
                await api.put(`/admin/reports/content/${id}/status`, { status: document.getElementById('ctr-status-select').value });
                toast.success('Status updated');
                modal.close();
                loadContent();
            });
        } catch (err) { toast.error('Failed: ' + err.message); modal.close(); }
    }

    return {
        render() { return '<div id="admin-reports-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadComments(); }
    };
})();