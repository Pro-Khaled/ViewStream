pages.emailPreferences = (() => {
    let state = { data: null, loading: true, saving: false };

    async function load() {
        state.loading = true; render();
        try {
            state.data = await api.get('/users/me/email-preferences');
        } catch (err) {
            if (err.status === 404) {
                // If preferences are not found/set yet, initialize with defaults
                state.data = { marketingEmails: false, newReleaseAlerts: false, recommendationEmails: false, accountUpdates: true };
            } else {
                toast.error('Failed to load email preferences: ' + err.message);
                state.data = null;
            }
        }
        state.loading = false; render();
    }

    async function save(e) {
        e.preventDefault();
        state.saving = true; render();
        try {
            const payload = {
                marketingEmails: document.getElementById('pref-marketing').checked,
                newReleaseAlerts: document.getElementById('pref-releases').checked,
                recommendationEmails: document.getElementById('pref-recs').checked,
                accountUpdates: document.getElementById('pref-accounts').checked
            };
            state.data = await api.put('/users/me/email-preferences', payload);
            toast.success('Email preferences updated successfully');
        } catch (err) {
            toast.error('Failed to update email preferences: ' + err.message);
        }
        state.saving = false; render();
    }

    function render() {
        const c = document.getElementById('email-pref-content');
        if (!c) return;

        let h = Comp.pageHeader('Email Preferences', 'Choose which notification and update emails you wish to receive.');
        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data) {
            h += Comp.emptyState('fa-envelope-open-text', 'Failed to retrieve preference data');
        } else {
            h += `<div class="card p-6 max-w-2xl mx-auto font-body shadow-lg">
                <form id="email-pref-form" class="space-y-6">
                    <div class="space-y-4">
                        <div class="flex items-start gap-4 p-3 rounded-lg hover:bg-vs-surface-2 transition-colors">
                            <div class="flex items-center h-5">
                                <input type="checkbox" id="pref-accounts" class="form-checkbox text-vs-accent" ${state.data.accountUpdates ? 'checked' : ''}>
                            </div>
                            <div class="flex-1">
                                <label for="pref-accounts" class="form-label font-semibold text-vs-text mb-0 cursor-pointer">Account Security & Updates</label>
                                <p class="text-xs text-vs-muted mt-1">Get emails about your account activity, login sessions, billing, and important privacy changes.</p>
                            </div>
                        </div>

                        <div class="flex items-start gap-4 p-3 rounded-lg hover:bg-vs-surface-2 transition-colors">
                            <div class="flex items-center h-5">
                                <input type="checkbox" id="pref-releases" class="form-checkbox text-vs-accent" ${state.data.newReleaseAlerts ? 'checked' : ''}>
                            </div>
                            <div class="flex-1">
                                <label for="pref-releases" class="form-label font-semibold text-vs-text mb-0 cursor-pointer">New Release Alerts</label>
                                <p class="text-xs text-vs-muted mt-1">Be notified when new movies, series, or seasons are added to ViewStream.</p>
                            </div>
                        </div>

                        <div class="flex items-start gap-4 p-3 rounded-lg hover:bg-vs-surface-2 transition-colors">
                            <div class="flex items-center h-5">
                                <input type="checkbox" id="pref-recs" class="form-checkbox text-vs-accent" ${state.data.recommendationEmails ? 'checked' : ''}>
                            </div>
                            <div class="flex-1">
                                <label for="pref-recs" class="form-label font-semibold text-vs-text mb-0 cursor-pointer">Recommendations</label>
                                <p class="text-xs text-vs-muted mt-1">Receive personalized viewing recommendations based on your watch history and liked items.</p>
                            </div>
                        </div>

                        <div class="flex items-start gap-4 p-3 rounded-lg hover:bg-vs-surface-2 transition-colors">
                            <div class="flex items-center h-5">
                                <input type="checkbox" id="pref-marketing" class="form-checkbox text-vs-accent" ${state.data.marketingEmails ? 'checked' : ''}>
                            </div>
                            <div class="flex-1">
                                <label for="pref-marketing" class="form-label font-semibold text-vs-text mb-0 cursor-pointer">Offers & Promotions</label>
                                <p class="text-xs text-vs-muted mt-1">Stay informed about special promotions, billing discounts, and partner offers.</p>
                            </div>
                        </div>
                    </div>

                    <div class="flex items-center justify-end border-t border-vs-border pt-4">
                        <button type="submit" class="btn btn-primary" ${state.saving ? 'disabled' : ''}>
                            ${state.saving ? '<i class="fas fa-spinner fa-spin mr-1.5"></i> Saving...' : '<i class="fas fa-save mr-1.5"></i> Save Preferences'}
                        </button>
                    </div>
                </form>
            </div>`;
        }
        c.innerHTML = h;

        document.getElementById('email-pref-form')?.addEventListener('submit', save);
    }

    return {
        render() { return '<div id="email-pref-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();
