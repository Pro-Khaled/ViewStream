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

            <!-- Downloads link -->
            <div class="bg-vs-surface border border-vs-border rounded-2xl overflow-hidden mb-6">
                <div class="px-6 py-4 border-b border-vs-border">
                    <h2 class="font-display font-semibold text-lg text-vs-text">Roles & Quick Links</h2>
                </div>
                <div class="p-6 space-y-3">
                    <div class="flex flex-wrap gap-2 mb-4">${roles || '<span class="text-vs-muted text-sm">No roles assigned</span>'}</div>
                    <div class="flex flex-wrap gap-3">
                        <a href="#/downloads" class="btn btn-secondary btn-sm"><i class="fas fa-download mr-1.5"></i> Offline Downloads</a>
                        <a href="#/library" class="btn btn-secondary btn-sm"><i class="fas fa-bookmark mr-1.5"></i> My Library</a>
                        <a href="#/history" class="btn btn-secondary btn-sm"><i class="fas fa-history mr-1.5"></i> Watch History</a>
                    </div>
                </div>
            </div>

            <!-- Danger zone: data deletion -->
            <div class="bg-vs-surface border border-vs-error/30 rounded-2xl overflow-hidden mb-6" id="danger-zone">
                <div class="px-6 py-4 border-b border-vs-error/20">
                    <h2 class="font-display font-semibold text-lg text-vs-error"><i class="fas fa-shield-alt mr-2"></i>Privacy & Data</h2>
                </div>
                <div class="p-6">
                    <p class="text-sm text-vs-dim mb-4">You can request deletion of all your personal data. Once submitted, our team will process your request within 30 days per GDPR/CCPA requirements.</p>
                    <div id="deletion-status" class="hidden mb-4 p-4 rounded-xl border bg-vs-card text-sm"></div>
                    <div class="flex gap-3 flex-wrap">
                        <button id="request-deletion-btn" class="btn btn-danger btn-sm"><i class="fas fa-trash-alt mr-1.5"></i> Request Data Deletion</button>
                        <button id="cancel-deletion-btn" class="btn btn-secondary btn-sm hidden"><i class="fas fa-undo mr-1.5"></i> Cancel Request</button>
                    </div>
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

            // Data deletion
            let activeDeletionId = null;
            const statusEl = document.getElementById('deletion-status');
            const reqBtn = document.getElementById('request-deletion-btn');
            const cancelBtn = document.getElementById('cancel-deletion-btn');

            function showDeletionPending(req) {
                activeDeletionId = req.id;
                statusEl.className = 'mb-4 p-4 rounded-xl border bg-vs-error/5 border-vs-error/30 text-sm text-vs-dim';
                statusEl.innerHTML = `<i class="fas fa-clock text-vs-error mr-2"></i>Deletion request submitted on <strong>${utils.formatDate(req.requestedAt)}</strong>. Status: <span class="text-vs-error font-semibold uppercase">${req.status}</span>`;
                statusEl.classList.remove('hidden');
                reqBtn.classList.add('hidden');
                cancelBtn.classList.remove('hidden');
            }

            reqBtn?.addEventListener('click', () => {
                modal.open('Confirm Data Deletion Request',
                    `<div class="space-y-3">
                        <p class="text-sm text-vs-dim">Are you sure you want to request deletion of all your personal data? This includes your account, watch history, library, and all associated content.</p>
                        <p class="text-sm text-vs-error font-medium"><i class="fas fa-exclamation-triangle mr-1"></i>This action is irreversible once processed.</p>
                    </div>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-danger" id="confirm-deletion-btn">Submit Request</button>`);
                document.getElementById('confirm-deletion-btn')?.addEventListener('click', async () => {
                    try {
                        const req = await api.post('/users/me/data-deletion-request', {});
                        showDeletionPending(req);
                        modal.close();
                        toast.info('Deletion request submitted. We will process it within 30 days.');
                    } catch (err) { toast.error(err.message); }
                });
            });

            cancelBtn?.addEventListener('click', () => {
                if (!activeDeletionId) return;
                modal.open('Cancel Deletion Request',
                    '<p class="text-sm text-vs-dim">Cancel your data deletion request? Your account will remain active.</p>',
                    `<button class="btn btn-secondary" onclick="modal.close()">Close</button>
                     <button class="btn btn-primary" id="confirm-cancel-del">Yes, Cancel Request</button>`);
                document.getElementById('confirm-cancel-del')?.addEventListener('click', async () => {
                    try {
                        await api.del(`/users/me/data-deletion-request/${activeDeletionId}`);
                        activeDeletionId = null;
                        statusEl.classList.add('hidden');
                        reqBtn.classList.remove('hidden');
                        cancelBtn.classList.add('hidden');
                        modal.close();
                        toast.success('Deletion request cancelled.');
                    } catch (err) { toast.error(err.message); }
                });
            });

        } catch (err) {
            c.innerHTML = Comp.emptyState('fa-exclamation-circle', 'Failed to load profile: ' + err.message);
        }
    }
};