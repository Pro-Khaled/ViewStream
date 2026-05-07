pages.adminPromos = (() => {
    let state = { page: 1, data: null, loading: true };
    let sortKey = 'code';
    let sortDir = 'asc';

    async function loadData() {
        state.loading = true; render();
        try {
            state.data = await api.get('/promocodes', {
                page: state.page,
                pageSize: CONFIG.PAGE_SIZE,
                orderBy: sortKey,
                isDescending: sortDir === 'desc'
            });
        } catch (err) { toast.error('Failed to load promos'); }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('admin-promos-content');
        if (!c) return;
        let h = Comp.pageHeader('Promo Codes', '', '<button id="create-promo-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus"></i> Create Code</button>');
        if (state.loading) { h += Comp.pageLoader(); }
        else if (!state.data?.items?.length) { h += Comp.emptyState('fa-tag', 'No promo codes'); }
        else {
            const rows = state.data.items.map(p => `<tr>
                <td class="font-mono font-bold text-vs-accent">${toast.esc(p.code)}</td>
                <td class="text-sm text-vs-text">${p.discountPercent ? p.discountPercent + '%' : '$' + p.discountAmount}</td>
                <td class="text-sm text-vs-dim">${utils.formatDateShort(p.validUntil)}</td>
                <td class="text-sm text-vs-dim">${p.usedCount}/${p.maxUses || '?'}</td>
                <td class="text-sm text-vs-dim">${p.remainingUses}</td>
                <td>${p.isValid ? '<span class="badge badge-success">Valid</span>' : '<span class="badge badge-danger">Expired</span>'}</td>
                <td><button class="btn btn-ghost btn-sm edit-promo-btn" data-id="${p.id}"><i class="fas fa-edit"></i></button>
                    <button class="btn btn-ghost btn-sm text-vs-error delete-promo-btn" data-id="${p.id}"><i class="fas fa-trash"></i></button></td>
            </tr>`).join('');
            h += Comp.dataTable(
                [
                    { key: 'code', label: 'Code' },
                    { key: 'discountPercent', label: 'Discount' },
                    { key: 'validUntil', label: 'Valid Until' },
                    { key: 'usedCount', label: 'Used' },
                    { key: '', label: 'Remaining' }, // remaining uses is computed – non-sortable
                    { key: 'isValid', label: 'Status' },
                    { key: '', label: '' }
                ],
                rows,
                'No promo codes',
                {
                    tableId: 'promos-table',
                    sortKey,
                    sortDir,
                    onSort: (key, dir) => { if (key) { sortKey = key; sortDir = dir; loadData(); } }
                }
            );
            h += Comp.pagination(state.data.pageNumber, state.data.totalPages, p => { state.page = p; loadData(); });
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('create-promo-btn')?.addEventListener('click', () => {
            modal.open('Create Promo Code', `<div class="space-y-4">
                <div><label class="form-label">Code *</label><input class="input-field" id="promo-code" placeholder="SUMMER25"></div>
                <div class="grid grid-cols-2 gap-4">
                    <div><label class="form-label">Discount %</label><input class="input-field" type="number" id="promo-pct"></div>
                    <div><label class="form-label">Flat Amount $</label><input class="input-field" type="number" step="0.01" id="promo-amt"></div>
                </div>
                <div class="grid grid-cols-2 gap-4">
                    <div><label class="form-label">Valid From</label><input class="input-field" type="date" id="promo-from"></div>
                    <div><label class="form-label">Valid Until</label><input class="input-field" type="date" id="promo-until"></div>
                </div>
                <div><label class="form-label">Max Uses</label><input class="input-field" type="number" id="promo-max"></div>
                <div><label class="form-label">Applies to Plan</label><select class="input-field" id="promo-plan"><option value="">All Plans</option><option>Basic</option><option>Premium</option><option>Ultra</option></select></div>
            </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-promo">Create</button>`);
            document.getElementById('save-promo')?.addEventListener('click', async () => {
                const code = document.getElementById('promo-code').value.trim();
                if (!code) { toast.warning('Code required'); return; }
                await api.post('/promocodes', {
                    code,
                    discountPercent: parseInt(document.getElementById('promo-pct').value) || null,
                    discountAmount: parseFloat(document.getElementById('promo-amt').value) || null,
                    validFrom: document.getElementById('promo-from').value || new Date().toISOString().slice(0, 10),
                    validUntil: document.getElementById('promo-until').value || null,
                    maxUses: parseInt(document.getElementById('promo-max').value) || null,
                    appliesToPlan: document.getElementById('promo-plan').value || null
                });
                modal.close(); toast.success('Promo code created'); loadData();
            });
        });
        document.querySelectorAll('.edit-promo-btn').forEach(b => b.addEventListener('click', async () => {
            const id = b.dataset.id;
            const promo = state.data.items.find(p => p.id == id);
            if (!promo) return;
            modal.open('Edit Promo', `<div class="space-y-4">
                <div><label class="form-label">Discount %</label><input class="input-field" type="number" id="ep-pct" value="${promo.discountPercent || ''}"></div>
                <div><label class="form-label">Flat Amount $</label><input class="input-field" type="number" step="0.01" id="ep-amt" value="${promo.discountAmount || ''}"></div>
                <div><label class="form-label">Valid Until</label><input class="input-field" type="date" id="ep-until" value="${promo.validUntil || ''}"></div>
                <div><label class="form-label">Max Uses</label><input class="input-field" type="number" id="ep-max" value="${promo.maxUses || ''}"></div>
            </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-edit-promo">Save</button>`);
            document.getElementById('save-edit-promo')?.addEventListener('click', async () => {
                await api.put(`/promocodes/${id}`, {
                    discountPercent: parseInt(document.getElementById('ep-pct').value) || null,
                    discountAmount: parseFloat(document.getElementById('ep-amt').value) || null,
                    validUntil: document.getElementById('ep-until').value || null,
                    maxUses: parseInt(document.getElementById('ep-max').value) || null
                });
                modal.close(); toast.success('Promo updated'); loadData();
            });
        }));
        document.querySelectorAll('.delete-promo-btn').forEach(b => b.addEventListener('click', async () => {
            if (confirm('Delete this promo code?')) {
                await api.del(`/promocodes/${b.dataset.id}`); toast.success('Promo deleted'); loadData();
            }
        }));
    }

    return {
        render() { return '<div id="admin-promos-content">' + Comp.pageLoader() + '</div>'; },
        init() { loadData(); }
    };
})();