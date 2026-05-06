const router = (() => {
    const routes = [];

    function register(pattern, pageKey) {
        const paramNames = [];
        const regexStr = '^' + pattern.replace(/:(\w+)/g, (_, n) => { paramNames.push(n); return '([^/]+)'; }) + '$';
        routes.push({ pattern, regex: new RegExp(regexStr), pageKey, paramNames });
    }

    function navigate(path) { window.location.hash = path; }
    function getHash() { return window.location.hash.slice(1) || '/'; }
    function getQueryParams() {
        const h = getHash();
        const q = h.indexOf('?');
        if (q === -1) return {};
        return Object.fromEntries(new URLSearchParams(h.slice(q + 1)));
    }

    function resolve() {
        const hash = getHash();
        const qIdx = hash.indexOf('?');
        const path = qIdx === -1 ? hash : hash.slice(0, qIdx);
        let match = null;

        for (const r of routes) {
            const m = path.match(r.regex);
            if (m) {
                const params = {};
                r.paramNames.forEach((n, i) => { params[n] = m[i + 1]; });
                match = { pageKey: r.pageKey, params };
                break;
            }
        }

        // If no match, redirect to home
        if (!match) { navigate('/'); return; }

        const { pageKey, params } = match;

        // Auth gating
        const publicPages = ['login', 'register', 'confirmEmail'];
        if (!publicPages.includes(pageKey) && !store.isAuthenticated()) { navigate('/login'); return; }
        if (pageKey === 'login' && store.isAuthenticated()) { navigate('/'); return; }

        // Role gating (admin pages)
        const roleMap = {
            adminDashboard: ['SuperAdmin', 'Analytics'],
            adminUsers: ['SuperAdmin', 'UserManager'],
            adminContent: ['SuperAdmin', 'ContentManager'],
            adminContentSub: ['SuperAdmin', 'ContentManager'],
            adminModeration: ['SuperAdmin', 'Moderator'],
            adminLogs: ['SuperAdmin', 'Auditor', 'Support', 'Analytics'],
            adminReports: ['SuperAdmin', 'Moderator'],
            adminAnalytics: ['SuperAdmin', 'Analytics'],
            adminMisc: ['SuperAdmin', 'Finance', 'Support', 'DataProtectionOfficer'],
            adminPromos: ['SuperAdmin', 'Marketing'],
            adminRoles: ['SuperAdmin'],
            adminNotify: ['SuperAdmin', 'Support'],
            adminDeletion: ['SuperAdmin', 'DataProtectionOfficer'],
            adminErrors: ['SuperAdmin', 'Support'],
            adminAudit: ['SuperAdmin', 'Auditor'],
        };

        if (roleMap[pageKey] && !store.hasRole(...roleMap[pageKey])) {
            toast.error('Access denied');
            navigate('/');
            return;
        }

        // Render the page via the global renderPage function (defined in app.js)
        if (typeof renderPage === 'function') {
            renderPage(pageKey, params);
        }
    }

    function init() {
        window.addEventListener('hashchange', resolve);
        resolve();
    }

    // Register all routes
    // Public
    register('/', 'home');
    register('/login', 'login');
    register('/register', 'register');
    register('/confirm-email', 'confirmEmail');

    // User authenticated
    register('/profile', 'profile');
    register('/shows', 'shows');
    register('/shows/:id', 'showDetail');
    register('/episode/:id', 'episode');
    register('/genres', 'genres');
    register('/persons', 'persons');
    register('/persons/:id', 'personDetail');
    register('/library', 'library');
    register('/history', 'history');
    register('/friends', 'friends');
    register('/subscriptions', 'subscriptions');
    register('/notifications', 'notifications');
    register('/watch-parties', 'watchParties');
    register('/lists', 'sharedLists');

    // Admin
    register('/admin', 'adminDashboard');
    register('/admin/users', 'adminUsers');
    register('/admin/content', 'adminContent');
    register('/admin/content/genres', 'adminContentSub');
    register('/admin/content/tags', 'adminContentSub');
    register('/admin/content/persons', 'adminContentSub');
    register('/admin/moderation', 'adminReports');
    register('/admin/audit', 'adminLogs');   // logs page with audit tab active
    register('/admin/errors', 'adminLogs');  // logs page with errors tab active
    register('/admin/search/logs', 'adminLogs'); // search logs tab
    register('/admin/data-deletion', 'adminMisc');
    register('/admin/promos', 'adminPromos');
    register('/admin/roles', 'adminRoles');
    register('/admin/notifications', 'adminNotify');
    register('/admin/playback/events', 'adminAnalytics');
    register('/admin/interactions', 'adminAnalytics');

    return { register, navigate, getHash, getQueryParams, resolve, init };
})();