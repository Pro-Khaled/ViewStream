pages.adminContentSub = (() => {
    let data = [];
    let columns = [];
    let type = 'genres';

    let sortKey = 'name';
    let sortDir = 'asc';

    async function load(subType) {
        type = subType;
        if (type === 'genres') {
            data = await api.get('/genres/all');
            columns = ['ID', 'Name', 'Show Count', ''];
        } else if (type === 'tags') {
            data = await api.get('/contenttags/all');
            columns = ['ID', 'Name', 'Category', 'Show Count', ''];
        } else {
            data = await api.get('/persons/all');
            columns = ['ID', 'Name', 'Credits', 'Awards', ''];
        }

        // Sort data locally (API returns full list, no pagination)
        if (sortKey) {
            data.sort((a, b) => {
                let valA = a[sortKey];
                let valB = b[sortKey];
                if (typeof valA === 'string') valA = valA.toLowerCase();
                if (typeof valB === 'string') valB = valB.toLowerCase();
                if (valA < valB) return sortDir === 'asc' ? -1 : 1;
                if (valA > valB) return sortDir === 'asc' ? 1 : -1;
                return 0;
            });
        }
        render();
    }

    function render() {
        const c = document.getElementById('page-content');
        if (!c) return;

        let h = `<div class="flex items-center justify-between mb-6">
            <div class="flex items-center gap-3">
                <a href="#/admin/content" class="text-vs-muted hover:text-vs-text"><i class="fas fa-arrow-left"></i></a>
                <h1 class="font-display font-bold text-2xl text-vs-text capitalize">${toast.esc(type)}</h1>
            </div>
            <button class="btn btn-primary btn-sm" id="add-item-btn">
                <i class="fas fa-plus mr-1"></i> Add
            </button>
        </div>`;

        const rows = data.map(item => {
            const id = item.id || item.code; // countries have 'code'
            const name = item.name;
            let row = `<tr>
                <td class="text-muted">${id}</td>
                <td class="font-medium text-vs-text">${toast.esc(name)}</td>`;
            if (item.showCount !== undefined) row += `<td class="text-sm text-vs-dim">${item.showCount}</td>`;
            if (item.category !== undefined) row += `<td class="text-xs text-vs-dim">${toast.esc(item.category || '—')}</td>`;
            if (item.creditCount !== undefined) row += `<td class="text-sm text-vs-dim">${item.creditCount}</td>`;
            if (item.awardCount !== undefined) row += `<td class="text-sm text-vs-dim">${item.awardCount}</td>`;
            row += `<td>
                <button class="btn btn-ghost btn-sm"><i class="fas fa-edit"></i></button>
                <button class="btn btn-ghost btn-sm text-vs-error"><i class="fas fa-trash"></i></button>
            </td></tr>`;
            return row;
        }).join('');

        // Build column definitions for dataTable
        const colDefs = [];
        colDefs.push({ key: 'id', label: 'ID' });
        colDefs.push({ key: 'name', label: 'Name' });
        if (type !== 'persons') colDefs.push({ key: 'showCount', label: 'Show Count' });
        if (type === 'tags') colDefs.push({ key: 'category', label: 'Category' });
        if (type === 'persons') {
            colDefs.push({ key: 'creditCount', label: 'Credits' });
            colDefs.push({ key: 'awardCount', label: 'Awards' });
        }
        colDefs.push({ key: '', label: '' }); // actions column

        h += Comp.dataTable(
            colDefs,
            rows,
            `No ${type} found`,
            {
                tableId: 'content-sub-table',
                sortKey,
                sortDir,
                onSort: (key, dir) => {
                    if (key) {
                        sortKey = key;
                        sortDir = dir;
                        load(type); // re-sort by reloading
                    }
                }
            }
        );

        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('add-item-btn')?.addEventListener('click', () => {
            const fields = type === 'genres'
                ? [{ id: 'cg-name', label: 'Genre Name', type: 'text' }]
                : type === 'tags'
                    ? [{ id: 'ct-name', label: 'Tag Name', type: 'text' }, { id: 'ct-cat', label: 'Category', type: 'text' }]
                    : [{ id: 'cp-name', label: 'Name', type: 'text' }, { id: 'cp-bio', label: 'Bio', type: 'textarea' }, { id: 'cp-photo', label: 'Photo URL', type: 'text' }];

            const body = fields.map(f => f.type === 'textarea'
                ? `<div class="form-group"><label class="form-label">${toast.esc(f.label)}</label><textarea id="${f.id}" class="input-field" rows="3"></textarea></div>`
                : `<div class="form-group"><label class="form-label">${toast.esc(f.label)}</label><input type="text" id="${f.id}" class="input-field"></div>`
            ).join('');

            modal.open(`Add ${type.slice(0, -1)}`, `<div class="space-y-4">${body}</div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-item-btn">Create</button>`);

            document.getElementById('save-item-btn')?.addEventListener('click', async () => {
                // Create logic here
                toast.success('Created successfully');
                modal.close();
                load(type);
            });
        });
    }

    return {
        render(params) { return '<div id="page-content">' + Comp.pageLoader() + '</div>'; },
        init(params) { load(params.type || 'genres'); }
    };
})();