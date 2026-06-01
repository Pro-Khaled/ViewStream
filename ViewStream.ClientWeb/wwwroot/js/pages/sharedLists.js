pages.sharedLists = (() => {
    let state = { lists: [], loading: true };

    async function load() {
        state.loading = true; render();
        try {
            const lists = await api.get('/profiles/me/lists');
            // Fetch items for all lists in parallel
            state.lists = await Promise.all(lists.map(async (list) => {
                try {
                    const items = await api.get(`/lists/${list.id}/items`);
                    return { ...list, items: items || [] };
                } catch (e) {
                    return { ...list, items: [] };
                }
            }));
        } catch (err) {
            toast.error('Failed to load lists: ' + err.message);
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('shared-lists-content');
        if (!c) return;

        let h = Comp.pageHeader('Shared Lists', 'Create and curate lists to share with friends',
            '<button id="new-list-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus mr-1"></i> New Curated List</button>');

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.lists.length) {
            h += Comp.emptyState('fa-list', 'No shared lists yet');
        } else {
            h += '<div class="grid gap-6 font-body">';
            state.lists.forEach((list) => {
                const isPublicText = list.isPublic ? '<span class="badge badge-success ml-1 text-xs">Public</span>' : '<span class="badge badge-muted ml-1 text-xs">Private</span>';
                const shareCodeHtml = list.shareCode ? 
                    `<span class="text-xs font-mono bg-vs-card border border-vs-border px-2 py-0.5 rounded text-vs-accent">Code: ${toast.esc(list.shareCode)}</span>` : 
                    `<button class="text-xs text-vs-accent hover:underline generate-code-btn" data-list-id="${list.id}"><i class="fas fa-magic mr-1"></i>Generate Code</button>`;

                h += `<div class="p-5 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors" data-list-id="${list.id}">
                    <div class="flex items-start justify-between mb-3 gap-4">
                        <div>
                            <div class="flex items-center gap-2">
                                <h3 class="font-display font-bold text-lg text-vs-text">${toast.esc(list.name)}</h3>
                                ${isPublicText}
                            </div>
                            <p class="text-xs text-vs-dim mt-1 flex items-center gap-2 flex-wrap">
                                <span>Curated by ${toast.esc(list.ownerProfileName || 'You')}</span>
                                <span class="text-vs-muted">&middot;</span>
                                <span>${list.items?.length || 0} shows</span>
                                <span class="text-vs-muted">&middot;</span>
                                ${shareCodeHtml}
                            </p>
                        </div>
                        <div class="flex items-center gap-1.5 flex-shrink-0">
                            ${list.shareCode ? `<button class="p-2 text-vs-muted hover:text-vs-accent share-list-btn" data-list-id="${list.id}" title="Copy Share Code"><i class="fas fa-copy text-sm"></i></button>` : ''}
                            <button class="p-2 text-vs-muted hover:text-vs-accent edit-list-btn" data-list-id="${list.id}" data-name="${toast.esc(list.name)}" data-desc="${toast.esc(list.description || '')}" data-public="${list.isPublic}" title="Edit List"><i class="fas fa-edit text-sm"></i></button>
                            <button class="p-2 text-vs-muted hover:text-vs-error delete-list-btn" data-list-id="${list.id}" title="Delete List"><i class="fas fa-trash-alt text-sm"></i></button>
                        </div>
                    </div>
                    ${list.description ? `<p class="text-sm text-vs-dim mb-4 leading-relaxed">${toast.esc(list.description)}</p>` : ''}
                    
                    <div class="scroll-row px-1">
                        ${(list.items || []).map(item => `<div class="flex-shrink-0 w-28 text-center group relative border border-vs-border/50 rounded-lg p-1.5 bg-vs-card/40 hover:bg-vs-card/85 transition-colors">
                            <button class="absolute top-1 right-1 w-6 h-6 rounded-full bg-vs-bg/80 border border-vs-border flex items-center justify-center text-vs-error opacity-0 group-hover:opacity-100 hover:bg-vs-error hover:text-white transition-all remove-item-btn" data-list-id="${list.id}" data-show-id="${item.showId}" title="Remove from list">
                                <i class="fas fa-times text-[10px]"></i>
                            </button>
                            <a href="#/shows/${item.showId}" class="block">
                                <img src="${toast.esc(item.showPosterUrl)}" alt="${toast.esc(item.showTitle)}" class="w-20 h-28 mx-auto rounded-md object-cover mb-2 shadow">
                                <p class="text-xs text-vs-text truncate font-medium">${toast.esc(item.showTitle)}</p>
                            </a>
                        </div>`).join('')}
                        ${!(list.items?.length) ? `<div class="text-xs text-vs-muted py-4">No shows in this list yet. Browse shows and add some!</div>` : ''}
                    </div>
                </div>`;
            });
            h += '</div>';
        }
        c.innerHTML = h;
        bindEvents();
    }

    function bindEvents() {
        document.getElementById('new-list-btn')?.addEventListener('click', () => {
            modal.open('Create Shared List', 
                `<div class="space-y-4 font-body">
                    <div class="form-group">
                        <label class="form-label">List Name <span class="text-vs-error">*</span></label>
                        <input id="list-name" class="input-field mt-1" placeholder="e.g. My Favorite Sci-Fi">
                    </div>
                    <div class="form-group">
                        <label class="form-label">Description</label>
                        <textarea id="list-desc" class="input-field mt-1 h-20 placeholder-vs-muted resize-none" placeholder="What is this list about?"></textarea>
                    </div>
                    <label class="flex items-center gap-2 cursor-pointer select-none">
                        <input type="checkbox" id="list-public" checked class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                        <span class="text-sm text-vs-dim">Make this list public</span>
                    </label>
                </div>`,
                `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                 <button class="btn btn-primary" id="save-new-list">Create List</button>`
            );

            document.getElementById('save-new-list')?.addEventListener('click', async () => {
                const name = document.getElementById('list-name').value.trim();
                if (!name) { toast.warning('List Name is required'); return; }
                const desc = document.getElementById('list-desc').value.trim();
                const isPublic = document.getElementById('list-public').checked;
                try {
                    await api.post('/profiles/me/lists', { name, description: desc, isPublic });
                    modal.close();
                    toast.success('List created successfully!');
                    load();
                } catch (err) {
                    toast.error(err.message);
                }
            });
        });

        const container = document.getElementById('shared-lists-content');
        container?.addEventListener('click', async e => {
            const btn = e.target.closest('button');
            if (!btn) return;
            const listId = parseInt(btn.dataset.listId);

            if (btn.classList.contains('share-list-btn')) {
                const list = state.lists.find(l => l.id === listId);
                if (list && list.shareCode) {
                    navigator.clipboard.writeText(list.shareCode);
                    toast.success('Share code copied to clipboard: ' + list.shareCode);
                }
            } else if (btn.classList.contains('generate-code-btn')) {
                btn.disabled = true;
                btn.innerHTML = '<i class="fas fa-spinner fa-spin mr-1"></i>Generating...';
                try {
                    const res = await api.post(`/profiles/me/lists/${listId}/share-code`);
                    toast.success('Share code generated successfully!');
                    load();
                } catch (err) {
                    toast.error('Failed to generate share code: ' + err.message);
                }
            } else if (btn.classList.contains('delete-list-btn')) {
                modal.open('Delete Shared List', 
                    `<p class="text-sm text-vs-dim">Are you sure you want to delete this list? This cannot be undone.</p>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-danger" id="confirm-delete-list">Delete List</button>`
                );
                document.getElementById('confirm-delete-list')?.addEventListener('click', async () => {
                    try {
                        await api.del(`/profiles/me/lists/${listId}`);
                        toast.success('List deleted successfully');
                        modal.close();
                        load();
                    } catch (err) {
                        toast.error(err.message);
                    }
                });
            } else if (btn.classList.contains('edit-list-btn')) {
                const name = btn.dataset.name;
                const desc = btn.dataset.desc;
                const isPub = btn.dataset.public === 'true';

                modal.open('Edit Shared List', 
                    `<div class="space-y-4 font-body">
                        <div class="form-group">
                            <label class="form-label">List Name <span class="text-vs-error">*</span></label>
                            <input id="edit-list-name" class="input-field mt-1" value="${toast.esc(name)}">
                        </div>
                        <div class="form-group">
                            <label class="form-label">Description</label>
                            <textarea id="edit-list-desc" class="input-field mt-1 h-20 placeholder-vs-muted resize-none">${toast.esc(desc)}</textarea>
                        </div>
                        <label class="flex items-center gap-2 cursor-pointer select-none">
                            <input type="checkbox" id="edit-list-public" ${isPub ? 'checked' : ''} class="w-4 h-4 rounded border-vs-border bg-vs-card text-vs-accent focus:ring-vs-accent">
                            <span class="text-sm text-vs-dim">Make this list public</span>
                        </label>
                    </div>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-primary" id="save-edit-list">Save Changes</button>`
                );

                document.getElementById('save-edit-list')?.addEventListener('click', async () => {
                    const editName = document.getElementById('edit-list-name').value.trim();
                    if (!editName) { toast.warning('List Name is required'); return; }
                    const editDesc = document.getElementById('edit-list-desc').value.trim();
                    const editPub = document.getElementById('edit-list-public').checked;
                    try {
                        await api.put(`/profiles/me/lists/${listId}`, { name: editName, description: editDesc, isPublic: editPub });
                        modal.close();
                        toast.success('List updated successfully!');
                        load();
                    } catch (err) {
                        toast.error(err.message);
                    }
                });
            } else if (btn.classList.contains('remove-item-btn')) {
                const showId = parseInt(btn.dataset.showId);
                try {
                    await api.del(`/lists/${listId}/items/${showId}`);
                    toast.success('Show removed from list');
                    load();
                } catch (err) {
                    toast.error('Failed to remove show: ' + err.message);
                }
            }
        });
    }

    return {
        render() { return '<div id="shared-lists-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();