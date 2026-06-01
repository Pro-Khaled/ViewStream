pages.userPromoUsages = (() => {
    let state = { data: null, loading: true, redeeming: false };

    async function load() {
        state.loading = true; render();
        try {
            state.data = await api.get('/users/me/promo-usages');
        } catch (err) {
            toast.error('Failed to load your promo code history: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    async function redeemCode(e) {
        e.preventDefault();
        const codeInput = document.getElementById('promo-code-input');
        const code = codeInput.value.trim();
        const planType = document.getElementById('promo-plan-type').value || undefined;

        if (!code) {
            toast.warn('Please enter a promo code');
            return;
        }

        state.redeeming = true; render();
        try {
            const res = await api.post('/users/me/promo-usages/redeem', { code, planType });
            toast.success(`Promo code "${res.promoCodeCode || code}" redeemed successfully!`);
            codeInput.value = '';
            load();
        } catch (err) {
            toast.error('Failed to redeem promo code: ' + err.message);
            state.redeeming = false; render();
        }
    }

    function render() {
        const c = document.getElementById('promo-usages-content');
        if (!c) return;

        let h = Comp.pageHeader('Promo Codes & Rewards', 'Redeem promotional offers, discounts, or trial codes and check your active savings.');

        if (state.loading) {
            h += Comp.pageLoader();
        } else {
            h += `<div class="grid grid-cols-1 lg:grid-cols-3 gap-6 font-body">
                <!-- Redeem Card -->
                <div class="lg:col-span-1 space-y-6">
                    <div class="card p-6 shadow-md border-vs-border">
                        <h3 class="font-bold text-lg text-vs-text mb-4"><i class="fas fa-ticket-alt text-vs-accent mr-1.5"></i> Redeem a Code</h3>
                        <form id="redeem-promo-form" class="space-y-4">
                            <div class="form-group">
                                <label class="form-label" for="promo-code-input">Promo Code</label>
                                <input class="input-field font-mono uppercase" id="promo-code-input" placeholder="e.g. SUMMER50, BINGEFREE" required>
                            </div>
                            <div class="form-group">
                                <label class="form-label" for="promo-plan-type">Target Subscription Plan (Optional)</label>
                                <select class="input-field" id="promo-plan-type">
                                    <option value="">Choose Plan (If applicable)</option>
                                    <option value="Basic">Basic</option>
                                    <option value="Standard">Standard</option>
                                    <option value="Premium">Premium</option>
                                </select>
                            </div>
                            <button type="submit" class="btn btn-primary w-full" ${state.redeeming ? 'disabled' : ''}>
                                ${state.redeeming ? '<i class="fas fa-spinner fa-spin mr-1"></i> Redeeming...' : 'Apply Offer'}
                            </button>
                        </form>
                    </div>
                </div>

                <!-- History Card -->
                <div class="lg:col-span-2 space-y-6">
                    <div class="card p-6 shadow-md border-vs-border">
                        <h3 class="font-bold text-lg text-vs-text mb-4"><i class="fas fa-history text-vs-accent mr-1.5"></i> Promo Code History</h3>
                        `;

            if (!state.data || state.data.length === 0) {
                h += Comp.emptyState('fa-percentage', 'You haven\'t redeemed any promo codes yet.');
            } else {
                h += `<div class="space-y-3">
                    ${state.data.map(u => `
                        <div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors">
                            <div class="flex items-center gap-3">
                                <div class="w-10 h-10 rounded-lg bg-vs-accent/10 flex items-center justify-center text-vs-accent flex-shrink-0">
                                    <i class="fas fa-gift text-lg"></i>
                                </div>
                                <div>
                                    <h4 class="font-bold text-vs-text text-sm font-mono uppercase">${toast.esc(u.promoCodeCode)}</h4>
                                    <p class="text-xs text-vs-muted mt-0.5">Applied on ${u.usedAt ? utils.formatDate(u.usedAt) : 'Unknown'}</p>
                                </div>
                            </div>
                            <span class="badge badge-success text-xs font-semibold py-1 px-2.5">Active / Redeemed</span>
                        </div>
                    `).join('')}
                </div>`;
            }

            h += `</div>
                </div>
            </div>`;
        }

        c.innerHTML = h;

        document.getElementById('redeem-promo-form')?.addEventListener('submit', redeemCode);
    }

    return {
        render() { return '<div id="promo-usages-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();
