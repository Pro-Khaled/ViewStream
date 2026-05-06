const toast = (() => {
    function esc(s) { const d = document.createElement('div'); d.textContent = s; return d.innerHTML; }
    function show(msg, type = 'info', duration = 4000) {
        const c = document.getElementById('toast-container');
        const el = document.createElement('div');
        el.className = `toast toast-${type} toast-in`;
        el.innerHTML = `<i class="fas fa-${{ success: 'check-circle', error: 'times-circle', warning: 'exclamation-triangle', info: 'info-circle' }[type] || 'info-circle'}"></i><span>${esc(msg)}</span>`;
        c.appendChild(el);
        setTimeout(() => { el.classList.replace('toast-in', 'toast-out'); setTimeout(() => el.remove(), 300); }, duration);
    }
    return { show, success(m) { show(m, 'success') }, error(m) { show(m, 'error', 6000) }, warning(m) { show(m, 'warning') }, info(m) { show(m, 'info') }, esc };
})();

const modal = (() => {
    function open(title, bodyHtml, opts = {}) {
        const size = opts.large ? 'modal-lg' : '';
        document.getElementById('modal-container').innerHTML = `
            <div class="modal-overlay modal-bg" id="modal-overlay">
                <div class="modal-window ${size} modal-content">
                    <div class="flex items-center justify-between p-6 border-b border-vs-border">
                        <h3 class="font-display font-bold text-lg text-vs-text">${title}</h3>
                        <button class="btn-ghost btn-sm" onclick="modal.close()"><i class="fas fa-times"></i></button>
                    </div>
                    <div class="p-6">${bodyHtml}</div>
                    ${opts.footer ? `<div class="flex items-center justify-end gap-3 p-6 border-t border-vs-border">${opts.footer}</div>` : ''}
                </div>
            </div>`;
        document.getElementById('modal-overlay').addEventListener('click', function (e) { if (e.target === this) close(); });
    }
    function close() { document.getElementById('modal-container').innerHTML = ''; }
    return { open, close };
})();

const utils = (() => {
    function formatDate(iso) { if (!iso) return '—'; try { const d = new Date(iso); if (isNaN(d)) return iso; return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' }) + ', ' + d.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' }); } catch { return iso; } }
    function formatDateShort(iso) { if (!iso) return '—'; try { const d = new Date(iso); if (isNaN(d)) return iso; return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' }); } catch { return iso; } }
    function truncate(t, max = 60) { if (!t) return '—'; return t.length > max ? t.slice(0, max) + '…' : t; }
    function statusBadge(s) {
        if (!s) return '<span class="badge badge-muted">Unknown</span>';
        const m = { pending: 'badge-warning', active: 'badge-success', completed: 'badge-success', dismissed: 'badge-muted', reviewed: 'badge-info', action_taken: 'badge-danger', processing: 'badge-info', failed: 'badge-danger', blocked: 'badge-danger', plan_to_watch: 'badge-warning', watching: 'badge-info', on_hold: 'badge-warning', dropped: 'badge-muted' };
        return `<span class="badge ${m[s.toLowerCase()] || 'badge-muted'}">${toast.esc(s)}</span>`;
    }
    function boolBadge(val) { return val ? '<span class="badge badge-success">Active</span>' : '<span class="badge badge-danger">Inactive</span>'; }
    function roleBadge(role) { const c = { superadmin: 'badge-danger', usermanager: 'badge-warning', moderator: 'badge-info', auditor: 'badge-muted', support: 'badge-info', analytics: 'badge-success', finance: 'badge-warning', contentmanager: 'badge-info', marketing: 'badge-warning' }; return `<span class="badge ${c[role.toLowerCase()] || 'badge-muted'}">${toast.esc(role)}</span>`; }
    function jsonPretty(obj) { try { return JSON.stringify(obj, null, 2); } catch { return String(obj); } }
    return { formatDate, formatDateShort, truncate, statusBadge, boolBadge, roleBadge, jsonPretty };
})();