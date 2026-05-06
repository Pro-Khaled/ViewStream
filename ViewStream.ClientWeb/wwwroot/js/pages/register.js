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
            const successEl = document.getElementById('reg-success');
            errEl.classList.add('hidden'); successEl.classList.add('hidden');
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
                successEl.textContent = res.message || 'Registration successful! Please check your email.';
                successEl.classList.remove('hidden');
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