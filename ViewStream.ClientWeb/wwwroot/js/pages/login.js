pages.login = {
    render() {
        return `<div class="min-h-screen flex items-center justify-center p-4 hero-gradient">
            <div class="w-full max-w-md">
                <div class="text-center mb-8">
                    <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-vs-accent to-vs-accentDim flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-play text-vs-bg text-xl"></i>
                    </div>
                    <h1 class="font-display font-black text-3xl text-vs-text">Welcome Back</h1>
                    <p class="text-vs-muted mt-2">Sign in to continue watching</p>
                </div>
                <form id="login-form" class="bg-vs-surface border border-vs-border rounded-2xl p-6 space-y-4 shadow-xl shadow-black/20" novalidate>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Email</label>
                        <input type="email" id="login-email" value="" required class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50 focus:ring-1 focus:ring-vs-accent/20 transition-all" placeholder="you@example.com">
                        <p class="text-xs text-vs-error mt-1 hidden" id="login-email-err"></p>
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Password</label>
                        <input type="password" id="login-password" required class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50 focus:ring-1 focus:ring-vs-accent/20 transition-all" placeholder="Enter your password">
                        <p class="text-xs text-vs-error mt-1 hidden" id="login-pass-err"></p>
                    </div>
                    <div id="login-error" class="hidden p-3 rounded-lg bg-vs-error/10 border border-vs-error/20 text-sm text-vs-error"></div>
                    <button type="submit" class="w-full py-3 bg-vs-accent hover:bg-vs-accentHover text-vs-bg font-bold rounded-xl transition-colors text-sm">Sign In</button>
                    <p class="text-center text-sm text-vs-muted">Don't have an account? <a href="#/register" class="text-vs-accent hover:underline font-medium">Sign up</a></p>
                </form>
            </div>
        </div>`;
    },
    init() {
        document.getElementById('login-form')?.addEventListener('submit', async (e) => {
            e.preventDefault();
            const email = document.getElementById('login-email').value.trim();
            const password = document.getElementById('login-password').value;
            let valid = true;
            const emailErr = document.getElementById('login-email-err');
            const passErr = document.getElementById('login-pass-err');
            if (!email) { emailErr.textContent = 'Email is required'; emailErr.classList.remove('hidden'); valid = false; }
            else { emailErr.classList.add('hidden'); }
            if (!password) { passErr.textContent = 'Password is required'; passErr.classList.remove('hidden'); valid = false; }
            else { passErr.classList.add('hidden'); }
            if (!valid) return;

            const btn = e.target.querySelector('button[type="submit"]');
            btn.disabled = true; btn.innerHTML = 'Signing in...';
            try {
                const authResponse = await api.post('/account/login', { email, password });
                store.setTokens(authResponse.accessToken, authResponse.refreshToken);
                const user = await api.get('/account/me');
                store.setUser(user);
                toast.success(`Welcome back, ${user.fullName || user.email}!`);
                router.navigate('/');
            } catch (err) {
                document.getElementById('login-error').textContent = err.response?.data?.message || err.message;
                document.getElementById('login-error').classList.remove('hidden');
            } finally {
                btn.disabled = false; btn.innerHTML = 'Sign In';
            }
        });
    }
};