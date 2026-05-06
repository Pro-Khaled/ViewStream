pages.profile = {
    render() { return '<div id="profile-content">' + Comp.pageLoader() + '</div>'; },
    async init() {
        const c = document.getElementById('profile-content');
        try {
            const user = await api.get('/account/me');
            const roles = (user.roles || []).map(r => utils.roleBadge(r)).join(' ');
            let h = Comp.pageHeader('My Profile', 'View and edit your account information');
            h += `<div class="bg-vs-surface border border-vs-border rounded-2xl overflow-hidden mb-6">
                <div class="px-6 py-4 border-b border-vs-border">
                    <h2 class="font-display font-semibold text-lg text-vs-text">Account Information</h2>
                </div>
                <form id="profile-form" class="p-6 space-y-4">
                    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div>
                            <label class="block text-sm font-medium text-vs-text mb-1.5">Email</label>
                            <input type="email" value="${toast.esc(user.email)}" disabled class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-muted cursor-not-allowed">
                        </div>
                        <div>
                            <label class="block text-sm font-medium text-vs-text mb-1.5">Full Name</label>
                            <input type="text" id="prof-name" value="${toast.esc(user.fullName || '')}" class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50">
                        </div>
                        <div>
                            <label class="block text-sm font-medium text-vs-text mb-1.5">Phone</label>
                            <input type="tel" id="prof-phone" value="${toast.esc(user.phone || '')}" class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50">
                        </div>
                        <div>
                            <label class="block text-sm font-medium text-vs-text mb-1.5">Country</label>
                            <input type="text" id="prof-country" value="${toast.esc(user.countryCode || '')}" class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50">
                        </div>
                    </div>
                    <div class="flex justify-end">
                        <button type="submit" class="px-6 py-2.5 bg-vs-accent hover:bg-vs-accentHover text-vs-bg font-semibold rounded-lg text-sm transition-colors">Save Changes</button>
                    </div>
                </form>
            </div>

            <!-- Change password -->
            <div class="bg-vs-surface border border-vs-border rounded-2xl overflow-hidden mb-6">
                <div class="px-6 py-4 border-b border-vs-border">
                    <h2 class="font-display font-semibold text-lg text-vs-text">Change Password</h2>
                </div>
                <form id="password-form" class="p-6 space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Current Password</label>
                        <input type="password" id="curr-pass" required class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">New Password</label>
                        <input type="password" id="new-pass" required minlength="8" class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Confirm New Password</label>
                        <input type="password" id="confirm-pass" required class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text focus:outline-none focus:border-vs-accent/50">
                    </div>
                    <div class="flex justify-end">
                        <button type="submit" class="px-6 py-2.5 bg-vs-card hover:bg-vs-elevated text-vs-text font-semibold rounded-lg text-sm border border-vs-border transition-colors">Update Password</button>
                    </div>
                </form>
            </div>

            <div class="bg-vs-surface border border-vs-border rounded-2xl overflow-hidden mb-6">
                <div class="px-6 py-4 border-b border-vs-border">
                    <h2 class="font-display font-semibold text-lg text-vs-text">Roles</h2>
                </div>
                <div class="p-6">
                    <div class="flex flex-wrap gap-2">${roles || '<span class="text-vs-muted text-sm">No roles assigned</span>'}</div>
                </div>
            </div>`;
            c.innerHTML = h;

            // Save profile
            document.getElementById('profile-form')?.addEventListener('submit', async (e) => {
                e.preventDefault();
                const data = {
                    fullName: document.getElementById('prof-name').value.trim(),
                    phoneNumber: document.getElementById('prof-phone').value.trim() || undefined,
                    countryCode: document.getElementById('prof-country').value.trim() || undefined
                };
                try {
                    await api.put('/users/me', data);
                    toast.success('Profile updated');
                } catch (err) { toast.error(err.message); }
            });

            // Change password
            document.getElementById('password-form')?.addEventListener('submit', async (e) => {
                e.preventDefault();
                const curr = document.getElementById('curr-pass').value;
                const nw = document.getElementById('new-pass').value;
                const confirm = document.getElementById('confirm-pass').value;
                if (nw !== confirm) { toast.error('Passwords do not match'); return; }
                if (nw.length < 8) { toast.error('Password must be at least 8 characters'); return; }
                try {
                    await api.post('/users/change-password', { currentPassword: curr, newPassword: nw, confirmNewPassword: confirm });
                    toast.success('Password changed successfully');
                    document.getElementById('password-form').reset();
                } catch (err) { toast.error(err.message); }
            });
        } catch (err) {
            c.innerHTML = Comp.emptyState('fa-exclamation-circle', 'Failed to load profile: ' + err.message);
        }
    }
};