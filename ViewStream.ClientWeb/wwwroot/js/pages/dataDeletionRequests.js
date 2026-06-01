pages.dataDeletionRequests = (() => {
    let state = { activeRequest: null, loading: true, checked: false };

    async function load() {
        state.loading = true; render();
        try {
            // Under GDPR/CCPA, we fetch or initialize the request status by sending a POST.
            // If one is already pending/processing, the API returns it; otherwise, we can retrieve/submit.
            // However, to avoid prematurely creating a request on page load, we should explain the policy
            // and let the user click "Check status or request deletion" or we can safely try to get/check.
            // Actually, we can check by doing the POST since the user chose to visit this page.
            state.activeRequest = await api.post('/users/me/data-deletion-request', {});
        } catch (err) {
            toast.error('Failed to retrieve deletion request status: ' + err.message);
            state.activeRequest = null;
        }
        state.loading = false; render();
    }

    async function submitRequest() {
        if (!state.checked) {
            toast.warn('You must acknowledge that this action is permanent and irreversible.');
            return;
        }
        if (!await modal.confirm('Confirm Permanent Deletion', 'Are you absolutely sure you want to permanently delete all your data? Your account, profiles, watch history, subscriptions, and billing logs will be deleted. This cannot be undone.', 'danger')) return;

        state.loading = true; render();
        try {
            state.activeRequest = await api.post('/users/me/data-deletion-request', {});
            toast.info('Data deletion request submitted successfully. It will be processed within 30 days.');
        } catch (err) {
            toast.error('Failed to submit request: ' + err.message);
        }
        state.loading = false; render();
    }

    async function cancelRequest(id) {
        if (!await modal.confirm('Cancel Deletion Request', 'Are you sure you want to cancel this data deletion request? Your account and data will remain fully active.', 'primary')) return;

        state.loading = true; render();
        try {
            await api.del(`/users/me/data-deletion-request/${id}`);
            toast.success('Your deletion request has been cancelled.');
            state.activeRequest = null;
        } catch (err) {
            toast.error('Failed to cancel deletion request: ' + err.message);
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('deletion-content');
        if (!c) return;

        let h = Comp.pageHeader('Data Deletion & Privacy', 'Manage your right to be forgotten under GDPR, CCPA, and global privacy frameworks.');

        if (state.loading) {
            h += Comp.pageLoader();
        } else {
            h += `<div class="max-w-3xl mx-auto font-body space-y-6">
                <!-- Info Policy Card -->
                <div class="card p-6 shadow-md border-vs-border">
                    <h3 class="font-bold text-lg text-vs-text mb-3 flex items-center gap-2">
                        <i class="fas fa-user-shield text-vs-accent"></i> Privacy Policy & Your Rights
                    </h3>
                    <p class="text-sm text-vs-dim leading-relaxed mb-4">
                        At ViewStream, we respect your privacy rights. You have the right to request the permanent deletion of your personal data. 
                        Once you request deletion:
                    </p>
                    <ul class="list-disc pl-5 text-xs text-vs-muted space-y-2 mb-4">
                        <li>Your account profile, including full name, phone number, and billing history, will be removed.</li>
                        <li>All created user profiles, watch histories, and playback telemetry will be permanently wiped.</li>
                        <li>Any active subscriptions will be immediately terminated without refund.</li>
                        <li>Your library, shared lists, and saved items will be deleted.</li>
                    </ul>
                    <div class="p-3 bg-vs-surface-2 rounded-lg text-xs text-vs-warning flex items-start gap-2 border border-vs-warning/20">
                        <i class="fas fa-exclamation-triangle mt-0.5"></i>
                        <span><strong>Warning:</strong> Deletion requests cannot be undone. Once processed, our support team cannot restore any of your profile, watch progress, or purchases.</span>
                    </div>
                </div>

                <!-- Request Management / Status Card -->
                <div class="card p-6 shadow-md border-vs-border">
                    <h3 class="font-bold text-lg text-vs-text mb-4">Deletion Status</h3>
                    `;

            if (state.activeRequest && (state.activeRequest.status === 'Pending' || state.activeRequest.status === 'Processing')) {
                const reqDate = state.activeRequest.requestedAt ? utils.formatDate(state.activeRequest.requestedAt) : 'Unknown';
                h += `
                    <div class="p-5 rounded-xl bg-vs-error/5 border border-vs-error/30 text-center space-y-4">
                        <div class="w-12 h-12 rounded-full bg-vs-error/10 flex items-center justify-center mx-auto text-vs-error text-xl animate-pulse">
                            <i class="fas fa-hourglass-half"></i>
                        </div>
                        <div>
                            <h4 class="font-bold text-vs-text text-lg">Data Deletion Request in Progress</h4>
                            <p class="text-xs text-vs-muted mt-1">Submitted on ${reqDate} &middot; Status: <span class="badge badge-warning text-xs font-semibold py-0.5 uppercase">${state.activeRequest.status}</span></p>
                            <p class="text-sm text-vs-dim mt-3 max-w-md mx-auto">Your request is currently being reviewed. Your data will be deleted within 30 days of submission.</p>
                        </div>
                        <div class="pt-2">
                            <button class="btn btn-secondary text-vs-accent border-vs-accent/20 btn-sm cancel-del-btn" data-id="${state.activeRequest.id}">
                                <i class="fas fa-undo mr-1.5"></i> Cancel Deletion Request
                            </button>
                        </div>
                    </div>
                `;
            } else if (state.activeRequest && state.activeRequest.status === 'Completed') {
                h += `
                    <div class="p-5 rounded-xl bg-vs-success/5 border border-vs-success/30 text-center space-y-4">
                        <div class="w-12 h-12 rounded-full bg-vs-success/10 flex items-center justify-center mx-auto text-vs-success text-xl">
                            <i class="fas fa-check-circle"></i>
                        </div>
                        <div>
                            <h4 class="font-bold text-vs-text text-lg">Your Data Was Deleted</h4>
                            <p class="text-xs text-vs-muted mt-1">This request has already been completed or archived.</p>
                        </div>
                    </div>
                `;
            } else {
                h += `
                    <div class="space-y-4">
                        <p class="text-sm text-vs-dim">You currently have no active or pending data deletion requests.</p>
                        <div class="flex items-start gap-3 p-3 rounded-lg hover:bg-vs-surface-2 transition-colors">
                            <div class="flex items-center h-5">
                                <input type="checkbox" id="ack-checkbox" class="form-checkbox text-vs-accent" ${state.checked ? 'checked' : ''}>
                            </div>
                            <div class="flex-1">
                                <label for="ack-checkbox" class="form-label font-semibold text-vs-text mb-0 cursor-pointer text-xs">
                                    I acknowledge that requesting data deletion will terminate my account and destroy all my watch history and platform access permanently.
                                </label>
                            </div>
                        </div>
                        <div class="flex justify-end">
                            <button class="btn btn-danger btn-sm" id="submit-deletion-btn">
                                <i class="fas fa-trash-alt mr-1.5"></i> Request Permanent Account Deletion
                            </button>
                        </div>
                    </div>
                `;
            }

            h += `</div>
            </div>`;
        }

        c.innerHTML = h;

        document.getElementById('ack-checkbox')?.addEventListener('change', (e) => {
            state.checked = e.target.checked;
        });

        document.getElementById('submit-deletion-btn')?.addEventListener('click', submitRequest);
        document.querySelector('.cancel-del-btn')?.addEventListener('click', (e) => {
            const btn = e.currentTarget;
            cancelRequest(parseInt(btn.dataset.id));
        });
    }

    return {
        render() { return '<div id="deletion-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();
