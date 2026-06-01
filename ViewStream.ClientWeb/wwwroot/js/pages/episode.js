pages.episode = (() => {
    let state = { episodeId: null, episode: null, streamUrl: null, audioTracks: [], subtitles: [], comments: null, commentsPage: 1, loading: true };

    async function load(id) {
        state.episodeId = id;
        state.loading = true; render();
        try {
            const [ep, streamUrlDto, audioTracks, subtitles] = await Promise.all([
                api.get(`/episodes/${id}`),
                api.get(`/episodes/${id}/stream`),
                api.get(`/episodes/${id}/audio-tracks`),
                api.get(`/episodes/${id}/subtitles`)
            ]);
            state.episode = ep;
            state.streamUrl = streamUrlDto ? streamUrlDto.videoUrl : ep.videoUrl;
            state.audioTracks = audioTracks || [];
            state.subtitles = subtitles || [];
            
            await loadComments(1);
        } catch (err) {
            toast.error('Failed to load episode: ' + err.message);
        }
        state.loading = false; render();
    }

    async function loadComments(page = 1) {
        try {
            const commentsRes = await api.get(`/episodes/${state.episodeId}/comments`, {
                page: page,
                pageSize: 10
            });
            if (page === 1) {
                state.comments = commentsRes;
            } else {
                state.comments.items = [...state.comments.items, ...(commentsRes.items || [])];
                state.comments.pageNumber = commentsRes.pageNumber;
                state.comments.totalPages = commentsRes.totalPages;
            }
            state.commentsPage = page;
        } catch (e) {
            console.error('Failed to load comments:', e);
        }
    }

    function render() {
        const c = document.getElementById('episode-content');
        if (!c) return;
        if (state.loading) { c.innerHTML = Comp.pageLoader(); return; }
        if (!state.episode) { c.innerHTML = Comp.emptyState('fa-film', 'Episode not found'); return; }
        const ep = state.episode;

        let h = `<div class="px-4 sm:px-6 pt-6 max-w-6xl mx-auto">
            <div class="relative aspect-video bg-black rounded-2xl overflow-hidden mb-6" id="video-container">
                <video id="main-video-player" class="w-full h-full" controls poster="${toast.esc(ep.thumbnailUrl)}">
                    <source src="${toast.esc(state.streamUrl)}" type="video/mp4">
                    Your browser does not support the video tag.
                </video>
            </div>

            <!-- Episode info -->
            <div class="flex flex-col lg:flex-row gap-8 mb-10">
                <div class="flex-1">
                    <div class="flex items-center gap-3 mb-2">
                        <a href="#/shows/${Math.floor(ep.id / 1000)}" class="text-sm text-vs-accent hover:underline font-semibold">${toast.esc(ep.showTitle)}</a>
                        <span class="text-vs-muted">&middot;</span>
                        <span class="text-sm text-vs-muted">Season ${ep.seasonNumber} &middot; Episode ${ep.episodeNumber}</span>
                    </div>
                    <h1 class="font-display font-black text-2xl sm:text-3xl text-vs-text mb-3">${toast.esc(ep.title)}</h1>
                    <p class="text-vs-dim leading-relaxed mb-4">${toast.esc(ep.description || 'No description available.')}</p>
                </div>
                
                <div class="w-full lg:w-64 flex-shrink-0">
                    <div class="rounded-xl bg-vs-card border border-vs-border p-4 space-y-4 font-body">
                        <div>
                            <h3 class="font-display font-bold text-sm text-vs-text mb-2">Audio Tracks</h3>
                            <div class="flex flex-col gap-2" id="audio-tracks-list">
                                ${state.audioTracks.length === 0 ? 
                                    `<div class="text-xs text-vs-muted">Original English (embedded)</div>` : 
                                    state.audioTracks.map(t => `<label class="flex items-center gap-2 text-sm text-vs-dim hover:text-vs-text cursor-pointer select-none">
                                        <input type="radio" name="audio-track" value="${toast.esc(t.audioUrl || '')}" ${t.isDefault ? 'checked' : ''} class="accent-vs-accent audio-track-radio">
                                        <span>${toast.esc(t.languageCode.toUpperCase())} ${t.trackType ? `(${toast.esc(t.trackType)})` : ''}</span>
                                    </label>`).join('')
                                }
                            </div>
                        </div>
                        <hr class="border-vs-border">
                        <div>
                            <h3 class="font-display font-bold text-sm text-vs-text mb-2">Subtitles</h3>
                            <div class="flex flex-col gap-2" id="subtitles-list">
                                <label class="flex items-center gap-2 text-sm text-vs-dim hover:text-vs-text cursor-pointer select-none">
                                    <input type="radio" name="subtitle-track" value="" checked class="accent-vs-accent subtitle-track-radio">
                                    <span>None</span>
                                </label>
                                ${state.subtitles.map(s => `<label class="flex items-center gap-2 text-sm text-vs-dim hover:text-vs-text cursor-pointer select-none">
                                    <input type="radio" name="subtitle-track" value="${toast.esc(s.subtitleUrl || '')}" data-lang="${toast.esc(s.languageCode)}" class="accent-vs-accent subtitle-track-radio">
                                    <span>${toast.esc(s.languageCode.toUpperCase())} ${s.label ? `(${toast.esc(s.label)})` : ''}</span>
                                </label>`).join('')}
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Comments -->
            <section class="mt-10 pb-10">
                <h2 class="font-display font-black text-xl text-vs-text mb-6">Comments (${state.comments?.totalCount || 0})</h2>
                ${store.isAuthenticated() ? `<div class="flex gap-3 mb-8 font-body">
                    <div class="w-10 h-10 rounded-full bg-vs-accent/20 flex items-center justify-center text-lg font-bold flex-shrink-0 border border-vs-border">${store.getUser()?.fullName?.charAt(0) || 'ًں‘¤'}</div>
                    <div class="flex-1">
                        <textarea id="new-comment" rows="3" placeholder="Share your thoughts..." class="w-full bg-vs-card border border-vs-border rounded-xl px-4 py-3 text-sm text-vs-text placeholder-vs-muted resize-none focus:outline-none focus:border-vs-accent/50"></textarea>
                        <div class="flex justify-end mt-2">
                            <button id="post-comment-btn" class="px-4 py-2 bg-vs-accent hover:bg-vs-accentHover text-vs-bg text-sm font-semibold rounded-lg">Post Comment</button>
                        </div>
                    </div>
                </div>` : `<div class="p-4 rounded-xl bg-vs-card border border-vs-border mb-6 text-center"><a href="#/login" class="text-sm text-vs-accent hover:underline font-semibold">Sign in</a> to comment</div>`}
                
                <div class="space-y-4" id="comments-list">
                    ${state.comments?.items?.length ? 
                        state.comments.items.map(c => renderComment(c)).join('') : 
                        `<div class="text-center text-vs-muted py-8 text-sm">No comments yet. Be the first to share your thoughts!</div>`
                    }
                </div>

                ${state.comments && state.comments.pageNumber < state.comments.totalPages ? 
                    `<div class="flex justify-center mt-6">
                        <button id="load-more-comments-btn" class="btn btn-ghost btn-sm text-vs-accent"><i class="fas fa-chevron-down mr-1.5"></i> Load More Comments</button>
                    </div>` : ''
                }
            </section>
        </div>`;
        c.innerHTML = h;
        bindEvents();
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
        const ml = depth > 0 ? `ml-${Math.min(depth * 8, 24)}` : '';
        
        let repliesHtml = '';
        if ((comment.replyCount > 0 || comment.ReplyCount > 0) && depth === 0) {
            const count = comment.replyCount || comment.ReplyCount;
            repliesHtml = `<div class="mt-2 ml-12" id="replies-container-${comment.id}">
                <button class="text-xs text-vs-accent hover:underline flex items-center gap-1.5 load-replies-btn" data-comment-id="${comment.id}" data-depth="${depth}">
                    <i class="fas fa-comments"></i> View ${count} ${count === 1 ? 'reply' : 'replies'}
                </button>
            </div>`;
        }

        return `<div class="${ml} ${depth > 0 ? 'border-l-2 border-vs-border pl-4' : ''} mb-4 font-body comment-item" id="comment-item-${comment.id}" data-comment-id="${comment.id}" data-depth="${depth}">
            <div class="flex gap-3">
                <div class="w-9 h-9 rounded-full bg-vs-card flex items-center justify-center text-base flex-shrink-0 font-bold border border-vs-border">${toast.esc(comment.profileAvatar || comment.profileName?.charAt(0) || 'ًں‘¤')}</div>
                <div class="flex-1 min-w-0">
                    <div class="flex items-center gap-2 mb-1">
                        <span class="text-sm font-semibold text-vs-text">${toast.esc(comment.profileName)}</span>
                        ${comment.isEdited || comment.IsEdited ? '<span class="text-xs text-vs-muted">(edited)</span>' : ''}
                        <span class="text-xs text-vs-muted">${timeAgo(comment.createdAt || comment.CreatedAt)}</span>
                    </div>
                    <p class="text-sm text-vs-dim leading-relaxed mb-2">${toast.esc(comment.commentText || comment.CommentText)}</p>
                    <div class="flex items-center gap-4">
                        <button class="flex items-center gap-1.5 text-xs text-vs-muted hover:text-vs-accent like-btn" data-comment-id="${comment.id}"><i class="far fa-thumbs-up"></i> <span class="like-count">${comment.likeCount || comment.LikeCount || 0}</span></button>
                        ${depth < 2 ? `<button class="text-xs text-vs-muted hover:text-vs-text reply-btn" data-comment-id="${comment.id}">Reply</button>` : ''}
                        ${store.isAuthenticated() ? `<button class="text-xs text-vs-muted hover:text-vs-error report-btn" data-comment-id="${comment.id}">Report</button>` : ''}
                    </div>
                </div>
            </div>
            ${repliesHtml}
        </div>`;
    }

    function bindEvents() {
        let replyToCommentId = null;

        // Video Player Error Handling
        const videoPlayer = document.getElementById('main-video-player');
        if (videoPlayer) {
            videoPlayer.addEventListener('error', () => {
                const mediaError = videoPlayer.error;
                let msg = 'An error occurred while loading the video stream.';
                if (mediaError) {
                    switch (mediaError.code) {
                        case 1: msg = 'Video loading aborted by user.'; break;
                        case 2: msg = 'Network error while downloading video.'; break;
                        case 3: msg = 'Video decoding failed. The format may not be supported.'; break;
                        case 4: msg = 'Video stream or audio track url not found (404).'; break;
                    }
                }
                toast.error(msg);
                
                // Show visual overlay inside player
                const container = document.getElementById('video-container');
                if (container) {
                    document.getElementById('video-error-overlay')?.remove();
                    const overlay = document.createElement('div');
                    overlay.id = 'video-error-overlay';
                    overlay.className = 'absolute inset-0 bg-black/90 flex flex-col items-center justify-center text-center p-6 space-y-4 font-body';
                    overlay.innerHTML = `
                        <div class="w-12 h-12 rounded-full bg-vs-danger/10 flex items-center justify-center text-vs-danger text-xl">
                            <i class="fas fa-exclamation-triangle"></i>
                        </div>
                        <div>
                            <h4 class="font-bold text-vs-text text-base">Playback Error</h4>
                            <p class="text-xs text-vs-muted mt-1 max-w-xs">${toast.esc(msg)}</p>
                        </div>
                        <button class="btn btn-primary btn-xs px-4" id="video-retry-btn">Retry Playback</button>
                    `;
                    container.appendChild(overlay);
                    document.getElementById('video-retry-btn')?.addEventListener('click', () => {
                        overlay.remove();
                        videoPlayer.load();
                        videoPlayer.play().catch(() => {});
                    });
                }
            });
        }

        // Audio Track Change
        document.querySelectorAll('.audio-track-radio').forEach(radio => {
            radio.addEventListener('change', (e) => {
                const trackUrl = e.target.value;
                const video = document.getElementById('main-video-player');
                if (!video || !trackUrl) return;
                const currentTime = video.currentTime;
                const isPaused = video.paused;
                
                video.src = trackUrl;
                video.load();
                video.currentTime = currentTime;
                if (!isPaused) {
                    video.play().catch(err => console.log(err));
                }
                toast.success('Audio track changed!');
            });
        });

        // Subtitle Track Change
        document.querySelectorAll('.subtitle-track-radio').forEach(radio => {
            radio.addEventListener('change', (e) => {
                const subUrl = e.target.value;
                const lang = e.target.dataset.lang;
                const video = document.getElementById('main-video-player');
                if (!video) return;

                const existingTracks = video.querySelectorAll('track');
                existingTracks.forEach(t => t.remove());

                if (subUrl) {
                    const track = document.createElement('track');
                    track.kind = 'subtitles';
                    track.label = lang.toUpperCase();
                    track.srclang = lang;
                    track.src = subUrl;
                    track.default = true;
                    video.appendChild(track);
                    toast.success(`Subtitles changed to ${lang.toUpperCase()}!`);
                } else {
                    toast.success('Subtitles turned off');
                }
            });
        });

        // Post comment
        document.getElementById('post-comment-btn')?.addEventListener('click', async () => {
            const text = document.getElementById('new-comment').value.trim();
            if (!text) { toast.warning('Please write a comment'); return; }
            try {
                await api.post(`/episodes/${state.episode.id}/comments`, {
                    episodeId: state.episode.id,
                    commentText: text,
                    parentCommentId: replyToCommentId || null
                });
                toast.success(replyToCommentId ? 'Reply posted!' : 'Comment posted!');
                
                // Reset replies helper
                replyToCommentId = null;
                document.getElementById('new-comment').placeholder = 'Share your thoughts...';
                document.getElementById('reply-badge')?.remove();
                document.getElementById('new-comment').value = '';
                
                // Reload first page of comments
                await loadComments(1);
                render();
            } catch (err) {
                toast.error(err.message);
            }
        });

        // Load More Comments
        document.getElementById('load-more-comments-btn')?.addEventListener('click', async () => {
            const nextPage = state.commentsPage + 1;
            await loadComments(nextPage);
            render();
        });

        // Like / reply / report click handler delegation
        document.getElementById('comments-list')?.addEventListener('click', async (e) => {
            const btn = e.target.closest('button');
            if (!btn) return;
            const commentId = btn.dataset.commentId;
            const depth = parseInt(btn.dataset.depth || '0');

            if (btn.classList.contains('like-btn')) {
                try {
                    await api.post(`/comments/${commentId}/likes`, { commentId: parseInt(commentId), reactionType: 'like' });
                    toast.success('Liked comment!');
                    // Increment client-side count for slick instant feedback
                    const countSpan = btn.querySelector('.like-count');
                    if (countSpan) {
                        countSpan.innerText = parseInt(countSpan.innerText) + 1;
                    }
                } catch (err) {
                    toast.error(err.message);
                }
            } else if (btn.classList.contains('reply-btn')) {
                replyToCommentId = parseInt(commentId);
                const commentArea = document.getElementById('new-comment');
                if (commentArea) {
                    commentArea.placeholder = 'Write your reply...';
                    commentArea.focus();
                    
                    document.getElementById('reply-badge')?.remove();
                    const badge = document.createElement('div');
                    badge.id = 'reply-badge';
                    badge.className = 'flex items-center gap-2 mb-2 text-xs text-vs-accent font-semibold';
                    badge.innerHTML = `<i class="fas fa-reply"></i> Replying to comment #${commentId} <button class="ml-1 text-vs-muted hover:text-vs-error" id="cancel-reply-btn"><i class="fas fa-times-circle"></i></button>`;
                    commentArea.parentElement.insertBefore(badge, commentArea);
                    
                    document.getElementById('cancel-reply-btn')?.addEventListener('click', () => {
                        replyToCommentId = null;
                        commentArea.placeholder = 'Share your thoughts...';
                        badge.remove();
                    });
                }
            } else if (btn.classList.contains('report-btn')) {
                modal.open('Report Comment', 
                    `<div class="space-y-4">
                        <p class="text-sm text-vs-dim">Let us know why you are reporting this comment.</p>
                        <textarea id="report-reason" class="input-field h-24" placeholder="e.g., Harassment, spoilers, spam..."></textarea>
                    </div>`,
                    `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
                     <button class="btn btn-danger" id="submit-comment-report">Submit Report</button>`
                );
                document.getElementById('submit-comment-report')?.addEventListener('click', async () => {
                    const reason = document.getElementById('report-reason').value.trim();
                    if (!reason) { toast.warning('Reason is required'); return; }
                    try {
                        await api.post(`/comments/${commentId}/reports`, { commentId: parseInt(commentId), reason });
                        modal.close();
                        toast.success('Report submitted successfully');
                    } catch (err) {
                        toast.error(err.message);
                    }
                });
            } else if (btn.classList.contains('load-replies-btn')) {
                const parentId = parseInt(commentId);
                const container = document.getElementById(`replies-container-${parentId}`);
                if (container) {
                    container.innerHTML = '<span class="text-xs text-vs-muted"><i class="fas fa-spinner fa-spin mr-1.5"></i>Loading replies...</span>';
                    try {
                        const replies = await api.get(`/episodes/${state.episodeId}/comments/replies/${parentId}`);
                        if (!replies || replies.length === 0) {
                            container.remove();
                            return;
                        }
                        container.innerHTML = replies.map(r => renderComment(r, depth + 1)).join('');
                    } catch (err) {
                        container.innerHTML = `<span class="text-xs text-vs-error">Failed to load replies: ${err.message}</span>`;
                    }
                }
            }
        });
    }

    return {
        render() { return '<div id="episode-content">' + Comp.pageLoader() + '</div>'; },
        init(params) {
            load(params.id);
        }
    };
})();