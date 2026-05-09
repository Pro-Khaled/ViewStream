pages.resetPassword = (() => {
    let params = {};

    function render() {
        return `<div id="reset-pw-page" class="min-h-screen flex items-center justify-center px-4" style="background:radial-gradient(ellipse at 60% 20%,#1a1230 0%,#0d0d12 70%)">
            <div style="width:100%;max-width:420px">
                <div class="text-center mb-8">
                    <a href="#/" class="inline-flex items-center gap-2 text-vs-accent font-display font-black text-2xl mb-6 hover:opacity-80 transition-opacity">
                        <i class="fas fa-play-circle"></i> ViewStream
                    </a>
                    <h1 class="font-display font-bold text-3xl text-vs-text mb-2">Reset Password</h1>
                    <p class="text-vs-dim text-sm">Enter a strong new password for your account.</p>
                </div>
                <div class="rounded-2xl bg-vs-card border border-vs-border p-8 shadow-2xl">
                    <div id="rp-error" class="hidden mb-4 p-4 rounded-xl bg-vs-error/10 border border-vs-error/30 text-sm text-vs-error"></div>
                    <form id="reset-pw-form" class="space-y-4">
                        <div>
                            <label class="form-label">New Password</label>
                            <input type="password" id="rp-password" class="input-field" placeholder="Minimum 8 characters" required minlength="8" autocomplete="new-password">
                        </div>
                        <div>
                            <label class="form-label">Confirm New Password</label>
                            <input type="password" id="rp-confirm" class="input-field" placeholder="Repeat your password" required minlength="8" autocomplete="new-password">
                        </div>
                        <div id="rp-strength" class="text-xs text-vs-muted hidden">
                            Password strength: <span id="rp-strength-label" class="font-semibold"></span>
                        </div>
                        <button type="submit" id="rp-submit" class="btn btn-primary w-full py-3 text-base font-bold">
                            <span id="rp-btn-text">Reset Password</span>
                            <i class="fas fa-spinner fa-spin hidden" id="rp-spinner"></i>
                        </button>
                    </form>
                    <p class="text-center text-sm text-vs-muted mt-6">
                        <a href="#/login" class="text-vs-accent hover:underline">Back to Sign in</a>
                    </p>
                </div>
            </div>
        </div>`;
    }

    function getStrength(pw) {
        let score = 0;
        if (pw.length >= 8) score++;
        if (/[A-Z]/.test(pw)) score++;
        if (/[0-9]/.test(pw)) score++;
        if (/[^A-Za-z0-9]/.test(pw)) score++;
        if (pw.length >= 12) score++;
        return score;
    }

    return {
        render() { return render(); },
        init(p) {
            params = p || router.getQueryParams();

            // Password strength indicator
            document.getElementById('rp-password')?.addEventListener('input', (e) => {
                const pw = e.target.value;
                const strengthEl = document.getElementById('rp-strength');
                const labelEl = document.getElementById('rp-strength-label');
                if (!pw) { strengthEl.classList.add('hidden'); return; }
                strengthEl.classList.remove('hidden');
                const s = getStrength(pw);
                const levels = ['Very Weak', 'Weak', 'Fair', 'Good', 'Strong'];
                const colors = ['text-vs-error', 'text-vs-error', 'text-yellow-400', 'text-vs-success', 'text-vs-success'];
                labelEl.textContent = levels[Math.min(s, 4)];
                labelEl.className = `font-semibold ${colors[Math.min(s, 4)]}`;
            });

            document.getElementById('reset-pw-form')?.addEventListener('submit', async (e) => {
                e.preventDefault();
                const password = document.getElementById('rp-password').value;
                const confirm = document.getElementById('rp-confirm').value;
                const errorEl = document.getElementById('rp-error');

                errorEl.classList.add('hidden');

                if (password !== confirm) {
                    errorEl.textContent = 'Passwords do not match.';
                    errorEl.classList.remove('hidden');
                    return;
                }

                if (!params.userId || !params.token) {
                    errorEl.textContent = 'Invalid reset link. Please request a new one.';
                    errorEl.classList.remove('hidden');
                    return;
                }

                const btn = document.getElementById('rp-submit');
                const txt = document.getElementById('rp-btn-text');
                const spinner = document.getElementById('rp-spinner');
                btn.disabled = true;
                txt.textContent = 'Resetting...';
                spinner.classList.remove('hidden');

                try {
                    await api.post('/account/reset-password', {
                        userId: parseInt(params.userId),
                        token: params.token,
                        newPassword: password,
                        confirmPassword: confirm
                    });
                    toast.success('Password reset! Please sign in with your new password.');
                    router.navigate('/login');
                } catch (err) {
                    errorEl.textContent = err.message || 'Reset failed. The link may have expired.';
                    errorEl.classList.remove('hidden');
                } finally {
                    btn.disabled = false;
                    txt.textContent = 'Reset Password';
                    spinner.classList.add('hidden');
                }
            });
        }
    };
})();
