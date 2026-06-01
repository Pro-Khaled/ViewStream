pages.subscriptions = (() => {
    let state = {
        sub: null,
        methods: [],
        invoices: null,
        loading: true
    };

    async function load() {
        state.loading = true; render();
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
        state.loading = false; render();
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
                h += `<div class="mb-8 p-5 rounded-xl bg-vs-accent/5 border border-vs-accent/20 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 font-body">
                    <div>
                        <div class="flex items-center gap-2 mb-1">
                            <span class="text-sm font-bold text-vs-accent">${toast.esc(state.sub.planType)} Plan</span>
                            ${utils.statusBadge(state.sub.status)}
                        </div>
                        <p class="text-sm text-vs-dim font-medium">Started ${utils.formatDateShort(state.sub.startDate)} ${state.sub.autoRenew ? '&middot; Auto-renews' : '&middot; Manual renewal'}</p>
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
            h += `<div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10 font-body">`;
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
            h += `<div class="mb-10 p-5 rounded-xl bg-vs-surface border border-vs-border font-body">
                <h2 class="font-display font-semibold text-lg text-vs-text mb-3">Have a Promo Code?</h2>
                <div class="flex gap-3">
                    <input type="text" id="promo-input" placeholder="Enter promo code" class="input-field uppercase" style="flex:1">
                    <button id="promo-apply-btn" class="btn btn-primary font-semibold">Apply</button>
                </div>
                <div id="promo-result" class="mt-2 text-sm"></div>
            </div>`;

            // Payment methods
            h += `<h2 class="font-display font-semibold text-xl text-vs-text mb-4">Payment Methods</h2>`;
            h += `<div class="space-y-3 mb-10 font-body" id="payment-methods-container">`;
            state.methods.forEach(m => {
                h += `<div class="flex items-center justify-between p-4 rounded-xl bg-vs-surface border border-vs-border">
                    <div class="flex items-center gap-4">
                        <div class="w-12 h-8 rounded bg-vs-card flex items-center justify-center border border-vs-border"><i class="fab fa-cc-${m.provider.toLowerCase()} text-vs-dim text-lg"></i></div>
                        <div>
                            <p class="text-sm font-semibold text-vs-text">${toast.esc(m.provider)} &middot; ****${toast.esc(m.lastFour)}</p>
                            <p class="text-xs text-vs-dim mt-0.5">Expires ${String(m.expiryMonth).padStart(2, '0')}/${m.expiryYear} ${m.isDefault ? '&middot; <span class="text-vs-accent font-semibold">Default</span>' : ''}</p>
                        </div>
                    </div>
                    <div class="flex items-center gap-2">
                        ${!m.isDefault ? `<button class="text-xs text-vs-accent hover:underline set-default-btn" data-id="${m.id}">Set Default</button>` : ''}
                        <button class="p-2 text-vs-muted hover:text-vs-error delete-pm-btn" data-id="${m.id}" title="Delete Card"><i class="fas fa-trash-alt text-xs"></i></button>
                    </div>
                </div>`;
            });
            h += `<button class="w-full p-4 rounded-xl border-2 border-dashed border-vs-border hover:border-vs-dim text-vs-muted text-sm font-semibold transition-colors" id="add-payment-btn"><i class="fas fa-plus mr-2"></i>Add Payment Method</button>`;
            h += `</div>`;

            // Billing history
            if (state.invoices && state.invoices.items && state.invoices.items.length) {
                h += `<h2 class="font-display font-semibold text-xl text-vs-text mb-4">Billing History</h2>`;
                
                const rows = state.invoices.items.map(inv => `<tr class="table-row border-b border-vs-border">
                    <td class="px-4 py-3 text-vs-dim text-sm">${utils.formatDateShort(inv.invoiceDate)}</td>
                    <td class="px-4 py-3 text-vs-text text-sm font-medium">${toast.esc(inv.subscriptionPlan)}</td>
                    <td class="px-4 py-3 text-vs-text text-sm font-semibold">$${inv.amount.toFixed(2)}</td>
                    <td class="px-4 py-3">${utils.statusBadge(inv.status)}</td>
                    <td class="px-4 py-3 text-right">${inv.invoicePdfUrl ? `<a href="${toast.esc(inv.invoicePdfUrl)}" class="text-xs text-vs-accent font-semibold hover:underline" target="_blank"><i class="fas fa-download mr-1"></i>PDF</a>` : ''}</td>
                </tr>`).join('');

                h += Comp.dataTable(
                    [
                        { key: 'invoiceDate', label: 'Date' },
                        { key: 'subscriptionPlan', label: 'Plan' },
                        { key: 'amount', label: 'Amount' },
                        { key: 'status', label: 'Status' },
                        { key: '', label: '' }
                    ],
                    rows,
                    'No billing history found'
                );
            }
        }

        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('cancel-auto-renew-btn')?.addEventListener('click', () => {
            const newFlag = !state.sub.autoRenew;
            api.put(`/subscriptions/${state.sub.id}`, { autoRenew: newFlag }).then(() => {
                state.sub.autoRenew = newFlag;
                toast.success(newFlag ? 'Auto-renew enabled' : 'Auto-renew disabled');
                load();
            }).catch(err => toast.error(err.message));
        });

        document.getElementById('cancel-sub-btn')?.addEventListener('click', () => {
            modal.open('Cancel Subscription', 
                `<p class="text-sm text-vs-dim">Are you sure you want to cancel your subscription? You will lose access to premium features at the end of your billing cycle.</p>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-danger" id="confirm-cancel-sub">Cancel Subscription</button>`
            );
            document.getElementById('confirm-cancel-sub')?.addEventListener('click', () => {
                api.post(`/subscriptions/${state.sub.id}/cancel`).then(() => {
                    state.sub = null;
                    toast.success('Subscription cancelled');
                    modal.close();
                    load();
                }).catch(err => toast.error(err.message));
            });
        });

        document.querySelectorAll('.plan-select-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const planType = btn.dataset.plan;
                if (state.sub) {
                    modal.open('Change Plan',
                        `<p class="text-sm text-vs-dim">Switch to <strong class="text-vs-text">${toast.esc(planType)}</strong> plan? Your billing will be pro-rated immediately.</p>`,
                        `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                         <button class="btn btn-primary" id="confirm-plan-btn">Confirm Change</button>`);
                    document.getElementById('confirm-plan-btn')?.addEventListener('click', () => {
                        api.put(`/subscriptions/${state.sub.id}`, { planType }).then(() => {
                            toast.success('Plan changed to ' + planType);
                            modal.close();
                            load();
                        }).catch(err => toast.error(err.message));
                    });
                } else {
                    if (!state.methods || state.methods.length === 0) {
                        toast.warning('Please add a payment method before subscribing');
                        document.getElementById('add-payment-btn')?.click();
                        return;
                    }
                    const methodOptions = state.methods.map(m =>
                        `<option value="${m.id}" ${m.isDefault ? 'selected' : ''}>${toast.esc(m.provider)} ****${toast.esc(m.lastFour)} ${m.isDefault ? '(Default)' : ''}</option>`).join('');

                    modal.open(`Subscribe to ${planType}`,
                        `<div class="space-y-4 font-body">
                            <div class="p-4 rounded-xl bg-vs-accent/5 border border-vs-accent/20">
                                <p class="text-sm font-semibold text-vs-text">${toast.esc(planType)} Plan</p>
                                <p class="text-xs text-vs-muted mt-0.5">Your card will be charged monthly. Cancel anytime.</p>
                            </div>
                            <div>
                                <label class="form-label">Payment Method</label>
                                <select id="sub-payment-method" class="input-field mt-1">${methodOptions}</select>
                            </div>
                            <label class="flex items-center gap-2 cursor-pointer select-none">
                                <input type="checkbox" id="sub-auto-renew" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent" checked>
                                <span class="text-sm text-vs-dim">Enable auto-renewal</span>
                            </label>
                        </div>`,
                        `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                         <button class="btn btn-primary" id="confirm-sub-btn"><i class="fas fa-credit-card mr-1.5"></i> Subscribe Now</button>`);

                    document.getElementById('confirm-sub-btn')?.addEventListener('click', async () => {
                        const paymentMethodId = parseInt(document.getElementById('sub-payment-method').value);
                        const autoRenew = document.getElementById('sub-auto-renew').checked;
                        const btn2 = document.getElementById('confirm-sub-btn');
                        btn2.disabled = true;
                        try {
                            await api.post('/subscriptions', { planType, paymentMethodId, autoRenew });
                            toast.success(`Subscribed to ${planType}! Welcome aboard ًںژ‰`);
                            modal.close();
                            load();
                        } catch (err) { toast.error(err.message); }
                        finally { btn2.disabled = false; }
                    });
                }
            });
        });

        document.getElementById('promo-apply-btn')?.addEventListener('click', () => {
            const code = document.getElementById('promo-input').value.trim();
            if (!code) { toast.warning('Enter a promo code'); return; }
            api.post('/promocodes/validate', { code }).then(res => {
                document.getElementById('promo-result').innerHTML = res.isValid
                    ? `<span class="text-vs-success font-semibold flex items-center gap-1"><i class="fas fa-check-circle mr-1"></i>${toast.esc(res.message)} ${res.discountAmount ? `(-$${res.discountAmount.toFixed(2)})` : ''}</span>`
                    : `<span class="text-vs-error font-semibold flex items-center gap-1"><i class="fas fa-times-circle mr-1"></i>${toast.esc(res.message)}</span>`;
            }).catch(err => {
                document.getElementById('promo-result').innerHTML = `<span class="text-vs-error font-semibold">${toast.esc(err.message)}</span>`;
            });
        });

        document.querySelectorAll('.set-default-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                api.post(`/paymentmethods/${btn.dataset.id}/set-default`).then(() => {
                    toast.success('Default payment method updated');
                    load();
                }).catch(err => toast.error(err.message));
            });
        });

        document.querySelectorAll('.delete-pm-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                api.del(`/paymentmethods/${btn.dataset.id}`).then(() => {
                    toast.success('Payment method removed');
                    load();
                }).catch(err => toast.error(err.message));
            });
        });

        document.getElementById('add-payment-btn')?.addEventListener('click', () => {
            modal.open('Add Payment Method',
                `<div class="space-y-4 font-body">
                    <div class="grid grid-cols-2 gap-4">
                        <div class="col-span-2">
                            <label class="form-label">Card Provider <span class="text-vs-error">*</span></label>
                            <select id="pm-provider" class="input-field mt-1">
                                <option value="">Select provider</option>
                                <option value="Visa">Visa</option>
                                <option value="Mastercard">Mastercard</option>
                                <option value="Amex">American Express</option>
                                <option value="Discover">Discover</option>
                                <option value="PayPal">PayPal</option>
                            </select>
                        </div>
                        <div class="col-span-2">
                            <label class="form-label">Card Holder Name <span class="text-vs-error">*</span></label>
                            <input type="text" id="pm-holder" class="input-field mt-1" placeholder="Name as on card">
                        </div>
                        <div class="col-span-2">
                            <label class="form-label">Last 4 Digits <span class="text-vs-error">*</span></label>
                            <input type="text" id="pm-last-four" class="input-field mt-1" placeholder="e.g. 4242" maxlength="4" pattern="[0-9]{4}">
                        </div>
                        <div>
                            <label class="form-label">Expiry Month</label>
                            <select id="pm-month" class="input-field mt-1">
                                ${Array.from({length:12},(_,i)=> `<option value="${i+1}">${String(i+1).padStart(2,'0')}</option>`).join('')}
                            </select>
                        </div>
                        <div>
                            <label class="form-label">Expiry Year</label>
                            <select id="pm-year" class="input-field mt-1">
                                ${Array.from({length:10},(_,i)=> `<option value="${new Date().getFullYear()+i}">${new Date().getFullYear()+i}</option>`).join('')}
                            </select>
                        </div>
                        <div class="col-span-2">
                            <label class="flex items-center gap-2 cursor-pointer select-none">
                                <input type="checkbox" id="pm-default" class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent" ${state.methods.length === 0 ? 'checked disabled' : ''}>
                                <span class="text-sm text-vs-dim">Set as default payment method</span>
                            </label>
                        </div>
                    </div>
                    <p class="text-xs text-vs-muted"><i class="fas fa-lock mr-1"></i>Payment details are tokenised and never stored on our servers.</p>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-pm-btn"><i class="fas fa-credit-card mr-1.5"></i> Add Card</button>`);

            document.getElementById('save-pm-btn')?.addEventListener('click', async () => {
                const provider = document.getElementById('pm-provider').value;
                const holder = document.getElementById('pm-holder').value.trim();
                const lastFour = document.getElementById('pm-last-four').value.trim();
                const month = parseInt(document.getElementById('pm-month').value);
                const year = parseInt(document.getElementById('pm-year').value);
                const isDefault = document.getElementById('pm-default').checked;

                if (!provider) { toast.warning('Please select a card provider'); return; }
                if (!holder) { toast.warning('Card holder name is required'); return; }
                if (!/^\d{4}$/.test(lastFour)) { toast.warning('Last 4 digits must be exactly 4 numbers'); return; }

                const btn2 = document.getElementById('save-pm-btn');
                btn2.disabled = true;
                try {
                    await api.post('/paymentmethods', {
                        provider,
                        cardHolderName: holder,
                        lastFour,
                        providerToken: `tok_${provider.toLowerCase()}_${lastFour}`,
                        expiryMonth: month,
                        expiryYear: year,
                        isDefault
                    });
                    toast.success('Payment method added');
                    modal.close();
                    load();
                } catch (err) { toast.error(err.message); }
                finally { btn2.disabled = false; }
            });
        });
    }

    return {
        render() { return '<div id="subscriptions-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();