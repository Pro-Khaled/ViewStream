pages.subscriptions = (() => {
    let state = {
        sub: null,
        methods: [],
        invoices: null,
        loading: true
    };

    async function load() {
        state.loading = true;
        render();
        try {
            const [sub, methods, invoices] = await Promise.all([
                api.get('/subscriptions/current').catch(() => null),
                api.get('/paymentmethods'),
                api.get('/invoices', { pageSize: 5 })
            ]);
            state.sub = sub;
            state.methods = methods;
            state.invoices = invoices;
        } catch (err) {
            toast.error('Failed to load subscription data: ' + err.message);
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('subscriptions-content');
        if (!c) return;

        let h = Comp.pageHeader('Subscription & Payment', 'Manage your plan, payment methods and billing');

        if (state.loading) {
            h += Comp.pageLoader();
        } else {
            // Current subscription
            if (state.sub) {
                h += `<div class="mb-8 p-5 rounded-xl bg-vs-accent/5 border border-vs-accent/20 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                    <div>
                        <div class="flex items-center gap-2 mb-1">
                            <span class="text-sm font-bold text-vs-accent">${toast.esc(state.sub.planType)} Plan</span>
                            ${utils.statusBadge(state.sub.status)}
                        </div>
                        <p class="text-sm text-vs-dim">Started ${utils.formatDateShort(state.sub.startDate)} ${state.sub.autoRenew ? '&middot; Auto-renews' : '&middot; Manual renewal'}</p>
                    </div>
                    <div class="flex gap-2">
                        <button class="btn btn-sm btn-secondary" id="cancel-auto-renew-btn">${state.sub.autoRenew ? 'Cancel Auto-Renew' : 'Enable Auto-Renew'}</button>
                        <button class="btn btn-sm btn-danger" id="cancel-sub-btn">Cancel Subscription</button>
                    </div>
                </div>`;
            }

            // Plan selection
            const plans = [
                { type: 'Basic', price: 8.99, features: ['720p Streaming', '1 Screen', 'Limited Library', 'Ads-Free'] },
                { type: 'Premium', price: 15.99, features: ['1080p Streaming', '4 Screens', 'Full Library', 'Offline Downloads', 'Early Access'], popular: true },
                { type: 'Ultra', price: 22.99, features: ['4K HDR Streaming', '6 Screens', 'Full Library', 'Offline Downloads', 'Early Access', 'Dolby Atmos', 'Exclusive Content'] }
            ];
            h += `<h2 class="font-display font-semibold text-xl text-vs-text mb-4">${state.sub ? 'Change Plan' : 'Select a Plan'}</h2>`;
            h += `<div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">`;
            plans.forEach(p => {
                const isCurrent = state.sub && state.sub.planType === p.type;
                h += `<div class="relative rounded-2xl border-2 ${p.popular ? 'border-vs-accent bg-vs-accent/5' : isCurrent ? 'border-vs-success/50 bg-vs-success/5' : 'border-vs-border bg-vs-surface'} p-6 transition-all hover:border-vs-dim">
                    ${p.popular ? '<div class="absolute -top-3 left-1/2 -translate-x-1/2 px-3 py-0.5 bg-vs-accent text-vs-bg text-xs font-bold rounded-full">MOST POPULAR</div>' : ''}
                    ${isCurrent ? '<div class="absolute top-4 right-4"><i class="fas fa-check-circle text-vs-success"></i></div>' : ''}
                    <h3 class="font-display font-bold text-xl text-vs-text mb-1">${toast.esc(p.type)}</h3>
                    <div class="mb-4"><span class="text-3xl font-display font-black text-vs-text">$${p.price}</span><span class="text-vs-muted text-sm">/month</span></div>
                    <ul class="space-y-2 mb-6">${p.features.map(f => `<li class="flex items-center gap-2 text-sm text-vs-dim"><i class="fas fa-check text-vs-accent text-xs"></i>${toast.esc(f)}</li>`).join('')}</ul>
                    <button class="w-full py-3 rounded-xl text-sm font-bold transition-colors ${isCurrent ? 'bg-vs-card text-vs-dim border border-vs-border cursor-default' : 'bg-vs-accent hover:bg-vs-accentHover text-vs-bg plan-select-btn'}" data-plan="${p.type}" ${isCurrent ? 'disabled' : ''}>${isCurrent ? 'Current Plan' : 'Select Plan'}</button>
                </div>`;
            });
            h += `</div>`;

            // Promo code
            h += `<div class="mb-10 p-5 rounded-xl bg-vs-surface border border-vs-border">
                <h2 class="font-display font-semibold text-lg text-vs-text mb-3">Have a Promo Code?</h2>
                <div class="flex gap-3">
                    <input type="text" id="promo-input" placeholder="Enter promo code" class="input-field uppercase" style="flex:1">
                    <button id="promo-apply-btn" class="btn btn-primary">Apply</button>
                </div>
                <div id="promo-result" class="mt-2 text-sm"></div>
            </div>`;

            // Payment methods
            h += `<h2 class="font-display font-semibold text-xl text-vs-text mb-4">Payment Methods</h2>`;
            h += `<div class="space-y-3 mb-10" id="payment-methods-container">`;
            state.methods.forEach(m => {
                h += `<div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-border">
                    <div class="flex items-center gap-4">
                        <div class="w-12 h-8 rounded bg-vs-card flex items-center justify-center"><i class="fab fa-cc-${m.provider.toLowerCase()} text-vs-dim text-lg"></i></div>
                        <div><p class="text-sm font-medium text-vs-text">${toast.esc(m.provider)} &middot; ****${toast.esc(m.lastFour)}</p><p class="text-xs text-vs-muted">Expires ${String(m.expiryMonth).padStart(2, '0')}/${m.expiryYear} ${m.isDefault ? '&middot; <span class="text-vs-accent">Default</span>' : ''}</p></div>
                    </div>
                    <div class="flex items-center gap-2">
                        ${!m.isDefault ? `<button class="text-xs text-vs-muted hover:text-vs-text set-default-btn" data-id="${m.id}">Set Default</button>` : ''}
                        <button class="p-2 text-vs-muted hover:text-vs-error delete-pm-btn" data-id="${m.id}"><i class="fas fa-trash-alt text-xs"></i></button>
                    </div>
                </div>`;
            });
            h += `<button class="w-full p-4 rounded-xl border-2 border-dashed border-vs-border hover:border-vs-dim text-vs-muted text-sm transition-colors" id="add-payment-btn"><i class="fas fa-plus mr-2"></i>Add Payment Method</button>`;
            h += `</div>`;

            // Billing history
            if (state.invoices && state.invoices.items && state.invoices.items.length) {
                h += `<h2 class="font-display font-semibold text-xl text-vs-text mb-4">Billing History</h2>`;
                h += Comp.dataTable(
                    ['Date', 'Plan', 'Amount', 'Status', ''],
                    state.invoices.items.map(inv => `<tr class="table-row border-b border-vs-border">
                        <td class="px-4 py-3 text-vs-dim">${utils.formatDateShort(inv.invoiceDate)}</td>
                        <td class="px-4 py-3 text-vs-text">${toast.esc(inv.subscriptionPlan)}</td>
                        <td class="px-4 py-3 text-vs-text font-medium">$${inv.amount.toFixed(2)}</td>
                        <td class="px-4 py-3">${utils.statusBadge(inv.status)}</td>
                        <td class="px-4 py-3">${inv.invoicePdfUrl ? `<a href="${toast.esc(inv.invoicePdfUrl)}" class="text-xs text-vs-accent hover:underline" target="_blank"><i class="fas fa-download mr-1"></i>PDF</a>` : ''}</td>
                    </tr>`)
                );
            }
        }

        c.innerHTML = h;
    }

    return {
        render() { return '<div id="subscriptions-content">' + Comp.pageLoader() + '</div>'; },
        init() {
            load();

            document.getElementById('subscriptions-content')?.addEventListener('click', e => {
                const btn = e.target.closest('button');
                if (!btn) return;

                // Cancel auto-renew
                if (btn.id === 'cancel-auto-renew-btn') {
                    const newFlag = !state.sub.autoRenew;
                    api.put(`/subscriptions/${state.sub.id}`, { autoRenew: newFlag }).then(() => {
                        state.sub.autoRenew = newFlag;
                        toast.success(newFlag ? 'Auto-renew enabled' : 'Auto-renew disabled');
                        render();
                    }).catch(err => toast.error(err.message));
                }

                // Cancel subscription
                if (btn.id === 'cancel-sub-btn') {
                    api.post(`/subscriptions/${state.sub.id}/cancel`).then(() => {
                        state.sub = null;
                        toast.success('Subscription cancelled');
                        render();
                    }).catch(err => toast.error(err.message));
                }

                // Plan selection
                if (btn.classList.contains('plan-select-btn')) {
                    const planType = btn.dataset.plan;
                    if (state.sub) {
                        api.put(`/subscriptions/${state.sub.id}`, { planType }).then(() => {
                            toast.success('Plan changed to ' + planType);
                            load();
                        }).catch(err => toast.error(err.message));
                    } else {
                        api.post('/subscriptions', { planType }).then(() => {
                            toast.success('Subscribed to ' + planType);
                            load();
                        }).catch(err => toast.error(err.message));
                    }
                }

                // Promo apply
                if (btn.id === 'promo-apply-btn') {
                    const code = document.getElementById('promo-input').value.trim();
                    if (!code) { toast.warning('Enter a promo code'); return; }
                    api.post('/promocodes/validate', { code }).then(res => {
                        document.getElementById('promo-result').innerHTML = res.isValid
                            ? `<span class="text-vs-success"><i class="fas fa-check-circle mr-1"></i>${toast.esc(res.message)} -${res.discountAmount.toFixed(2)}$${res.discountAmount ? `(-$${res.discountAmount.toFixed(2)})` : ''}</span>`
                            : `<span class="text-vs-error"><i class="fas fa-times-circle mr-1"></i>${toast.esc(res.message)}</span>`;
                    }).catch(err => {
                        document.getElementById('promo-result').innerHTML = `<span class="text-vs-error">${toast.esc(err.message)}</span>`;
                    });
                }

                // Set default payment method
                if (btn.classList.contains('set-default-btn')) {
                    api.post(`/paymentmethods/${btn.dataset.id}/set-default`).then(() => {
                        toast.success('Default payment method updated');
                        load();
                    }).catch(err => toast.error(err.message));
                }

                // Delete payment method
                if (btn.classList.contains('delete-pm-btn')) {
                    api.del(`/paymentmethods/${btn.dataset.id}`).then(() => {
                        toast.success('Payment method removed');
                        load();
                    }).catch(err => toast.error(err.message));
                }

                // Add payment method
                if (btn.id === 'add-payment-btn') {
                    // For simplicity, show a prompt; a real form would be better.
                    const provider = prompt('Enter card provider (Visa, Mastercard, etc.)');
                    if (!provider) return;
                    const lastFour = prompt('Last four digits');
                    if (!lastFour) return;
                    api.post('/paymentmethods', { provider, lastFour, providerToken: 'dummy', expiryMonth: 12, expiryYear: new Date().getFullYear() + 2 })
                        .then(() => { toast.success('Payment method added'); load(); })
                        .catch(err => toast.error(err.message));
                }
            });
        }
    };
})();