const api = (() => {
    function toCamelCase(obj) {
        if (obj === null || typeof obj !== 'object') return obj;
        if (Array.isArray(obj)) return obj.map(toCamelCase);
        const newObj = {};
        for (const key in obj) {
            const camelKey = key.replace(/([-_][a-z])/gi, ($1) => $1.toUpperCase().replace('-', '').replace('_', ''));
            const finalKey = camelKey.charAt(0).toLowerCase() + camelKey.slice(1);
            newObj[finalKey] = toCamelCase(obj[key]);
        }
        return newObj;
    }

    function toPascalCase(obj) {
        if (obj === null || typeof obj !== 'object') return obj;
        if (Array.isArray(obj)) return obj.map(toPascalCase);
        const newObj = {};
        for (const key in obj) {
            const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
            newObj[pascalKey] = toPascalCase(obj[key]);
        }
        return newObj;
    }

    async function request(method, path, body, options = {}) {
        const url = CONFIG.API_BASE + path;
        const headers = { 'Content-Type': 'application/json' };
        const token = localStorage.getItem(CONFIG.TOKEN_KEY);
        if (token) headers['Authorization'] = 'Bearer ' + token;
        const fetchOpts = { method, headers };
        if (body !== undefined && body !== null) {
            fetchOpts.body = JSON.stringify(toPascalCase(body));
        }
        if (options.signal) fetchOpts.signal = options.signal;

        let response = await fetch(url, fetchOpts);
        if (response.status === 401 && !options._retry) {
            const refreshed = await tryRefresh();
            if (refreshed) {
                headers['Authorization'] = 'Bearer ' + localStorage.getItem(CONFIG.TOKEN_KEY);
                fetchOpts.headers = headers;
                response = await fetch(url, fetchOpts);
            } else {
                store.clearTokens();
                router.navigate('/login');
                throw new Error('Session expired. Please log in again.');
            }
        }
        if (!response.ok) {
            let data; try { data = await response.json(); } catch { data = {}; }
            const message = data.Message || data.message || 'Request failed';
            const err = new Error(message); err.status = response.status; err.data = toCamelCase(data); throw err;
        }
        if (response.status === 204) return null;
        const rawData = await response.json();
        return toCamelCase(rawData);
    }
    async function tryRefresh() {
        const rt = localStorage.getItem(CONFIG.REFRESH_KEY);
        if (!rt) return false;
        try {
            const res = await fetch(CONFIG.API_BASE + '/account/refresh', {
                method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ RefreshToken: rt })
            });
            if (!res.ok) { store.clearTokens(); return false; }
            const data = await res.json();
            const camelData = toCamelCase(data);
            store.setTokens(camelData.accessToken, camelData.refreshToken);
            if (camelData.user) store.setUser(camelData.user);
            return true;
        } catch { store.clearTokens(); return false; }
    }
    function get(path, params, opts) {
        const qs = params ? '?' + new URLSearchParams(Object.entries(params).filter(([, v]) => v !== undefined && v !== null && v !== '')).toString() : '';
        return request('GET', path + qs, undefined, opts);
    }
    function post(path, body, opts) { return request('POST', path, body, opts); }
    function put(path, body, opts) { return request('PUT', path, body, opts); }
    function del(path, body, opts) {
        if (body && (body.signal || body.headers || body.method || body._retry)) {
            opts = body;
            body = undefined;
        }
        return request('DELETE', path, body, opts);
    }
    async function upload(path, formData, opts) {
        const token = localStorage.getItem(CONFIG.TOKEN_KEY);
        const headers = {}; if (token) headers['Authorization'] = 'Bearer ' + token;
        const res = await fetch(CONFIG.API_BASE + path, { method: 'POST', headers, body: formData, signal: opts?.signal });
        if (!res.ok) { let data; try { data = await res.json(); } catch { data = {}; } throw new Error(data.Message || 'Upload failed'); }
        const rawData = await res.json();
        return toCamelCase(rawData);
    }
    return { request, get, post, put, del, delete: del, upload };
})();