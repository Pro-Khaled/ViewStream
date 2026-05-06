// ========== Login ==========
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

// ========== Register ==========
pages.register = {
    render() {
        return `<div class="min-h-screen flex items-center justify-center p-4 hero-gradient">
            <div class="w-full max-w-md">
                <div class="text-center mb-8">
                    <div class="w-14 h-14 rounded-2xl bg-gradient-to-br from-vs-accent to-vs-accentDim flex items-center justify-center mx-auto mb-4">
                        <i class="fas fa-play text-vs-bg text-xl"></i>
                    </div>
                    <h1 class="font-display font-black text-3xl text-vs-text">Create Account</h1>
                    <p class="text-vs-muted mt-2">Start your streaming journey</p>
                </div>
                <form id="register-form" class="bg-vs-surface border border-vs-border rounded-2xl p-6 space-y-4 shadow-xl shadow-black/20" novalidate>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Full Name</label>
                        <input type="text" id="reg-name" required class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50" placeholder="John Doe">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Email</label>
                        <input type="email" id="reg-email" required class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50" placeholder="you@example.com">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Password</label>
                        <input type="password" id="reg-password" required minlength="8" class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50" placeholder="Min 8 characters">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-vs-text mb-1.5">Confirm Password</label>
                        <input type="password" id="reg-confirm" required class="w-full bg-vs-card border border-vs-border rounded-lg px-4 py-2.5 text-sm text-vs-text placeholder:text-vs-muted focus:outline-none focus:border-vs-accent/50" placeholder="Re-enter password">
                    </div>
                    <div id="reg-error" class="hidden p-3 rounded-lg bg-vs-error/10 border border-vs-error/20 text-sm text-vs-error"></div>
                    <div id="reg-success" class="hidden p-3 rounded-lg bg-vs-success/10 border border-vs-success/20 text-sm text-vs-success"></div>
                    <button type="submit" class="w-full py-3 bg-vs-accent hover:bg-vs-accentHover text-vs-bg font-bold rounded-xl transition-colors text-sm">Create Account</button>
                    <p class="text-center text-sm text-vs-muted">Already have an account? <a href="#/login" class="text-vs-accent hover:underline font-medium">Sign in</a></p>
                </form>
            </div>
        </div>`;
    },
    init() {
        document.getElementById('register-form')?.addEventListener('submit', async (e) => {
            e.preventDefault();
            const name = document.getElementById('reg-name').value.trim();
            const email = document.getElementById('reg-email').value.trim();
            const pass = document.getElementById('reg-password').value;
            const confirm = document.getElementById('reg-confirm').value;
            const errEl = document.getElementById('reg-error');
            const succEl = document.getElementById('reg-success');
            errEl.classList.add('hidden'); succEl.classList.add('hidden');
            if (!name || !email || !pass || !confirm) {
                errEl.textContent = 'All fields are required';
                errEl.classList.remove('hidden'); return;
            }
            if (pass.length < 8) {
                errEl.textContent = 'Password must be at least 8 characters';
                errEl.classList.remove('hidden'); return;
            }
            if (pass !== confirm) {
                errEl.textContent = 'Passwords do not match';
                errEl.classList.remove('hidden'); return;
            }
            const btn = e.target.querySelector('button[type="submit"]');
            btn.disabled = true; btn.innerHTML = 'Creating...';
            try {
                const res = await api.post('/account/register', {
                    fullName: name, email, password: pass, confirmPassword: confirm
                });
                succEl.textContent = res.message || 'Registration successful! Please check your email.';
                succEl.classList.remove('hidden');
                btn.style.display = 'none';
                toast.success('Account created!');
            } catch (err) {
                errEl.textContent = err.message;
                errEl.classList.remove('hidden');
                btn.disabled = false; btn.innerHTML = 'Create Account';
            }
        });
    }
};

// ========== Confirm Email ==========
pages.confirmEmail = {
    render() {
        return `<div class="min-h-screen flex items-center justify-center p-4">
            <div class="card p-8 w-full max-w-md text-center" id="confirm-container">
                <div class="spinner mx-auto mb-4"></div>
                <p class="text-muted">Verifying your email…</p>
            </div>
        </div>`;
    },
    async init() {
        const container = document.getElementById('confirm-container');
        const params = router.getQueryParams();
        if (!params.userId || !params.token) {
            container.innerHTML = `<i class="fas fa-times-circle text-vs-error text-5xl mb-4"></i>
                <h2 class="font-display font-bold text-xl mb-2">Invalid Link</h2>
                <p class="text-muted mb-6">The confirmation link is malformed.</p>
                <a href="#/login" class="btn btn-primary">Go to Login</a>`;
            return;
        }
        try {
            await api.get('/account/confirm-email', { userId: params.userId, token: params.token });
            container.innerHTML = `<i class="fas fa-check-circle text-vs-success text-5xl mb-4"></i>
                <h2 class="font-display font-bold text-xl mb-2">Email Confirmed</h2>
                <p class="text-muted mb-6">Your email has been verified. You can now sign in.</p>
                <a href="#/login" class="btn btn-primary">Sign In</a>`;
        } catch (err) {
            container.innerHTML = `<i class="fas fa-times-circle text-vs-error text-5xl mb-4"></i>
                <h2 class="font-display font-bold text-xl mb-2">Confirmation Failed</h2>
                <p class="text-muted mb-6">${toast.esc(err.message)}</p>
                <a href="#/login" class="btn btn-primary">Go to Login</a>`;
        }
    }
};