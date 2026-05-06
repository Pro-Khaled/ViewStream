const store = (() => {
    const _listeners = [];
    const _state = {
        user: null, isAuthenticated: false,
        sidebarCollapsed: false, mobileSidebarOpen: false,
    };

    function init() {
        const token = localStorage.getItem(CONFIG.TOKEN_KEY);
        const raw = localStorage.getItem(CONFIG.USER_KEY);
        if (token && raw) {
            try { _state.user = JSON.parse(raw); _state.isAuthenticated = true; }
            catch { clearTokens(); }
        }
    }
    function getUser() { return _state.user; }
    function isAuthenticated() { return _state.isAuthenticated; }
    function getRoles() { return _state.user?.roles || []; }
    function isSuperAdmin() { return getRoles().includes('SuperAdmin'); }
    function hasRole(...roles) { if (isSuperAdmin()) return true; return roles.some(r => getRoles().includes(r)); }
    function setUser(user) { _state.user = user; _state.isAuthenticated = !!user; localStorage.setItem(CONFIG.USER_KEY, JSON.stringify(user)); _notify(); }
    function setTokens(access, refresh) { localStorage.setItem(CONFIG.TOKEN_KEY, access); if (refresh) localStorage.setItem(CONFIG.REFRESH_KEY, refresh); }
    function clearTokens() { localStorage.removeItem(CONFIG.TOKEN_KEY); localStorage.removeItem(CONFIG.REFRESH_KEY); localStorage.removeItem(CONFIG.USER_KEY); _state.user = null; _state.isAuthenticated = false; _notify(); }
    function logout() {
        const rt = localStorage.getItem(CONFIG.REFRESH_KEY);
        if (rt) { fetch(CONFIG.API_BASE + '/account/logout', { method: 'POST', headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + (localStorage.getItem(CONFIG.TOKEN_KEY) || '') }, body: JSON.stringify({ refreshToken: rt }) }).catch(() => { }); }
        clearTokens(); router.navigate('/login'); toast.info('Logged out successfully');
    }
    function toggleSidebar() { _state.sidebarCollapsed = !_state.sidebarCollapsed; _notify(); }
    function toggleMobileSidebar() { _state.mobileSidebarOpen = !_state.mobileSidebarOpen; _notify(); }
    function closeMobileSidebar() { _state.mobileSidebarOpen = false; _notify(); }
    function onChange(fn) { _listeners.push(fn); }
    function _notify() { _listeners.forEach(fn => fn(_state)); }
    return { init, getUser, isAuthenticated, getRoles, isSuperAdmin, hasRole, setUser, setTokens, clearTokens, logout, toggleSidebar, toggleMobileSidebar, closeMobileSidebar, onChange, get sidebarCollapsed() { return _state.sidebarCollapsed }, get mobileSidebarOpen() { return _state.mobileSidebarOpen } };
})();