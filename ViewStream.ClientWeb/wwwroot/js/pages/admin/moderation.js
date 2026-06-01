pages.adminModeration = (() => {
    let state = { commentReports: [], contentReports: [], loading: true };
    async function loadData() {
        state.loading = true; render();
        try {
            const [cr, ctr] = await Promise.all([api.get('/admin/reports/comments'), api.get('/admin/reports/content')]);
            state.commentReports = cr.items;
            state.contentReports = ctr.items;
        } catch (err) { toast.error('Failed to load reports'); }
        state.loading = false; render();
    }
    function render() {
        const c = document.getElementById('admin-moderation-content');
        if (!c) return;
        let h = Comp.pageHeader('Moderation');
        if (state.loading) { h += Comp.pageLoader(); }
        else {
            h += `<div class="flex gap-1 mb-6 bg-vs-card rounded-lg p-1 w-fit">
                <button id="tab-comments-btn" class="px-4 py-2 rounded-md text-sm font-medium bg-vs-accent text-vs-bg">Comment Reports (${state.commentReports.length})</button>
                <button id="tab-content-btn" class="px-4 py-2 rounded-md text-sm font-medium text-vs-dim hover:bg-vs-elevated">Content Reports (${state.contentReports.length})</button>
            </div>
            <div id="tab-comments">` + buildCommentTable(state.commentReports) + `</div>
            <div id="tab-content" class="hidden">` + buildContentTable(state.contentReports) + `</div>`;
        }
        c.innerHTML = h;
        bindEvents();
    }
    function buildCommentTable(items) {
        return `<div class="card table-container"><table class="data-table"><thead><tr>
            <th>ID</th><th>Comment</th><th>Reporter</th><th>Reason</th><th>Status</th><th>Date</th><th></th></tr></thead><tbody>` +
            items.map(r => `<tr>
                <td class="text-muted">${r.id}</td>
                <td class="max-w-[200px]">${toast.esc(r.commentText)}</td>
                <td>${toast.esc(r.reportedByProfileName)}</td>
                <td>${toast.esc(r.reason)}</td>
                <td>${utils.statusBadge(r.status)}</td>
                <td class="text-muted text-sm">${utils.formatDateShort(r.createdAt)}</td>
                <td><button class="btn btn-ghost btn-sm view-cr-btn" data-id="${r.id}"><i class="fas fa-eye"></i></button></td>
            </tr>`).join('') +
            `</tbody></table></div>`;
    }
    function buildContentTable(items) {
        return `<div class="card table-container"><table class="data-table"><thead><tr>
            <th>ID</th><th>Target Type</th><th>Title</th><th>Reporter</th><th>Reason</th><th>Status</th><th>Date</th><th></th></tr></thead><tbody>` +
            items.map(r => `<tr>
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
    }
    function bindEvents() {
        document.getElementById('tab-comments-btn')?.addEventListener('click', () => {
            document.getElementById('tab-comments').classList.remove('hidden');
            document.getElementById('tab-content').classList.add('hidden');
            document.getElementById('tab-comments-btn').classList.add('bg-vs-accent', 'text-vs-bg');
            document.getElementById('tab-comments-btn').classList.remove('text-vs-dim', 'hover:bg-vs-elevated');
            document.getElementById('tab-content-btn').classList.remove('bg-vs-accent', 'text-vs-bg');
            document.getElementById('tab-content-btn').classList.add('text-vs-dim', 'hover:bg-vs-elevated');
        });
        document.getElementById('tab-content-btn')?.addEventListener('click', () => {
            document.getElementById('tab-content').classList.remove('hidden');
            document.getElementById('tab-comments').classList.add('hidden');
            // swap classes similarly
        });
    }
    return {
        render() { return '<div id="admin-moderation-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();