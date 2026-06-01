pages.forgotPassword = (() => {
    function render() {
        return `<div id="forgot-pw-page" class="min-h-screen flex items-center justify-center px-4" style="background:radial-gradient(ellipse at 60% 20%,#1a1230 0%,#0d0d12 70%)">
            <div style="width:100%;max-width:420px">
                <div class="text-center mb-8">
                    <a href="#/" class="inline-flex items-center gap-2 text-vs-accent font-display font-black text-2xl mb-6 hover:opacity-80 transition-opacity">
                        <i class="fas fa-play-circle"></i> ViewStream
                    </a>
                    <h1 class="font-display font-bold text-3xl text-vs-text mb-2">Forgot Password</h1>
                    <p class="text-vs-dim text-sm">Enter your email and we'll send you a reset link.</p>
                </div>
                <div class="rounded-2xl bg-vs-card border border-vs-border p-8 shadow-2xl">
                    <div id="fp-success" class="hidden mb-4 p-4 rounded-xl bg-vs-success/10 border border-vs-success/30 text-sm text-vs-success flex items-center gap-2">
                        <i class="fas fa-check-circle"></i>
                        <span>If that email is registered, a reset link has been sent. Check your inbox.</span>
                    </div>
                    <form id="forgot-pw-form" class="space-y-4">
                        <div>
                            <label class="form-label">Email Address</label>
                            <input type="email" id="fp-email" class="input-field" placeholder="you@example.com" required autocomplete="email">
                        </div>
                        <button type="submit" id="fp-submit" class="btn btn-primary w-full py-3 text-base font-bold">
                            <span id="fp-btn-text">Send Reset Link</span>
                            <i class="fas fa-spinner fa-spin hidden" id="fp-spinner"></i>
                        </button>
                    </form>
                    <p class="text-center text-sm text-vs-muted mt-6">
                        Remember your password? <a href="#/login" class="text-vs-accent hover:underline">Sign in</a>
                    </p>
                </div>
            </div>
        </div>`;
    }

    return {
        render() { return render(); },
        init() {
            document.getElementById('forgot-pw-form')?.addEventListener('submit', async (e) => {
                e.preventDefault();
                const email = document.getElementById('fp-email').value.trim();
                if (!email) return;

                const btn = document.getElementById('fp-submit');
                const txt = document.getElementById('fp-btn-text');
                const spinner = document.getElementById('fp-spinner');
                btn.disabled = true;
                txt.textContent = 'Sending...';
                spinner.classList.remove('hidden');

                try {
                    await api.post('/account/forgot-password', { email });
                    document.getElementById('forgot-pw-form').classList.add('hidden');
                    document.getElementById('fp-success').classList.remove('hidden');
                } catch (err) {
                    // Always show success to avoid email enumeration
                    document.getElementById('forgot-pw-form').classList.add('hidden');
                    document.getElementById('fp-success').classList.remove('hidden');
                } finally {
                    btn.disabled = false;
                    txt.textContent = 'Send Reset Link';
                    spinner.classList.add('hidden');
                }
            });
        }
    };
})();