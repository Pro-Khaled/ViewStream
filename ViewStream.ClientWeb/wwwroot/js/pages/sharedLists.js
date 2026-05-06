pages.sharedLists = (() => {
    let state = { lists: [], loading: true };

    async function load() {
        state.loading = true;
        render();
        try {
            state.lists = await api.get('/profiles/me/lists');
        } catch (err) {
            toast.error('Failed to load lists: ' + err.message);
        }
        state.loading = false;
        render();
    }

    function render() {
        const c = document.getElementById('shared-lists-content');
        if (!c) return;

        let h = Comp.pageHeader('Shared Lists', 'Create and manage your curated lists',
            '<button id="new-list-btn" class="btn btn-primary btn-sm"><i class="fas fa-plus mr-1"></i> New List</button>');

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.lists.length) {
            h += Comp.emptyState('fa-list', 'No shared lists yet');
        } else {
            h += '<div class="grid gap-4">';
            state.lists.forEach(async (list) => {
                // For each list we fetch its items asynchronously
                let items = [];
                try {
                    const res = await api.get(`/lists/${list.id}/items`);
                    items = res || [];
                } catch (err) {
                    items = [];
                }
                h += `<div class="p-5 rounded-xl bg-vs-surface border border-vs-border hover:border-vs-dim transition-colors">
                    <div class="flex items-start justify-between mb-3">
                        <div>
                            <h3 class="font-display font-semibold text-lg text-vs-text">${toast.esc(list.name)}</h3>
                            <p class="text-xs text-vs-muted mt-0.5">By ${toast.esc(list.ownerProfileName)} &middot; ${list.itemCount} shows ${list.isPublic ? '&middot; <span class="text-vs-accent">Public</span>' : ''}</p>
                        </div>
                        <div class="flex items-center gap-2">
                            <button class="p-2 text-vs-muted hover:text-vs-accent share-list-btn" data-list-id="${list.id}" title="Share"><i class="fas fa-share-alt text-sm"></i></button>
                            <button class="p-2 text-vs-muted hover:text-vs-text edit-list-btn" data-list-id="${list.id}" title="Edit"><i class="fas fa-edit text-sm"></i></button>
                            <button class="p-2 text-vs-muted hover:text-vs-error delete-list-btn" data-list-id="${list.id}" title="Delete"><i class="fas fa-trash-alt text-sm"></i></button>
                        </div>
                    </div>
                    ${list.description ? `<p class="text-sm text-vs-dim mb-3">${toast.esc(list.description)}</p>` : ''}
                    <div class="scroll-row">
                        ${items.map(item => `<div class="flex-shrink-0 w-32 text-center">
                            <a href="#/shows/${item.showId}">
                                <img src="${toast.esc(item.showPosterUrl)}" alt="${toast.esc(item.showTitle)}" class="w-24 h-36 mx-auto rounded-lg object-cover mb-1">
                                <p class="text-xs text-vs-text truncate">${toast.esc(item.showTitle)}</p>
                            </a>
                        </div>`).join('')}
                    </div>
                </div>`;
            });
            h += '</div>';
        }
        c.innerHTML = h;
    }

    return {
        render() { return '<div id="shared-lists-content">' + Comp.pageLoader() + '</div>'; },
        init() {
            load();

            const container = document.getElementById('shared-lists-content');
            container?.addEventListener('click', e => {
                const btn = e.target.closest('button');
                if (!btn) return;

                if (btn.id === 'new-list-btn') {
                    modal.open('Create Shared List', `<div class="space-y-4">
                        <div class="form-group"><label class="form-label">List Name</label><input id="list-name" class="input-field"></div>
                        <div class="form-group"><label class="form-label">Description</label><textarea id="list-desc" class="input-field" rows="2"></textarea></div>
                        <label class="flex items-center gap-2 cursor-pointer"><input type="checkbox" id="list-public" checked class="accent-vs-accent"><span class="text-sm text-vs-dim">Public (anyone with the link can view)</span></label>
                    </div>`,
                        `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                         <button class="btn btn-primary" onclick="pages.sharedLists.createList()">Create</button>`);
                }

                if (btn.classList.contains('share-list-btn')) {
                    const listId = btn.dataset.listId;
                    const list = state.lists.find(l => l.id == listId);
                    if (list && list.shareCode) {
                        // copy share code
                        navigator.clipboard.writeText(list.shareCode);
                        toast.info('Share code copied: ' + list.shareCode);
                    }
                }
                if (btn.classList.contains('delete-list-btn')) {
                    const listId = btn.dataset.listId;
                    api.del(`/profiles/me/lists/${listId}`).then(() => { toast.success('List deleted'); load(); }).catch(err => toast.error(err.message));
                }
                if (btn.classList.contains('edit-list-btn')) {
                    toast.info('Edit list - feature coming soon');
                }
            });
        },
        createList: async function () {
            const name = document.getElementById('list-name').value.trim();
            if (!name) { toast.warning('Enter a name'); return; }
            const desc = document.getElementById('list-desc').value.trim();
            const isPublic = document.getElementById('list-public').checked;
            try {
                await api.post('/profiles/me/lists', { name, description: desc, isPublic });
                modal.close();
                toast.success('List created');
                load();
            } catch (err) { toast.error(err.message); }
        }
    };
})();