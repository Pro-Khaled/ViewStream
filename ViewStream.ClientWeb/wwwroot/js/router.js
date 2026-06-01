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

        if (!match) { navigate('/'); return; }

        const { pageKey, params } = match;

        // Auth gating
        const publicPages = ['login', 'register', 'confirmEmail', 'forgotPassword', 'resetPassword'];
        if (!publicPages.includes(pageKey) && !store.isAuthenticated()) { navigate('/login'); return; }
        if (pageKey === 'login' && store.isAuthenticated()) { navigate('/'); return; }

        // Role gating (admin pages)
        const roleMap = {
            adminDashboard:           ['SuperAdmin', 'Analytics'],
            adminUsers:               ['SuperAdmin', 'UserManager'],
            adminContent:             ['SuperAdmin', 'ContentManager'],
            adminContentSub:          ['SuperAdmin', 'ContentManager'],
            adminEpisodes:            ['SuperAdmin', 'ContentManager'],
            adminSeasons:             ['SuperAdmin', 'ContentManager'],
            adminAudioTracks:         ['SuperAdmin', 'ContentManager'],
            adminSubtitles:           ['SuperAdmin', 'ContentManager'],
            adminCredits:             ['SuperAdmin', 'ContentManager'],
            adminAwards:              ['SuperAdmin', 'ContentManager'],
            adminShowAvailabilities:  ['SuperAdmin', 'ContentManager'],
            adminShowAwards:          ['SuperAdmin', 'ContentManager'],
            adminPersonAwards:        ['SuperAdmin', 'ContentManager'],
            adminItemVectors:         ['SuperAdmin', 'ContentManager'],
            adminUserVectors:         ['SuperAdmin', 'Analytics'],
            adminCountries:           ['SuperAdmin', 'ContentManager'],
            adminModeration:          ['SuperAdmin', 'Moderator'],
            adminReports:             ['SuperAdmin', 'Moderator'],
            adminLogs:                ['SuperAdmin', 'Auditor', 'Support', 'Analytics'],
            adminAnalytics:           ['SuperAdmin', 'Analytics'],
            adminMisc:                ['SuperAdmin', 'Finance', 'Support', 'DataProtectionOfficer'],
            adminPromos:              ['SuperAdmin', 'Marketing'],
            adminRoles:               ['SuperAdmin'],
            adminNotify:              ['SuperAdmin', 'Support'],
            adminDeletion:            ['SuperAdmin', 'DataProtectionOfficer'],
            adminErrors:              ['SuperAdmin', 'Support'],
            adminAudit:               ['SuperAdmin', 'Auditor'],
            // Granular data management pages
            adminAuditLogs:               ['SuperAdmin', 'Auditor'],
            adminCommentLikes:            ['SuperAdmin', 'Moderator'],
            adminCommentReports:          ['SuperAdmin', 'Moderator'],
            adminContentReports:          ['SuperAdmin', 'Moderator'],
            adminContentTags:             ['SuperAdmin', 'ContentManager'],
            adminDevices:                 ['SuperAdmin', 'UserManager', 'Support'],
            adminEpisodeComments:         ['SuperAdmin', 'Moderator'],
            adminFriendships:             ['SuperAdmin', 'UserManager', 'Support'],
            adminGenres:                  ['SuperAdmin', 'ContentManager'],
            adminInvoices:                ['SuperAdmin', 'Finance'],
            adminLoginSessions:           ['SuperAdmin', 'UserManager', 'Support'],
            adminNotifications:           ['SuperAdmin', 'Support'],
            adminOfflineDownloads:        ['SuperAdmin', 'Support'],
            adminPaymentMethods:          ['SuperAdmin', 'Finance'],
            adminPermissions:             ['SuperAdmin'],
            adminPersonalizedRows:        ['SuperAdmin', 'ContentManager'],
            adminPlaybackEvents:          ['SuperAdmin', 'Analytics'],
            adminProfiles:                ['SuperAdmin', 'UserManager', 'Support'],
            adminRatings:                 ['SuperAdmin', 'Moderator', 'Analytics'],
            adminSearchLogs:              ['SuperAdmin', 'Analytics'],
            adminSharedListItems:         ['SuperAdmin', 'Moderator'],
            adminSharedLists:             ['SuperAdmin', 'Moderator'],
            adminSubscriptions:           ['SuperAdmin', 'Finance', 'Support'],
            adminUserInteractions:        ['SuperAdmin', 'Analytics'],
            adminUserLibraries:           ['SuperAdmin', 'UserManager', 'Support'],
            adminUserPromoUsages:         ['SuperAdmin', 'Marketing', 'Finance'],
            adminWatchHistories:          ['SuperAdmin', 'Analytics', 'Support'],
            adminWatchParties:            ['SuperAdmin', 'Moderator', 'Support'],
            adminWatchPartyParticipants:  ['SuperAdmin', 'Moderator', 'Support'],
        };

        if (roleMap[pageKey] && !store.hasRole(...roleMap[pageKey])) {
            toast.error('Access denied');
            navigate('/');
            return;
        }

        router.currentPageKey = pageKey;
        if (typeof renderPage === 'function') {
            renderPage(pageKey, params);
        }
    }

    function init() {
        window.addEventListener('hashchange', resolve);
        resolve();
    }

    // ---- Public ----
    register('/', 'home');
    register('/login', 'login');
    register('/register', 'register');
    register('/confirm-email', 'confirmEmail');
    register('/forgot-password', 'forgotPassword');
    register('/reset-password', 'resetPassword');

    // ---- User authenticated ----
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
    register('/downloads', 'downloads');
    // User account management
    register('/account/email-preferences', 'emailPreferences');
    register('/account/devices', 'devices');
    register('/account/sessions', 'sessions');
    register('/account/data-deletion', 'dataDeletionRequests');
    register('/account/promo-codes', 'userPromoUsages');
    register('/account/recommendations', 'userVector');

    // ---- Admin: Content ----
    register('/admin', 'adminDashboard');
    register('/admin/users', 'adminUsers');
    register('/admin/content', 'adminContent');
    register('/admin/content/genres', 'adminContentSub');
    register('/admin/content/tags', 'adminContentSub');
    register('/admin/content/persons', 'adminContentSub');
    register('/admin/content/countries', 'adminCountries');
    register('/admin/episodes', 'adminEpisodes');
    register('/admin/seasons', 'adminSeasons');
    register('/admin/audiotracks', 'adminAudioTracks');
    register('/admin/subtitles', 'adminSubtitles');
    register('/admin/credits', 'adminCredits');
    register('/admin/awards', 'adminAwards');
    register('/admin/show-availabilities', 'adminShowAvailabilities');
    register('/admin/show-awards', 'adminShowAwards');
    register('/admin/person-awards', 'adminPersonAwards');
    register('/admin/itemvectors', 'adminItemVectors');
    register('/admin/uservectors', 'adminUserVectors');
    register('/admin/countries', 'adminCountries');

    // ---- Admin: Moderation & Logs ----
    register('/admin/moderation', 'adminReports');
    register('/admin/audit', 'adminLogs');
    register('/admin/errors', 'adminLogs');
    register('/admin/search/logs', 'adminLogs');
    register('/admin/data-deletion', 'adminMisc');
    register('/admin/promos', 'adminPromos');
    register('/admin/roles', 'adminRoles');
    register('/admin/notifications', 'adminNotify');
    register('/admin/playback/events', 'adminAnalytics');
    register('/admin/interactions', 'adminAnalytics');

    // ---- Admin: Granular data management ----
    register('/admin/audit-logs', 'adminAuditLogs');
    register('/admin/comment-likes', 'adminCommentLikes');
    register('/admin/comment-reports', 'adminCommentReports');
    register('/admin/content-reports', 'adminContentReports');
    register('/admin/content-tags', 'adminContentTags');
    register('/admin/devices', 'adminDevices');
    register('/admin/episode-comments', 'adminEpisodeComments');
    register('/admin/friendships', 'adminFriendships');
    register('/admin/genres', 'adminGenres');
    register('/admin/invoices', 'adminInvoices');
    register('/admin/login-sessions', 'adminLoginSessions');
    register('/admin/notifications-all', 'adminNotifications');
    register('/admin/offline-downloads', 'adminOfflineDownloads');
    register('/admin/payment-methods', 'adminPaymentMethods');
    register('/admin/permissions', 'adminPermissions');
    register('/admin/personalized-rows', 'adminPersonalizedRows');
    register('/admin/playback-events', 'adminPlaybackEvents');
    register('/admin/profiles', 'adminProfiles');
    register('/admin/ratings', 'adminRatings');
    register('/admin/search-logs', 'adminSearchLogs');
    register('/admin/shared-list-items', 'adminSharedListItems');
    register('/admin/shared-lists', 'adminSharedLists');
    register('/admin/subscriptions', 'adminSubscriptions');
    register('/admin/user-interactions', 'adminUserInteractions');
    register('/admin/user-libraries', 'adminUserLibraries');
    register('/admin/user-promo-usages', 'adminUserPromoUsages');
    register('/admin/watch-histories', 'adminWatchHistories');
    register('/admin/watch-parties', 'adminWatchParties');
    register('/admin/watch-party-participants', 'adminWatchPartyParticipants');

    return { register, navigate, getHash, getQueryParams, resolve, init, currentPageKey: null };
})();