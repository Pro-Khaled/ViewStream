pages.adminContentSub = (() => {
    let data = [];
    let type = 'genres';
    let sortKey = 'name';
    let sortDir = 'asc';

    // Config per sub-type: endpoint, fields for create/edit form, table columns
    const CONFIG_MAP = {
        genres: {
            endpoint: '/genres',
            listEndpoint: '/genres/all',
            singular: 'Genre',
            fields: [
                { id: 'f-name', label: 'Name', prop: 'name', required: true }
            ],
            cols: [
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Name' },
                { key: 'showCount', label: 'Shows' },
                { key: '', label: '' }
            ]
        },
        tags: {
            endpoint: '/contenttags',
            listEndpoint: '/contenttags/all',
            singular: 'Tag',
            fields: [
                { id: 'f-name', label: 'Tag Name', prop: 'name', required: true },
                { id: 'f-cat', label: 'Category', prop: 'category' }
            ],
            cols: [
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Name' },
                { key: 'category', label: 'Category' },
                { key: 'showCount', label: 'Shows' },
                { key: '', label: '' }
            ]
        },
        persons: {
            endpoint: '/persons',
            listEndpoint: '/persons/all',
            singular: 'Person',
            fields: [
                { id: 'f-name', label: 'Full Name', prop: 'name', required: true },
                { id: 'f-bio', label: 'Biography', prop: 'bio', type: 'textarea' },
                { id: 'f-photo', label: 'Photo URL', prop: 'photoUrl' },
                { id: 'f-birth', label: 'Birth Date', prop: 'birthDate', type: 'date' },
                { id: 'f-nationality', label: 'Nationality', prop: 'nationality' }
            ],
            cols: [
                { key: 'id', label: 'ID' },
                { key: 'name', label: 'Name' },
                { key: 'nationality', label: 'Nationality' },
                { key: 'creditCount', label: 'Credits' },
                { key: '', label: '' }
            ]
        }
    };

    async function load(subType) {
        type = subType;
        const cfg = CONFIG_MAP[type];
        try {
            data = await api.get(cfg.listEndpoint) || [];
        } catch (err) {
            toast.error('Failed to load ' + type);
            data = [];
        }
        sortData();
        render();
    }

    function sortData() {
        if (!sortKey) return;
        data.sort((a, b) => {
            let vA = a[sortKey]; let vB = b[sortKey];
            if (typeof vA === 'string') vA = vA.toLowerCase();
            if (typeof vB === 'string') vB = vB.toLowerCase();
            if (vA < vB) return sortDir === 'asc' ? -1 : 1;
            if (vA > vB) return sortDir === 'asc' ? 1 : -1;
            return 0;
        });
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;
        const cfg = CONFIG_MAP[type];

        let h = `<div class="flex items-center justify-between mb-6">
            <div class="flex items-center gap-3">
                <a href="#/admin/content" class="w-8 h-8 flex items-center justify-center rounded-lg bg-vs-card hover:bg-vs-elevated text-vs-muted hover:text-vs-text transition-all border border-vs-border">
                    <i class="fas fa-arrow-left text-sm"></i>
                </a>
                <h1 class="font-display font-bold text-2xl text-vs-text capitalize">${toast.esc(type)}</h1>
                <span class="px-2 py-0.5 rounded-full bg-vs-card border border-vs-border text-vs-muted text-xs">${data.length}</span>
            </div>
            <button class="btn btn-primary btn-sm" id="add-item-btn">
                <i class="fas fa-plus mr-1.5"></i> Add ${cfg.singular}
            </button>
        </div>`;

        const rows = data.map(item => {
            let row = `<tr>
                <td class="text-vs-muted text-sm">${item.id || item.code}</td>
                <td class="font-medium text-vs-text">${toast.esc(item.name)}</td>`;
            if (type === 'tags') row += `<td class="text-xs text-vs-dim">${toast.esc(item.category || 'â€”')}</td>`;
            if (type !== 'persons') row += `<td class="text-sm text-vs-dim">${item.showCount ?? 'â€”'}</td>`;
            if (type === 'persons') row += `<td class="text-xs text-vs-dim">${toast.esc(item.nationality || 'â€”')}</td>
                <td class="text-sm text-vs-dim">${item.creditCount ?? 'â€”'}</td>`;
            row += `<td class="text-right">
                <div class="flex items-center justify-end gap-1.5">
                    <button class="btn btn-ghost btn-sm edit-item-btn" data-index="${data.indexOf(item)}" title="Edit"><i class="fas fa-edit text-xs"></i></button>
                    <button class="btn btn-ghost btn-sm text-vs-error delete-item-btn" data-id="${item.id}" data-name="${toast.esc(item.name)}" title="Delete"><i class="fas fa-trash-alt text-xs"></i></button>
                </div>
            </td></tr>`;
            return row;
        }).join('');

        h += Comp.dataTable(cfg.cols, rows, `No ${type} found`, {
            tableId: 'content-sub-table',
            sortKey,
            sortDir,
            onSort: (key, dir) => { if (key) { sortKey = key; sortDir = dir; sortData(); render(); } }
        });

        c.innerHTML = h;
        bindEvents();
    }

    function buildFormFields(item = null) {
        const cfg = CONFIG_MAP[type];
        return cfg.fields.map(f => {
            const val = item ? (item[f.prop] || '') : '';
            const label = `<label class="form-label">${toast.esc(f.label)}${f.required ? ' <span class="text-vs-error">*</span>' : ''}</label>`;
            if (f.type === 'textarea') {
                return `<div>${label}<textarea id="${f.id}" class="input-field" rows="3">${toast.esc(val)}</textarea></div>`;
            }
            return `<div>${label}<input type="${f.type || 'text'}" id="${f.id}" class="input-field" value="${toast.esc(val)}" placeholder="${toast.esc(f.label)}"></div>`;
        }).join('');
    }

    function collectFormData() {
        const cfg = CONFIG_MAP[type];
        const payload = {};
        cfg.fields.forEach(f => {
            const el = document.getElementById(f.id);
            if (el) payload[f.prop] = el.value.trim() || null;
        });
        return payload;
    }

    function showCreateModal() {
        const cfg = CONFIG_MAP[type];
        modal.open(`Add ${cfg.singular}`, `<div class="space-y-4">${buildFormFields()}</div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-item-btn"><i class="fas fa-plus mr-1"></i> Create</button>`);
        document.getElementById('save-item-btn')?.addEventListener('click', async () => {
            const payload = collectFormData();
            const firstField = CONFIG_MAP[type].fields.find(f => f.required);
            if (firstField && !payload[firstField.prop]) { toast.warning(`${firstField.label} is required`); return; }
            try {
                await api.post(cfg.endpoint, payload);
                toast.success(`${cfg.singular} created`);
                modal.close();
                load(type);
            } catch (err) { toast.error(err.message); }
        });
    }

    function showEditModal(item) {
        const cfg = CONFIG_MAP[type];
        modal.open(`Edit ${cfg.singular}: ${item.name}`, `<div class="space-y-4">${buildFormFields(item)}</div>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="save-item-btn"><i class="fas fa-save mr-1"></i> Save</button>`);
        document.getElementById('save-item-btn')?.addEventListener('click', async () => {
            const payload = collectFormData();
            try {
                await api.put(`${cfg.endpoint}/${item.id}`, payload);
                toast.success(`${cfg.singular} updated`);
                modal.close();
                load(type);
            } catch (err) { toast.error(err.message); }
        });
    }

    function showDeleteModal(id, name) {
        const cfg = CONFIG_MAP[type];
        modal.open(`Delete ${cfg.singular}`,
            `<p class="text-sm text-vs-dim">Delete <strong class="text-vs-text">"${toast.esc(name)}"</strong>? This may affect associated shows.</p>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-danger" id="confirm-del-btn">Delete</button>`);
        document.getElementById('confirm-del-btn')?.addEventListener('click', async () => {
            try {
                await api.del(`${cfg.endpoint}/${id}`);
                toast.success(`${cfg.singular} deleted`);
                modal.close();
                load(type);
            } catch (err) { toast.error(err.message); }
        });
    }

    function bindEvents() {
        document.getElementById('add-item-btn')?.addEventListener('click', showCreateModal);

        document.getElementById('content-sub-table')?.addEventListener('click', e => {
            const btn = e.target.closest('button');
            if (!btn) return;
            if (btn.classList.contains('edit-item-btn')) {
                const idx = parseInt(btn.dataset.index);
                showEditModal(data[idx]);
            }
            if (btn.classList.contains('delete-item-btn')) {
                showDeleteModal(btn.dataset.id, btn.dataset.name);
            }
        });
    }

    return {
        render(params) { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init(params) {
            const path = window.location.hash;
            const subType = path.includes('/genres') ? 'genres' : path.includes('/tags') ? 'tags' : 'persons';
            load(subType);
        }
    };
})();