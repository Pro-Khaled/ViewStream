pages.sessions = (() => {
    let state = { data: null, loading: true };

    async function load() {
        state.loading = true; render();
        try {
            state.data = await api.get('/users/me/sessions');
        } catch (err) {
            toast.error('Failed to load login sessions: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    async function revokeSession(id) {
        if (!await modal.confirm('Revoke Session', 'Are you sure you want to end this login session? You will be logged out on that device.', 'danger')) return;
        try {
            await api.post(`/users/me/sessions/${id}/revoke`);
            toast.success('Session revoked successfully');
            load();
        } catch (err) {
            toast.error('Failed to revoke session: ' + err.message);
        }
    }

    async function revokeAllSessions() {
        if (!await modal.confirm('Sign Out of All Devices', 'This will log you out from all other active devices. Proceed?', 'danger')) return;
        try {
            await api.post('/users/me/sessions/revoke-all');
            toast.success('All other sessions revoked. Please log in again if required.');
            // If the user's current session is revoked as part of revoke-all, we should redirect to login.
            // But usually API leaves the current one or we re-authenticate.
            load();
        } catch (err) {
            toast.error('Failed to revoke all sessions: ' + err.message);
        }
    }

    function render() {
        const c = document.getElementById('sessions-content');
        if (!c) return;

        let h = Comp.pageHeader('Active Sessions', 'Monitor and manage the active login sessions on your ViewStream account. You can log out of individual devices or log out from all devices.',
            `<button class="btn btn-secondary btn-sm text-vs-danger hover:bg-vs-danger/10 border-vs-danger/20" id="revoke-all-btn"><i class="fas fa-sign-out-alt mr-1.5"></i> Sign Out All Other Devices</button>`);

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data || state.data.length === 0) {
            h += Comp.emptyState('fa-shield-alt', 'No active login sessions found.');
        } else {
            h += `<div class="card p-6 font-body shadow-lg">
                <div class="space-y-4">
                    ${state.data.map(s => {
                        const createdStr = s.createdAt ? utils.formatDate(s.createdAt) : 'Unknown';
                        const expiresStr = s.expiresAt ? utils.formatDate(s.expiresAt) : 'Unknown';
                        return `
                            <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 p-4 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors group">
                                <div class="flex items-start gap-4">
                                    <div class="w-10 h-10 rounded-lg bg-vs-surface-2 flex items-center justify-center flex-shrink-0 mt-1">
                                        <i class="fas fa-desktop text-vs-accent"></i>
                                    </div>
                                    <div>
                                        <div class="flex items-center gap-2 flex-wrap">
                                            <h4 class="font-bold text-vs-text text-base">${toast.esc(s.deviceName || 'Web Session / Unknown Device')}</h4>
                                            ${s.isActive ? '<span class="badge badge-success text-xs font-semibold py-0.5">Current Session</span>' : ''}
                                        </div>
                                        <p class="text-xs text-vs-muted mt-1"><i class="fas fa-wifi mr-1"></i> IP Address: <span class="font-mono">${toast.esc(s.ipAddress || 'Unknown')}</span></p>
                                        <p class="text-xs text-vs-muted mt-0.5"><i class="far fa-clock mr-1"></i> Logged in: ${createdStr} &middot; Expires: ${expiresStr}</p>
                                    </div>
                                </div>
                                <div class="flex items-center gap-2 self-end sm:self-center">
                                    <button class="btn btn-ghost btn-xs text-vs-danger revoke-sess-btn" data-id="${s.id}"><i class="fas fa-ban mr-1"></i> Revoke</button>
                                </div>
                            </div>
                        `;
                    }).join('')}
                </div>
            </div>`;
        }

        c.innerHTML = h;

        document.getElementById('revoke-all-btn')?.addEventListener('click', revokeAllSessions);
        document.querySelectorAll('.revoke-sess-btn').forEach(btn => btn.addEventListener('click', () => {
            revokeSession(parseInt(btn.dataset.id));
        }));
    }

    return {
        render() { return '<div id="sessions-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();
