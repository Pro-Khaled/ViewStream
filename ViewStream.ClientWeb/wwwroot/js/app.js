(function () {
    // -------- Page mapping (pageKey -> pages.xxx) --------
    const pageMap = {
        home: pages.home,
        login: pages.login,
        register: pages.register,
        confirmEmail: pages.confirmEmail,
        forgotPassword: pages.forgotPassword,
        resetPassword: pages.resetPassword,
        profile: pages.profile,
        shows: pages.shows,
        showDetail: pages.showDetail,
        episode: pages.episode,
        library: pages.library,
        history: pages.history,
        friends: pages.friends,
        subscriptions: pages.subscriptions,
        notifications: pages.notifications,
        watchParties: pages.watchParties,
        sharedLists: pages.sharedLists,
        downloads: pages.downloads,
        genres: pages.genres,
        persons: pages.persons,
        personDetail: pages.personDetail,
        notFound: pages.notFound,

        // Admin
        adminDashboard: pages.adminDashboard,
        adminUsers: pages.adminUsers,
        adminContent: pages.adminContent,
        adminContentSub: pages.adminContentSub,
        adminReports: pages.adminReports,   // moderation
        adminLogs: pages.adminLogs,         // audit/errors/search
        adminAnalytics: pages.adminAnalytics,
        adminMisc: pages.adminMisc,         // invoices, notifications, deletions
        adminPromos: pages.adminPromos,
        adminRoles: pages.adminRoles,
        adminNotify: pages.adminNotify,
        adminDeletion: pages.adminDeletion,
        adminErrors: pages.adminErrors,
        adminAudit: pages.adminAudit,
    };

    // -------- Sidebar sections (same as before) --------
    const SIDEBAR_SECTIONS = [
        {
            title: 'General',
            items: [
                { icon: 'fa-home', label: 'Dashboard', route: '/', roles: [] },
                { icon: 'fa-user-circle', label: 'My Profile', route: '/profile', roles: [] },
                { icon: 'fa-film', label: 'Shows', route: '/shows', roles: [] },
                { icon: 'fa-bookmark', label: 'My Library', route: '/library', roles: [] },
                { icon: 'fa-history', label: 'Watch History', route: '/history', roles: [] },
                { icon: 'fa-download', label: 'Downloads', route: '/downloads', roles: [] },
                { icon: 'fa-users', label: 'Friends', route: '/friends', roles: [] },
            ]
        },
        {
            title: 'Administration',
            items: [
                { icon: 'fa-chart-line', label: 'Dashboard', route: '/admin', roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-users-cog', label: 'User Management', route: '/admin/users', roles: ['SuperAdmin', 'UserManager'] },
                { icon: 'fa-film', label: 'Content Management', route: '/admin/content', roles: ['SuperAdmin', 'ContentManager'] },
                { icon: 'fa-shield-alt', label: 'Moderation', route: '/admin/moderation', roles: ['SuperAdmin', 'Moderator'] },
                { icon: 'fa-clipboard-list', label: 'Logs', route: '/admin/audit', roles: ['SuperAdmin', 'Auditor', 'Support', 'Analytics'] },
                { icon: 'fa-chart-bar', label: 'Analytics', route: '/admin/playback/events', roles: ['SuperAdmin', 'Analytics'] },
                { icon: 'fa-file-invoice-dollar', label: 'Invoices & Misc', route: '/admin/data-deletion', roles: ['SuperAdmin', 'DataProtectionOfficer', 'Finance', 'Support'] },
                { icon: 'fa-tag', label: 'Promo Codes', route: '/admin/promos', roles: ['SuperAdmin', 'Marketing'] },
                { icon: 'fa-key', label: 'Roles & Permissions', route: '/admin/roles', roles: ['SuperAdmin'] },
                { icon: 'fa-bell', label: 'Send Notification', route: '/admin/notifications', roles: ['SuperAdmin', 'Support'] },
            ]
        }
    ];

    function buildSidebar() {
        const collapsed = store.sidebarCollapsed;
        const w = collapsed ? 'w-16' : 'w-60';
        const showText = collapsed ? 'sr-only' : '';
        const logoText = collapsed ? '' : '<span class="font-display font-bold text-lg ml-3">ViewStream</span>';

        let links = '';
        SIDEBAR_SECTIONS.forEach(section => {
            const visible = section.items.filter(item => item.roles.length === 0 || store.hasRole(...item.roles));
            if (!visible.length) return;
            links += `<div class="sidebar-section-title ${showText}">${toast.esc(section.title)}</div>`;
            visible.forEach(item => {
                const isActive = router.getHash() === item.route || (item.route !== '/' && router.getHash().startsWith(item.route));
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
        // Similar to desktop sidebar but full-screen modal
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
                const isActive = router.getHash() === item.route || (item.route !== '/' && router.getHash().startsWith(item.route));
                links += `<a href="#${item.route}" class="sidebar-link${isActive ? ' active' : ''}">
                    <i class="fas ${item.icon}"></i><span>${toast.esc(item.label)}</span>
                </a>`;
            });
        });
        return links;
    }

    // -------- Main renderPage function --------
    window.renderPage = function (pageKey, params) {
        const root = document.getElementById('app');
        if (!root) return;

        const publicPages = ['login', 'register', 'confirmEmail', 'forgotPassword', 'resetPassword'];
        const isAuthLayout = !publicPages.includes(pageKey) && store.isAuthenticated();
        const isAdmin = pageKey.startsWith('admin');

        let layoutHtml = '';

        if (isAuthLayout) {
            if (isAdmin) {
                // Admin layout: sidebar + main
                const sidebarW = store.sidebarCollapsed ? 'ml-16' : 'ml-60';
                layoutHtml = buildSidebar() + buildMobileHeader() + buildMobileSidebar() +
                    `<main class="main-shift transition-all duration-300 ${sidebarW} min-h-screen pt-0 lg:pt-0">
                        <div class="p-6 pt-4 lg:p-8 mt-14 lg:mt-0">
                            <div id="page-content"></div>
                        </div>
                    </main>`;
            } else {
                // User layout: navbar + footer
                // Use a simplified public layout (no admin sidebar)
                layoutHtml = `<div class="hero-gradient min-h-screen">
                    <nav class="fixed top-0 left-0 right-0 z-50 bg-vs-bg/80 backdrop-blur-xl border-b border-vs-border/50" role="navigation">
                        <div class="max-w-[1440px] mx-auto px-4 sm:px-6 h-16 flex items-center justify-between">
                            <a href="#/" class="flex items-center gap-2"><i class="fas fa-play text-vs-accent"></i><span class="font-display font-bold text-lg text-vs-text">ViewStream</span></a>
                            <div class="flex items-center gap-3">
                                <a href="#/shows" class="text-sm text-vs-dim hover:text-vs-text">Shows</a>
                                <a href="#/genres" class="text-sm text-vs-dim hover:text-vs-text">Genres</a>
                                <a href="#/profile" class="text-sm text-vs-dim hover:text-vs-text">Profile</a>
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
            // Public layout (no sidebar / navbar)
            layoutHtml = `<main class="min-h-screen" id="page-content"></main>`;
        }

        root.innerHTML = layoutHtml;

        const contentContainer = document.getElementById('page-content');
        if (contentContainer) {
            const page = pageMap[pageKey] || pageMap.notFound;
            contentContainer.innerHTML = page.render(params);
            // Call init after render
            if (typeof page.init === 'function') {
                page.init(params);
            }
            // Call onAfterRender if defined (used for post-render DOM wiring)
            if (typeof page.onAfterRender === 'function') {
                page.onAfterRender(params);
            }
        }
    };

    // -------- Initialize --------
    document.addEventListener('DOMContentLoaded', () => {
        store.init();
        router.init();
        // Ctrl+K to close modals
        document.addEventListener('keydown', e => {
            if (e.key === 'Escape') {
                modal.close();
                store.closeMobileSidebar();
            }
        });
    });

    // Listen for store changes to update sidebar
    store.onChange(() => {
        if (router.currentPageKey && router.currentPageKey.startsWith('admin')) {
            // Re-render only the sidebar portion (optional)
            const sidebar = document.querySelector('.sidebar-desktop');
            if (sidebar) {
                sidebar.outerHTML = buildSidebar();
            }
            // Update main margin
            const main = document.querySelector('main.main-shift');
            if (main) {
                const sidebarW = store.sidebarCollapsed ? 'ml-16' : 'ml-60';
                main.className = main.className.replace(/ml-\d+/g, sidebarW);
            }
        }
    });
})();