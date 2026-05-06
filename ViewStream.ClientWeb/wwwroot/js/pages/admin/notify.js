pages.adminNotify = {
    render() {
        return `<div class="max-w-lg bg-vs-surface border border-vs-border rounded-xl p-6">
            <h1 class="font-display font-bold text-2xl text-vs-text mb-6">Send Notification</h1>
            <form id="admin-notify-form" class="space-y-4" novalidate>
                <div><label class="block text-sm font-medium text-vs-text mb-1.5">User Email</label><input type="email" id="an-email" required class="input-field" placeholder="user@example.com"></div>
                <div><label class="block text-sm font-medium text-vs-text mb-1.5">Title</label><input type="text" id="an-title" required class="input-field" placeholder="Notification title"></div>
                <div><label class="block text-sm font-medium text-vs-text mb-1.5">Body</label><textarea id="an-body" rows="4" class="input-field" placeholder="Notification body..."></textarea></div>
                <div><label class="block text-sm font-medium text-vs-text mb-1.5">Type</label><select id="an-type" class="input-field"><option value="general">General</option><option value="new_episode">New Episode</option><option value="subscription">Subscription</option><option value="recommendation">Recommendation</option></select></div>
                <button type="submit" class="btn btn-primary w-full">Send Notification</button>
            </form>
        </div>`;
    },
    init() {
        document.getElementById('admin-notify-form')?.addEventListener('submit', async (e) => {
            e.preventDefault();
            const email = document.getElementById('an-email').value.trim();
            const title = document.getElementById('an-title').value.trim();
            if (!email || !title) { toast.warning('Email and title are required'); return; }
            const body = document.getElementById('an-body').value;
            const type = document.getElementById('an-type').value;
            try {
                await api.post('/admin/notifications', { userId: email, title, body, notificationType: type });
                toast.success('Notification sent');
                e.target.reset();
            } catch (err) { toast.error(err.message); }
        });
    }
};