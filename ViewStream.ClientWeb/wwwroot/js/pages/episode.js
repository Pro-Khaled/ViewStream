pages.episode = (() => {
    let state = { episode: null, comments: [], loading: true };

    async function load(id) {
        state.loading = true; render();
        try {
            const [ep, comments] = await Promise.all([
                api.get(`/episodes/${id}`),
                api.get(`/episodes/${id}/comments`)
            ]);
            state.episode = ep; state.comments = comments;
        } catch (err) { toast.error('Failed to load episode'); }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('episode-content');
        if (!c) return;
        if (state.loading) { c.innerHTML = Comp.pageLoader(); return; }
        if (!state.episode) { c.innerHTML = Comp.emptyState('fa-film', 'Episode not found'); return; }
        const ep = state.episode;

        let h = `<div class="px-4 sm:px-6 pt-6 max-w-6xl mx-auto">
            <div class="relative aspect-video bg-black rounded-2xl overflow-hidden mb-6 group cursor-pointer" id="video-player">
                <img src="${toast.esc(ep.thumbnailUrl)}" alt="" class="w-full h-full object-cover">
                <div class="absolute inset-0 bg-black/30 flex items-center justify-center group-hover:bg-black/40 transition-colors" id="player-overlay">
                    <div class="w-16 h-16 rounded-full bg-vs-accent/90 flex items-center justify-center shadow-lg group-hover:scale-110 transition-transform">
                        <i class="fas fa-play text-vs-bg text-xl ml-1" id="play-icon"></i>
                    </div>
                </div>
                <div class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/80 to-transparent p-4 opacity-0 group-hover:opacity-100 transition-opacity">
                    <div class="h-1 bg-white/20 rounded-full mb-3 cursor-pointer"><div class="h-full bg-vs-accent rounded-full progress-bar" style="width:35%"></div></div>
                    <div class="flex items-center justify-between">
                        <div class="flex items-center gap-3">
                            <button id="ctrl-play" class="text-white hover:text-vs-accent"><i class="fas fa-play text-lg" id="ctrl-play-icon"></i></button>
                            <button class="text-white hover:text-vs-accent"><i class="fas fa-forward text-lg"></i></button>
                            <button class="text-white hover:text-vs-accent"><i class="fas fa-volume-up text-lg"></i></button>
                            <span class="text-xs text-white/60">15:42 / ${ep.runtimeSeconds ? Math.floor(ep.runtimeSeconds / 60) + ':' + String(ep.runtimeSeconds % 60).padStart(2, '0') : '??'}</span>
                        </div>
                        <div class="flex items-center gap-3">
                            <select class="bg-transparent text-xs text-white border border-white/20 rounded px-2 py-1"><option>1080p</option></select>
                            <button class="text-white hover:text-vs-accent"><i class="fas fa-closed-captioning text-lg"></i></button>
                            <button class="text-white hover:text-vs-accent"><i class="fas fa-expand text-lg"></i></button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Episode info -->
            <div class="flex flex-col lg:flex-row gap-8 mb-10">
                <div class="flex-1">
                    <div class="flex items-center gap-3 mb-2">
                        <a href="#/shows/${Math.floor(ep.id / 1000)}" class="text-sm text-vs-accent hover:underline">${toast.esc(ep.showTitle)}</a>
                        <span class="text-vs-muted">&middot;</span>
                        <span class="text-sm text-vs-muted">Season ${ep.seasonNumber} &middot; Episode ${ep.episodeNumber}</span>
                    </div>
                    <h1 class="font-display font-bold text-2xl text-vs-text mb-3">${toast.esc(ep.title)}</h1>
                    <p class="text-vs-dim leading-relaxed mb-4">${toast.esc(ep.description)}</p>
                </div>
                <div class="w-full lg:w-64 flex-shrink-0">
                    <div class="rounded-xl bg-vs-card border border-vs-border p-4 space-y-3">
                        <h3 class="font-display font-semibold text-sm text-vs-text">Audio</h3>
                        <div class="flex flex-col gap-1.5">
                            <label><input type="radio" name="audio" checked class="accent-vs-accent"> English</label>
                            <label><input type="radio" name="audio" class="accent-vs-accent"> Japanese</label>
                        </div>
                        <h3 class="font-display font-semibold text-sm text-vs-text pt-2">Subtitles</h3>
                        <div class="flex flex-col gap-1.5">
                            <label><input type="checkbox" class="accent-vs-accent"> English [CC]</label>
                            <label><input type="checkbox" checked class="accent-vs-accent"> Spanish</label>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Comments -->
            <section class="mt-10 pb-10">
                <h2 class="font-display font-bold text-xl text-vs-text mb-6">Comments (${state.comments.length})</h2>
                ${store.isAuthenticated() ? `<div class="flex gap-3 mb-8">
                    <div class="w-10 h-10 rounded-full bg-vs-accent/20 flex items-center justify-center text-lg flex-shrink-0">${store.getUser()?.fullName?.charAt(0) || '👤'}</div>
                    <div class="flex-1">
                        <textarea id="new-comment" rows="3" placeholder="Share your thoughts..." class="w-full bg-vs-card border border-vs-border rounded-xl px-4 py-3 text-sm text-vs-text placeholder-vs-muted resize-none focus:outline-none focus:border-vs-accent/50"></textarea>
                        <div class="flex justify-end mt-2">
                            <button id="post-comment-btn" class="px-4 py-2 bg-vs-accent hover:bg-vs-accentHover text-vs-bg text-sm font-semibold rounded-lg">Post Comment</button>
                        </div>
                    </div>
                </div>` : `<div class="p-4 rounded-xl bg-vs-card border border-vs-border mb-6 text-center"><a href="#/login" class="text-sm text-vs-accent hover:underline">Sign in</a> to comment</div>`}
                <div class="space-y-4" id="comments-list">
                    ${state.comments.map(c => renderComment(c)).join('')}
                </div>
            </section>
        </div>`;
        c.innerHTML = h;
    }

    function renderComment(comment, depth = 0) {
        const timeAgo = d => {
            const diff = Date.now() - new Date(d).getTime();
            const mins = Math.floor(diff / 60000);
            if (mins < 60) return `${mins}m ago`;
            const hrs = Math.floor(mins / 60);
            if (hrs < 24) return `${hrs}h ago`;
            return `${Math.floor(hrs / 24)}d ago`;
        };
        const ml = depth > 0 ? `ml-${Math.min(depth * 8, 48)}` : '';
        return `<div class="${ml} ${depth > 0 ? 'border-l-2 border-vs-border pl-4' : ''}">
            <div class="flex gap-3">
                <div class="w-9 h-9 rounded-full bg-vs-card flex items-center justify-center text-base flex-shrink-0">${toast.esc(comment.profileAvatar || comment.profileName?.charAt(0) || '👤')}</div>
                <div class="flex-1 min-w-0">
                    <div class="flex items-center gap-2 mb-1">
                        <span class="text-sm font-semibold text-vs-text">${toast.esc(comment.profileName)}</span>
                        ${comment.isEdited ? '<span class="text-xs text-vs-muted">(edited)</span>' : ''}
                        <span class="text-xs text-vs-muted">${timeAgo(comment.createdAt)}</span>
                    </div>
                    <p class="text-sm text-vs-dim leading-relaxed mb-2">${toast.esc(comment.commentText)}</p>
                    <div class="flex items-center gap-4">
                        <button class="flex items-center gap-1.5 text-xs text-vs-muted hover:text-vs-accent like-btn" data-comment-id="${comment.id}"><i class="far fa-thumbs-up"></i> ${comment.likeCount || 0}</button>
                        ${depth < 2 ? `<button class="text-xs text-vs-muted hover:text-vs-text reply-btn" data-comment-id="${comment.id}">Reply</button>` : ''}
                        ${store.isAuthenticated() ? `<button class="text-xs text-vs-muted hover:text-vs-error report-btn" data-comment-id="${comment.id}">Report</button>` : ''}
                    </div>
                </div>
            </div>
            ${comment.replies?.length ? comment.replies.map(r => renderComment(r, depth + 1)).join('') : ''}
        </div>`;
    }

    return {
        render() { return '<div id="episode-content">' + Comp.pageLoader() + '</div>'; },
        init(params) {
            load(params.id);
            // Post comment
            document.getElementById('post-comment-btn')?.addEventListener('click', async () => {
                const text = document.getElementById('new-comment').value.trim();
                if (!text) { toast.warning('Please write a comment'); return; }
                try {
                    await api.post(`/episodes/${state.episode.id}/comments`, { commentText: text });
                    toast.success('Comment posted');
                    load(state.episode.id); // refresh
                } catch (err) { toast.error(err.message); }
            });
            // Like / reply / report
            document.getElementById('comments-list')?.addEventListener('click', e => {
                const btn = e.target.closest('button');
                if (!btn) return;
                const commentId = btn.dataset.commentId;
                if (btn.classList.contains('like-btn')) {
                    api.post(`/comments/${commentId}/likes`, { commentId, reactionType: 'like' }).then(() => {
                        toast.success('Reaction recorded');
                        load(state.episode.id);
                    }).catch(err => toast.error(err.message));
                } else if (btn.classList.contains('reply-btn')) {
                    document.getElementById('new-comment').focus();
                    document.getElementById('new-comment').placeholder = `Replying to comment #${commentId}...`;
                } else if (btn.classList.contains('report-btn')) {
                    // simple modal for reason
                    modal.open('Report Comment', `<textarea id="report-reason" class="input-field" placeholder="Reason..."></textarea>`,
                        `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                         <button class="btn btn-danger" onclick="pages.episode.submitReport(${commentId})">Submit</button>`);
                }
            });
        },
        submitReport: async function (commentId) {
            const reason = document.getElementById('report-reason')?.value?.trim() || 'Other';
            try {
                await api.post(`/comments/${commentId}/reports`, { commentId, reason });
                modal.close();
                toast.success('Report submitted');
            } catch (err) { toast.error(err.message); }
        }
    };
})();