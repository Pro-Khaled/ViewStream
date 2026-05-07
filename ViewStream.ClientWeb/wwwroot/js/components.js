const Comp = (() => {
    function pageLoader() { return '<div class="flex items-center justify-center py-20"><div class="spinner"></div></div>'; }
    function emptyState(icon, msg) { return `<div class="empty-state"><i class="fas ${icon}"></i><p>${toast.esc(msg)}</p></div>`; }
    function statCard(value, label, icon, color = 'text-vs-accent') {
        return `<div class="bg-vs-surface border border-vs-border rounded-xl p-5">
            <div class="flex items-center justify-between mb-2"><span class="text-sm text-vs-muted">${toast.esc(label)}</span><i class="fas ${icon} ${color} opacity-60"></i></div>
            <div class="text-2xl font-display font-black text-vs-text">${toast.esc(String(value))}</div>
        </div>`;
    }
    function pagination(page, totalPages, onChange) {
        if (totalPages <= 1) return '';
        const id = 'pg-' + Math.random().toString(36).slice(2, 8);
        let btns = '';
        btns += `<button class="page-btn" data-pg="${id}" data-page="${page - 1}" ${page <= 1 ? 'disabled' : ''}><i class="fas fa-chevron-left text-xs"></i></button>`;
        const pages = computePages(page, totalPages, 7);
        pages.forEach(p => {
            if (p === '...') btns += '<span class="px-1 text-vs-muted text-sm">…</span>';
            else btns += `<button class="page-btn${p === page ? ' active' : ''}" data-pg="${id}" data-page="${p}">${p}</button>`;
        });
        btns += `<button class="page-btn" data-pg="${id}" data-page="${page + 1}" ${page >= totalPages ? 'disabled' : ''}><i class="fas fa-chevron-right text-xs"></i></button>`;
        requestAnimationFrame(() => {
            document.querySelectorAll(`[data-pg="${id}"]`).forEach(b => {
                b.addEventListener('click', () => { const p = parseInt(b.dataset.page); if (p >= 1 && p <= totalPages && p !== page) onChange(p); });
            });
        });
        return `<div class="flex items-center justify-between mt-6"><span class="text-sm text-vs-muted">Page ${page} of ${totalPages}</span><div class="pagination">${btns}</div></div>`;
    }
    function computePages(cur, total, max) {
        if (total <= max) return Array.from({ length: total }, (_, i) => i + 1);
        const out = []; const half = Math.floor((max - 2) / 2);
        let s = Math.max(2, cur - half), e = Math.min(total - 1, cur + half);
        if (cur - half < 2) e = Math.min(total - 1, max - 1);
        if (cur + half > total - 1) s = Math.max(2, total - max + 2);
        out.push(1); if (s > 2) out.push('...'); for (let i = s; i <= e; i++) out.push(i); if (e < total - 1) out.push('...'); out.push(total); return out;
    }
    function pageHeader(title, sub, actions) {
        return `<div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6"><div><h1 class="font-display font-bold text-2xl text-vs-text">${toast.esc(title)}</h1>${sub ? `<p class="text-vs-muted text-sm mt-1">${toast.esc(sub)}</p>` : ''}</div>${actions ? `<div class="flex items-center gap-3">${actions}</div>` : ''}</div>`;
    }
    function filterBar(filters, onApply) { /* same as Frontend B, using vs- classes */ }
    function detailRow(label, value) {
        return `<div class="detail-item"><div class="detail-label">${toast.esc(label)}</div><div class="detail-value">${value || '—'}</div></div>`;
    }
    // Additional components from Frontend A
    function showCard(show, size = 'normal') {
        const w = size === 'large' ? 'w-52 sm:w-60' : 'w-36 sm:w-40 lg:w-44';
        return `<a href="#/shows/${show.id}" class="show-card ${w} block rounded-xl overflow-hidden bg-vs-card cursor-pointer" aria-label="${toast.esc(show.title)}">
            <div class="relative aspect-[2/3]">
                <img src="${toast.esc(show.posterUrl)}" alt="${toast.esc(show.title)}" class="w-full h-full object-cover" loading="lazy">
                <div class="card-overlay absolute inset-0 bg-gradient-to-t from-black/90 via-black/30 to-transparent flex flex-col justify-end p-3">
                    <div class="flex items-center gap-2 mb-1">
                        ${show.imdbRating ? `<span class="flex items-center gap-1 text-xs font-semibold text-vs-accent"><i class="fas fa-star text-[10px]"></i>${show.imdbRating}</span>` : ''}
                        <span class="text-[10px] px-1.5 py-0.5 rounded bg-white/10 text-white/80">${toast.esc(show.maturityRating || '')}</span>
                    </div>
                    <p class="text-xs text-white/70 line-clamp-2">${toast.esc(show.description || '')}</p>
                </div>
            </div>
            <div class="p-2">
                <h3 class="text-sm font-medium text-vs-text truncate">${toast.esc(show.title)}</h3>
                <p class="text-xs text-vs-muted mt-0.5">${show.releaseYear || ''} &middot; ${(show.genres || []).slice(0, 2).join(', ') || ''}</p>
            </div>
        </a>`;
    }
    function showRow(title, shows, size = 'normal') {
        if (!shows?.length) return '';
        return `<section class="mb-8"><h2 class="font-display font-bold text-xl text-vs-text mb-4 px-1">${toast.esc(title)}</h2><div class="scroll-row px-1">${shows.map(s => showCard(s, size)).join('')}</div></section>`;
    }
    function skeletonCards(count = 6) {
        return `<div class="scroll-row px-1">${Array.from({ length: count }, () => `<div class="w-40 flex-shrink-0"><div class="skeleton aspect-[2/3] rounded-xl mb-2"></div><div class="skeleton h-4 w-3/4 mb-1"></div><div class="skeleton h-3 w-1/2"></div></div>`).join('')}</div>`;
    }
    function skeletonRows(count = 3) {
        return Array.from({ length: count }, (_, i) => `<section class="mb-8"><div class="skeleton h-7 w-48 mb-4 px-1"></div>${skeletonCards(6)}</section>`).join('');
    }
    function starRating(rating = 0, interactive = false, onChange = null) {
        let stars = '';
        for (let i = 1; i <= 5; i++) {
            const filled = i <= Math.round(rating);
            stars += `<i class="${filled ? 'fas filled' : 'far empty'} fa-star text-lg" ${interactive ? `onclick="(${onChange})(${i})"` : ''} role="button" tabindex="0" aria-label="Rate ${i}"></i>`;
        }
        return `<div class="star-rate flex items-center gap-1">${stars}</div>`;
    }
    function dataTable(headers, rows, emptyMsg = 'No data found', options = {}) {
        if (!rows?.length) {
            return `<div class="text-center py-16 text-vs-muted"><i class="fas fa-inbox text-3xl mb-3 block"></i><p>${emptyMsg}</p></div>`;
        }

        const { tableId, sortKey, sortDir, onSort } = options;

        const headersHtml = headers.map(col => {
            const key = col.key || '';
            let icon = '';
            if (key && sortKey === key) {
                icon = sortDir === 'asc'
                    ? '<i class="fas fa-sort-up ml-1"></i>'
                    : '<i class="fas fa-sort-down ml-1"></i>';
            } else if (key) {
                icon = '<i class="fas fa-sort ml-1 opacity-25"></i>';
            }
            const sortAttr = key ? `data-sort="${key}"` : '';
            const cursorClass = key ? 'cursor-pointer select-none' : '';
            return `<th class="px-4 py-3 text-left font-semibold text-vs-dim whitespace-nowrap ${cursorClass}" ${sortAttr}>
            <div class="flex items-center justify-between">${col.label}${icon}</div>
        </th>`;
        }).join('');

        const html = `
        <div class="overflow-x-auto rounded-xl border border-vs-border" id="${tableId || ''}">
            <table class="w-full text-sm">
                <thead><tr class="bg-vs-card border-b border-vs-border">${headersHtml}</tr></thead>
                <tbody>${rows}</tbody>
            </table>
        </div>`;

        // Attach click events after DOM insertion
        setTimeout(() => {
            if (tableId && onSort) {
                const tableEl = document.getElementById(tableId);
                if (tableEl) {
                    tableEl.querySelectorAll('th[data-sort]').forEach(th => {
                        th.addEventListener('click', () => {
                            const key = th.dataset.sort;
                            const newDir = (sortKey === key && sortDir === 'asc') ? 'desc' : 'asc';
                            onSort(key, newDir);
                        });
                    });
                }
            }
        }, 0);

        return html;
    }


    return { pageLoader, emptyState, statCard, pagination, pageHeader, filterBar, detailRow, showCard, showRow, skeletonCards, skeletonRows, starRating, dataTable };
})();