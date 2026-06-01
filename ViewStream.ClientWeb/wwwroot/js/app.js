(function () {
    // -------- Page mapping --------
    const pageMap = {
        home:               pages.home,
        login:              pages.login,
        register:           pages.register,
        confirmEmail:       pages.confirmEmail,
        forgotPassword:     pages.forgotPassword,
        resetPassword:      pages.resetPassword,
        profile:            pages.profile,
        shows:              pages.shows,
        showDetail:         pages.showDetail,
        episode:            pages.episode,
        library:            pages.library,
        history:            pages.history,
        friends:            pages.friends,
        subscriptions:      pages.subscriptions,
        notifications:      pages.notifications,
        watchParties:       pages.watchParties,
        sharedLists:        pages.sharedLists,
        downloads:          pages.downloads,
        genres:             pages.genres,
        persons:            pages.persons,
        personDetail:       pages.personDetail,
        notFound:           pages.notFound,
        // User account pages
        emailPreferences:        pages.emailPreferences,
        devices:                 pages.devices,
        sessions:                pages.sessions,
        dataDeletionRequests:    pages.dataDeletionRequests,
        userPromoUsages:         pages.userPromoUsages,
        userVector:              pages.userVector,

        // Admin: core
        adminDashboard:           pages.adminDashboard,
        adminUsers:               pages.adminUsers,
        adminContent:             pages.adminContent,
        adminContentSub:          pages.adminContentSub,
        adminEpisodes:            pages.adminEpisodes,
        adminSeasons:             pages.adminSeasons,
        adminAudioTracks:         pages.adminAudioTracks,
        adminSubtitles:           pages.adminSubtitles,
        adminCredits:             pages.adminCredits,
        adminAwards:              pages.adminAwards,
        adminShowAvailabilities:  pages.adminShowAvailabilities,
        adminShowAwards:          pages.adminShowAwards,
        adminPersonAwards:        pages.adminPersonAwards,
        adminItemVectors:         pages.adminItemVectors,
        adminUserVectors:         pages.adminUserVectors,
        adminCountries:           pages.adminCountries,
        adminReports:             pages.adminReports,
        adminLogs:                pages.adminLogs,
        adminAnalytics:           pages.adminAnalytics,
        adminMisc:                pages.adminMisc,
        adminPromos:              pages.adminPromos,
        adminRoles:               pages.adminRoles,
        adminNotify:              pages.adminNotify,
        adminDeletion:            pages.adminDeletion,
        adminErrors:              pages.adminErrors,
        adminAudit:               pages.adminAudit,
        // Admin: granular data management
        adminAuditLogs:               pages.adminAuditLogs,
        adminCommentLikes:            pages.adminCommentLikes,
        adminCommentReports:          pages.adminCommentReports,
        adminContentReports:          pages.adminContentReports,
        adminContentTags:             pages.adminContentTags,
        adminDevices:                 pages.adminDevices,
        adminEpisodeComments:         pages.adminEpisodeComments,
        adminFriendships:             pages.adminFriendships,
        adminGenres:                  pages.adminGenres,
        adminInvoices:                pages.adminInvoices,
        adminLoginSessions:           pages.adminLoginSessions,
        adminNotifications:           pages.adminNotifications,
        adminOfflineDownloads:        pages.adminOfflineDownloads,
        adminPaymentMethods:          pages.adminPaymentMethods,
        adminPermissions:             pages.adminPermissions,
        adminPersonalizedRows:        pages.adminPersonalizedRows,
        adminPlaybackEvents:          pages.adminPlaybackEvents,
        adminProfiles:                pages.adminProfiles,
        adminRatings:                 pages.adminRatings,
        adminSearchLogs:              pages.adminSearchLogs,
        adminSharedListItems:         pages.adminSharedListItems,
        adminSharedLists:             pages.adminSharedLists,
        adminSubscriptions:           pages.adminSubscriptions,
        adminUserInteractions:        pages.adminUserInteractions,
        adminUserLibraries:           pages.adminUserLibraries,
        adminUserPromoUsages:         pages.adminUserPromoUsages,
        adminWatchHistories:          pages.adminWatchHistories,
        adminWatchParties:            pages.adminWatchParties,
        adminWatchPartyParticipants:  pages.adminWatchPartyParticipants,
    };

    // -------- Sidebar sections --------
    const SIDEBAR_SECTIONS = [
        {
            title: 'General',
            items: [
                { icon: 'fa-home',         label: 'Home',          route: '/',              roles: [] },
                { icon: 'fa-film',         label: 'Shows',         route: '/shows',         roles: [] },
                { icon: 'fa-bookmark',     label: 'My Library',    route: '/library',       roles: [] },
                { icon: 'fa-history',      label: 'Watch History', route: '/history',       roles: [] },
                { icon: 'fa-download',     label: 'Downloads',     route: '/downloads',     roles: [] },
                { icon: 'fa-users',        label: 'Friends',       route: '/friends',       roles: [] },
                { icon: 'fa-list',         label: 'My Lists',      route: '/lists',         roles: [] },
                { icon: 'fa-tv',           label: 'Watch Parties', route: '/watch-parties', roles: [] },
                { icon: 'fa-bell',         label: 'Notifications', route: '/notifications', roles: [] },
                { icon: 'fa-credit-card',  label: 'Subscription',  route: '/subscriptions', roles: [] },
                { icon: 'fa-user-circle',  label: 'My Profile',    route: '/profile',       roles: [] },
            ]
        },
        {
            title: 'Account Settings',
            items: [
                { icon: 'fa-envelope',        label: 'Email Preferences', route: '/account/email-preferences', roles: [] },
                { icon: 'fa-laptop',          label: 'My Devices',        route: '/account/devices',           roles: [] },
                { icon: 'fa-shield-alt',      label: 'Login Sessions',    route: '/account/sessions',          roles: [] },
                { icon: 'fa-ticket-alt',      label: 'Promo Codes',       route: '/account/promo-codes',       roles: [] },
                { icon: 'fa-brain',           label: 'Recommendations',   route: '/account/recommendations',   roles: [] },
                { icon: 'fa-user-slash',      label: 'Data & Privacy',    route: '/account/data-deletion',     roles: [] },
            ]
        },
        {
            title: 'Administration',
            items: [
                { icon: 'fa-chart-line',           label: 'Dashboard',          route: '/admin',                    roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-users-cog',            label: 'User Management',    route: '/admin/users',              roles: ['SuperAdmin', 'UserManager'] },
                { icon: 'fa-film',                 label: 'Shows & Content',    route: '/admin/content',            roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-layer-group',          label: 'Seasons',            route: '/admin/seasons',            roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-play-circle',          label: 'Episodes',           route: '/admin/episodes',           roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-volume-up',            label: 'Audio Tracks',       route: '/admin/audiotracks',        roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-closed-captioning',    label: 'Subtitles',          route: '/admin/subtitles',          roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-user-tie',             label: 'Credits',            route: '/admin/credits',            roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-trophy',               label: 'Awards',             route: '/admin/awards',             roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-star',                 label: 'Show Awards',        route: '/admin/show-awards',        roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-medal',                label: 'Person Awards',      route: '/admin/person-awards',      roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-globe',                label: 'Availability',       route: '/admin/show-availabilities',roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-tags',                 label: 'Genres & Tags',      route: '/admin/content/genres',     roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-flag',                 label: 'Countries',          route: '/admin/countries',          roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-brain',                label: 'Item Vectors',       route: '/admin/itemvectors',        roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-user-astronaut',       label: 'User Vectors',       route: '/admin/uservectors',        roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-table',                label: 'Personalized Rows',  route: '/admin/personalized-rows',  roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-shield-alt',           label: 'Moderation',         route: '/admin/moderation',         roles: ['SuperAdmin', 'Moderator'] },
                { icon: 'fa-comment-slash',        label: 'Episode Comments',   route: '/admin/episode-comments',   roles: ['SuperAdmin', 'Moderator'] },
                { icon: 'fa-thumbs-up',            label: 'Comment Likes',      route: '/admin/comment-likes',      roles: ['SuperAdmin', 'Moderator'] },
                { icon: 'fa-flag',                 label: 'Comment Reports',    route: '/admin/comment-reports',    roles: ['SuperAdmin', 'Moderator'] },
                { icon: 'fa-exclamation-triangle', label: 'Content Reports',    route: '/admin/content-reports',    roles: ['SuperAdmin', 'Moderator'] },
                { icon: 'fa-star-half',            label: 'User Ratings',       route: '/admin/ratings',            roles: ['SuperAdmin', 'Moderator', 'Analytics'] },
                { icon: 'fa-clipboard-list',       label: 'Audit Logs',         route: '/admin/audit-logs',         roles: ['SuperAdmin', 'Auditor'] },
                { icon: 'fa-chart-bar',            label: 'Analytics',          route: '/admin/playback/events',    roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-search',               label: 'Search Logs',        route: '/admin/search-logs',        roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-play',                 label: 'Playback Events',    route: '/admin/playback-events',    roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-mouse-pointer',        label: 'User Interactions',  route: '/admin/user-interactions',  roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-user-friends',         label: 'Friendships',        route: '/admin/friendships',        roles: ['SuperAdmin', 'UserManager', 'Support'] },
                { icon: 'fa-desktop',              label: 'Devices',            route: '/admin/devices',            roles: ['SuperAdmin', 'UserManager', 'Support'] },
                { icon: 'fa-lock',                 label: 'Login Sessions',     route: '/admin/login-sessions',     roles: ['SuperAdmin', 'UserManager', 'Support'] },
                { icon: 'fa-id-card',              label: 'User Profiles',      route: '/admin/profiles',           roles: ['SuperAdmin', 'UserManager', 'Support'] },
                { icon: 'fa-bookmark',             label: 'User Libraries',     route: '/admin/user-libraries',     roles: ['SuperAdmin', 'UserManager', 'Support'] },
                { icon: 'fa-clock',                label: 'Watch Histories',    route: '/admin/watch-histories',    roles: ['SuperAdmin', 'Analytics', 'Support'] },
                { icon: 'fa-cloud-download-alt',   label: 'Offline Downloads',  route: '/admin/offline-downloads',  roles: ['SuperAdmin', 'Support'] },
                { icon: 'fa-users',                label: 'Watch Parties',      route: '/admin/watch-parties',      roles: ['SuperAdmin', 'Moderator', 'Support'] },
                { icon: 'fa-user-plus',            label: 'Party Participants', route: '/admin/watch-party-participants', roles: ['SuperAdmin', 'Moderator', 'Support'] },
                { icon: 'fa-list-ul',              label: 'Shared Lists',       route: '/admin/shared-lists',       roles: ['SuperAdmin', 'Moderator'] },
                { icon: 'fa-file-invoice-dollar',  label: 'Invoices',           route: '/admin/invoices',           roles: ['SuperAdmin', 'Finance'] },
                { icon: 'fa-credit-card',          label: 'Payment Methods',    route: '/admin/payment-methods',    roles: ['SuperAdmin', 'Finance'] },
                { icon: 'fa-receipt',              label: 'Subscriptions',      route: '/admin/subscriptions',      roles: ['SuperAdmin', 'Finance', 'Support'] },
                { icon: 'fa-percentage',           label: 'Promo Usages',       route: '/admin/user-promo-usages',  roles: ['SuperAdmin', 'Marketing', 'Finance'] },
                { icon: 'fa-tag',                  label: 'Promo Codes',        route: '/admin/promos',             roles: ['SuperAdmin', 'Marketing'] },
                { icon: 'fa-bell',                 label: 'Send Notification',  route: '/admin/notifications',      roles: ['SuperAdmin', 'Support'] },
                { icon: 'fa-bell',                 label: 'All Notifications',  route: '/admin/notifications-all',  roles: ['SuperAdmin', 'Support'] },
                { icon: 'fa-key',                  label: 'Roles & Permissions',route: '/admin/roles',              roles: ['SuperAdmin'] },
                { icon: 'fa-unlock',               label: 'Permissions',        route: '/admin/permissions',        roles: ['SuperAdmin'] },
                { icon: 'fa-trash-alt',            label: 'Data Deletion',      route: '/admin/data-deletion',      roles: ['SuperAdmin', 'DataProtectionOfficer'] },
            ]
        }
    ];

    function buildSidebar() {
        const collapsed = store.sidebarCollapsed;
        const w = collapsed ? 'w-16' : 'w-60';
        const showText = collapsed ? 'sr-only' : '';
        const logoText = collapsed ? '' : `<span class="font-display font-bold text-lg ml-3">ViewStream</span>`;

        let links = '';
        SIDEBAR_SECTIONS.forEach(section => {
            const visible = section.items.filter(item => item.roles.length === 0 || store.hasRole(...item.roles));
            if (!visible.length) return;
            links += `<div class="sidebar-section-title ${showText}">${toast.esc(section.title)}</div>`;
            visible.forEach(item => {
                const hash = router.getHash().split('?')[0];
                const isActive = hash === item.route || (item.route !== '/' && hash.startsWith(item.route));
                links += `<a href="#${item.route}" class="sidebar-link${isActive ? ' active' : ''}" title="${toast.esc(item.label)}">
                    <i class="fas ${item.icon}"></i><span class="${showText}">${toast.esc(item.label)}</span>
                </a>`;
            });
        });

        return `<aside class="sidebar-desktop fixed top-0 left-0 h-full bg-vs-surface border-r border-vs-border z-50 flex flex-col transition-all duration-300 ${w}">
            <div class="flex items-center justify-between p-4 border-b border-vs-border">
                <div class="flex items-center min-w-0">
                    <i class="fas fa-play text-lg text-vs-accent flex-shrink-0"></i>${logoText}
                </div>
                <button class="btn-ghost p-2 text-vs-muted hover:text-vs-text" onclick="store.toggleSidebar()" title="Toggle sidebar">
                    <i class="fas fa-${collapsed ? 'angle-right' : 'angle-left'}"></i>
                </button>
            </div>
            <nav class="flex-1 overflow-y-auto py-2">${links}</nav>
        </aside>`;
    }

    function buildMobileHeader() {
        return `<header class="mobile-header fixed top-0 left-0 right-0 bg-vs-surface border-b border-vs-border z-40 flex items-center justify-between px-4 py-3" style="display:none">
            <button class="btn-ghost p-2" onclick="store.toggleMobileSidebar()"><i class="fas fa-bars"></i></button>
            <span class="font-display font-bold text-vs-accent">ViewStream</span>
            <button class="btn-ghost p-2" onclick="store.logout()" title="Sign out"><i class="fas fa-sign-out-alt"></i></button>
        </header>`;
    }

    function buildMobileSidebar() {
        if (!store.mobileSidebarOpen) return '';
        return `<div class="fixed inset-0 z-50 lg:hidden" onclick="store.closeMobileSidebar()">
            <div class="absolute inset-0 bg-black/40"></div>
            <div class="fixed left-0 top-0 h-full w-64 bg-vs-surface z-50" style="animation:modalIn 0.2s ease">
                <div class="flex items-center justify-between p-4 border-b border-vs-border">
                    <span class="font-display font-bold text-vs-accent">ViewStream</span>
                    <button class="btn-ghost p-2" onclick="store.closeMobileSidebar()"><i class="fas fa-times"></i></button>
                </div>
                <nav class="overflow-y-auto py-2">${buildSidebarLinks()}</nav>
            </div>
        </div>`;
    }

    function buildSidebarLinks() {
        let links = '';
        SIDEBAR_SECTIONS.forEach(section => {
            const visible = section.items.filter(item => item.roles.length === 0 || store.hasRole(...item.roles));
            if (!visible.length) return;
            links += `<div class="sidebar-section-title">${toast.esc(section.title)}</div>`;
            visible.forEach(item => {
                const hash = router.getHash().split('?')[0];
                const isActive = hash === item.route || (item.route !== '/' && hash.startsWith(item.route));
                links += `<a href="#${item.route}" class="sidebar-link${isActive ? ' active' : ''}">
                    <i class="fas ${item.icon}"></i><span>${toast.esc(item.label)}</span>
                </a>`;
            });
        });
        return links;
    }

    // -------- Main renderPage --------
    window.renderPage = function (pageKey, params) {
        const root = document.getElementById('app');
        if (!root) return;

        const publicPages = ['login', 'register', 'confirmEmail', 'forgotPassword', 'resetPassword'];
        const isAuthLayout = !publicPages.includes(pageKey) && store.isAuthenticated();
        const isAdmin = pageKey.startsWith('admin');

        let layoutHtml = '';

        if (isAuthLayout) {
            if (isAdmin) {
                const sidebarW = store.sidebarCollapsed ? 'ml-16' : 'ml-60';
                layoutHtml = buildSidebar() + buildMobileHeader() + buildMobileSidebar() +
                    `<main class="main-shift transition-all duration-300 ${sidebarW} min-h-screen pt-0 lg:pt-0">
                        <div class="p-6 pt-4 lg:p-8 mt-14 lg:mt-0">
                            <div id="page-content"></div>
                        </div>
                    </main>`;
            } else {
                layoutHtml = `<div class="hero-gradient min-h-screen">
                    <nav class="fixed top-0 left-0 right-0 z-50 bg-vs-bg/80 backdrop-blur-xl border-b border-vs-border/50" role="navigation">
                        <div class="max-w-[1440px] mx-auto px-4 sm:px-6 h-16 flex items-center justify-between">
                            <a href="#/" class="flex items-center gap-2"><i class="fas fa-play text-vs-accent"></i><span class="font-display font-bold text-lg text-vs-text">ViewStream</span></a>
                            <div class="flex items-center gap-3">
                                <a href="#/shows" class="text-sm text-vs-dim hover:text-vs-text">Shows</a>
                                <a href="#/genres" class="text-sm text-vs-dim hover:text-vs-text">Genres</a>
                                <a href="#/persons" class="text-sm text-vs-dim hover:text-vs-text">People</a>
                                <a href="#/library" class="text-sm text-vs-dim hover:text-vs-text">Library</a>
                                <a href="#/profile" class="text-sm text-vs-dim hover:text-vs-text">Profile</a>
                                ${store.hasRole('SuperAdmin','ContentManager','Moderator','UserManager','Analytics','Auditor','Support','Finance','Marketing','DataProtectionOfficer')
                                    ? `<a href="#/admin" class="text-sm text-vs-accent hover:underline"><i class="fas fa-cog mr-1"></i>Admin</a>` : ''}
                                <button onclick="store.logout()" class="text-sm text-vs-error hover:underline">Sign Out</button>
                            </div>
                        </div>
                    </nav>
                    <main class="pt-16 max-w-[1440px] mx-auto min-h-screen page-enter">
                        <div class="px-4 sm:px-6 py-6" id="page-content"></div>
                    </main>
                    <footer class="border-t border-vs-border/30 mt-8">
                        <div class="max-w-[1440px] mx-auto px-6 py-8 text-center text-xs text-vs-muted">&copy; 2024 ViewStream</div>
                    </footer>
                </div>`;
            }
        } else {
            layoutHtml = `<main class="min-h-screen" id="page-content"></main>`;
        }

        root.innerHTML = layoutHtml;

        const contentContainer = document.getElementById('page-content');
        if (contentContainer) {
            const page = pageMap[pageKey] || pageMap.notFound;
            if (!page) { contentContainer.innerHTML = '<div class="p-8 text-center text-vs-muted">Page not found</div>'; return; }
            contentContainer.innerHTML = page.render(params);
            if (typeof page.init === 'function') page.init(params);
            if (typeof page.onAfterRender === 'function') page.onAfterRender(params);
        }
    };

    // -------- Initialize --------
    document.addEventListener('DOMContentLoaded', () => {
        store.init();
        router.init();
        document.addEventListener('keydown', e => {
            if (e.key === 'Escape') { modal.close(); store.closeMobileSidebar(); }
        });
    });

    store.onChange(() => {
        const pk = router.currentPageKey;
        if (pk && pk.startsWith('admin')) {
            const sidebar = document.querySelector('.sidebar-desktop');
            if (sidebar) sidebar.outerHTML = buildSidebar();
            const main = document.querySelector('main.main-shift');
            if (main) {
                const sidebarW = store.sidebarCollapsed ? 'ml-16' : 'ml-60';
                main.className = main.className.replace(/ml-\d+/, sidebarW);
            }
        }
    });
})();