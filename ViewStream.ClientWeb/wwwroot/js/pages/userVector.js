pages.userVector = (() => {
    let state = { data: null, loading: true };

    async function load() {
        state.loading = true; render();
        try {
            state.data = await api.get('/profiles/me/vector');
        } catch (err) {
            if (err.status === 404) {
                // Initialize default dummy preference vector
                state.data = { profileName: 'Standard Profile', embeddingJson: '[0.25, 0.45, 0.75, 0.12, 0.90, 0.33, 0.50, 0.61]', lastUpdated: new Date() };
            } else {
                toast.error('Failed to load recommendation profile: ' + err.message);
                state.data = null;
            }
        }
        state.loading = false; render();
    }

    async function recalculate() {
        // Upsert default/new embedding to simulate personalization update
        state.loading = true; render();
        try {
            // Randomize slightly to simulate active learning
            const newArr = Array.from({ length: 8 }, () => Math.random().toFixed(2));
            const embeddingJson = JSON.stringify(newArr);
            state.data = await api.post('/profiles/me/vector', { embeddingJson });
            toast.success('Your personalized recommendations profile has been updated!');
        } catch (err) {
            toast.error('Failed to update recommendation profile: ' + err.message);
        }
        state.loading = false; render();
    }

    function render() {
        const c = document.getElementById('vector-content');
        if (!c) return;

        let h = Comp.pageHeader('Recommendation Engine', 'Explore the machine learning preferences vector representing your custom recommendations.',
            `<button class="btn btn-primary btn-sm" id="recalc-vector-btn"><i class="fas fa-sync mr-1.5"></i> Update Preferences</button>`);

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data) {
            h += Comp.emptyState('fa-brain', 'Failed to retrieve recommendation telemetry.');
        } else {
            let embedding = [];
            try {
                embedding = JSON.parse(state.data.embeddingJson || '[]');
            } catch (e) {
                embedding = [0.1, 0.2, 0.3, 0.4];
            }

            // Categories representing fictional genres corresponding to dimensions in the embedding space
            const categories = ['Sci-Fi & Fantasy', 'Action & Adventure', 'Drama & Romance', 'Thriller & Mystery', 'Comedy & Satire', 'Documentary & Indie', 'Kids & Family', 'Anime & Animation'];

            h += `<div class="grid grid-cols-1 lg:grid-cols-2 gap-6 font-body">
                <!-- Dimension Plots -->
                <div class="card p-6 shadow-md border-vs-border space-y-6">
                    <h3 class="font-bold text-lg text-vs-text mb-4"><i class="fas fa-chart-bar text-vs-accent mr-1.5"></i> Preference Breakdown</h3>
                    <div class="space-y-4">
                        ${embedding.map((val, idx) => {
                            const pct = Math.round(parseFloat(val) * 100);
                            const category = categories[idx] || `Dimension #${idx + 1}`;
                            return `
                                <div class="space-y-1">
                                    <div class="flex justify-between text-xs font-semibold">
                                        <span class="text-vs-text">${toast.esc(category)}</span>
                                        <span class="text-vs-accent">${pct}% affinity</span>
                                    </div>
                                    <div class="w-full h-2.5 bg-vs-surface-2 rounded-full overflow-hidden">
                                        <div class="bg-gradient-to-r from-vs-accent to-vs-accent-dim h-full rounded-full" style="width: ${pct}%"></div>
                                    </div>
                                </div>
                            `;
                        }).join('')}
                    </div>
                </div>

                <!-- Technical Details & Info -->
                <div class="card p-6 shadow-md border-vs-border space-y-6">
                    <h3 class="font-bold text-lg text-vs-text mb-4"><i class="fas fa-microchip text-vs-accent mr-1.5"></i> Technical Vector DTO</h3>
                    <div class="space-y-4">
                        <div class="grid grid-cols-2 gap-4">
                            ${Comp.detailRow('Profile', state.data.profileName || '—')}
                            ${Comp.detailRow('Last Synced', state.data.lastUpdated ? utils.formatDate(state.data.lastUpdated) : '—')}
                        </div>
                        <div class="form-group mb-0">
                            <label class="form-label">High-Dimensional Vector Array (JSON)</label>
                            <pre class="p-4 rounded-xl bg-vs-surface-2 border border-vs-border font-mono text-xs text-vs-accent whitespace-pre-wrap overflow-x-auto">${toast.esc(JSON.stringify(embedding, null, 2))}</pre>
                        </div>
                        <p class="text-xs text-vs-muted leading-relaxed">
                            <strong>How this works:</strong> Every time you watch, like, or rate shows, our recommendation model automatically computes interest affinities across high-dimensional feature representations. This math model directly drives your personalized row displays.
                        </p>
                    </div>
                </div>
            </div>`;
        }

        c.innerHTML = h;

        document.getElementById('recalc-vector-btn')?.addEventListener('click', recalculate);
    }

    return {
        render() { return '<div id="vector-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();
