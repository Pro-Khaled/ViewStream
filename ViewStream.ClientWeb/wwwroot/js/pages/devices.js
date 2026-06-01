pages.devices = (() => {
    let state = { data: null, loading: true };

    async function load() {
        state.loading = true; render();
        try {
            state.data = await api.get('/users/me/devices');
        } catch (err) {
            toast.error('Failed to load devices: ' + err.message);
            state.data = null;
        }
        state.loading = false; render();
    }

    async function registerDevice(e) {
        e.preventDefault();
        const name = document.getElementById('new-device-name').value.trim();
        const id = document.getElementById('new-device-id').value.trim();
        const platform = document.getElementById('new-device-platform').value;

        if (!name || !id) {
            toast.warn('Device Name and Device ID are required');
            return;
        }

        try {
            await api.post('/users/me/devices', { deviceName: name, deviceId: id, platform });
            toast.success('Device registered successfully');
            modal.close();
            load();
        } catch (err) {
            toast.error('Failed to register device: ' + err.message);
        }
    }

    async function updateDevice(id, currentName, currentTrusted) {
        modal.open('Edit Device',
            `<form id="edit-device-form" class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Device Name</label>
                    <input class="input-field" id="edit-device-name" value="${toast.esc(currentName || '')}" required maxlength="100">
                </div>
                <div class="flex items-center gap-3">
                    <input type="checkbox" id="edit-device-trusted" class="form-checkbox" ${currentTrusted ? 'checked' : ''}>
                    <label for="edit-device-trusted" class="form-label mb-0 cursor-pointer">Trust this device</label>
                </div>
            </form>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="edit-device-save-btn">Save Changes</button>`
        );

        document.getElementById('edit-device-save-btn')?.addEventListener('click', async () => {
            const name = document.getElementById('edit-device-name').value.trim();
            const trusted = document.getElementById('edit-device-trusted').checked;
            if (!name) return toast.warn('Device Name is required');

            try {
                await api.put(`/users/me/devices/${id}`, { deviceName: name, isTrusted: trusted });
                toast.success('Device updated');
                modal.close();
                load();
            } catch (err) {
                toast.error('Failed to update device: ' + err.message);
            }
        });
    }

    async function deleteDevice(id) {
        if (!await modal.confirm('Remove Device', 'Are you sure you want to remove this device? You will be logged out on it.', 'danger')) return;
        try {
            await api.delete(`/users/me/devices/${id}`);
            toast.success('Device removed successfully');
            load();
        } catch (err) {
            toast.error('Failed to remove device: ' + err.message);
        }
    }

    function openRegisterModal() {
        modal.open('Register New Device',
            `<form id="register-device-form" class="space-y-4 font-body">
                <div class="form-group">
                    <label class="form-label">Device Name</label>
                    <input class="input-field" id="new-device-name" placeholder="e.g. My SmartTV, iPad Pro" required maxlength="100">
                </div>
                <div class="form-group">
                    <label class="form-label">Unique Device ID (Hardware ID)</label>
                    <input class="input-field font-mono" id="new-device-id" placeholder="e.g. tracking-uuid-or-serial" required maxlength="255">
                </div>
                <div class="form-group">
                    <label class="form-label">Platform</label>
                    <select class="input-field" id="new-device-platform">
                        <option value="Web">Web Browser</option>
                        <option value="iOS">iOS Device</option>
                        <option value="Android">Android Device</option>
                        <option value="SmartTV">Smart TV</option>
                        <option value="Xbox">Xbox</option>
                        <option value="PlayStation">PlayStation</option>
                    </select>
                </div>
            </form>`,
            `<button class="btn btn-secondary" onclick="modal.close()">Cancel</button>
             <button class="btn btn-primary" id="register-device-btn">Register Device</button>`
        );
        document.getElementById('register-device-btn')?.addEventListener('click', registerDevice);
    }

    function platformIcon(platform) {
        const p = (platform || '').toLowerCase();
        if (p.includes('ios') || p.includes('apple') || p.includes('ipad') || p.includes('iphone')) return 'fab fa-apple text-xl';
        if (p.includes('android')) return 'fab fa-android text-xl text-green-500';
        if (p.includes('tv')) return 'fas fa-tv text-xl text-vs-accent';
        if (p.includes('xbox') || p.includes('playstation') || p.includes('console')) return 'fas fa-gamepad text-xl text-purple-400';
        return 'fas fa-laptop text-xl text-muted';
    }

    function render() {
        const c = document.getElementById('devices-content');
        if (!c) return;

        let h = Comp.pageHeader('Registered Devices', 'Manage the smartphones, tablets, TVs, and computers connected to your ViewStream account.',
            `<button class="btn btn-primary btn-sm" id="reg-device-modal-btn"><i class="fas fa-plus mr-1.5"></i> Register Device</button>`);

        if (state.loading) {
            h += Comp.pageLoader();
        } else if (!state.data || state.data.length === 0) {
            h += Comp.emptyState('fa-laptop-house', 'No registered devices found. Register this device to get started!');
        } else {
            h += `<div class="grid grid-cols-1 md:grid-cols-2 gap-6 font-body">
                ${state.data.map(d => `
                    <div class="card p-5 relative border border-vs-border hover:border-vs-accent/40 transition-all shadow-md group">
                        <div class="flex items-start gap-4">
                            <div class="w-12 h-12 rounded-xl bg-vs-surface-2 flex items-center justify-center flex-shrink-0">
                                <i class="${platformIcon(d.platform)}"></i>
                            </div>
                            <div class="flex-1 min-w-0">
                                <div class="flex items-center gap-2 flex-wrap">
                                    <h3 class="font-bold text-lg text-vs-text truncate" title="${toast.esc(d.deviceName)}">${toast.esc(d.deviceName || 'Unnamed Device')}</h3>
                                    ${d.isTrusted ? '<span class="badge badge-success text-xs font-semibold py-0.5"><i class="fas fa-shield-alt mr-1"></i> Trusted</span>' : ''}
                                </div>
                                <p class="text-xs text-vs-muted mt-1 font-mono">Platform: ${toast.esc(d.platform || 'Unknown')}</p>
                                <p class="text-xs text-vs-muted mt-0.5">Last active: ${d.lastActive ? utils.formatDate(d.lastActive) : 'Never'}</p>
                            </div>
                        </div>
                        <div class="flex justify-end gap-2 border-t border-vs-border pt-3 mt-4">
                            <button class="btn btn-ghost btn-xs text-vs-accent edit-d-btn" data-id="${d.id}" data-name="${toast.esc(d.deviceName)}" data-trusted="${d.isTrusted ? 'true' : 'false'}"><i class="fas fa-edit mr-1"></i> Edit</button>
                            <button class="btn btn-ghost btn-xs text-vs-danger del-d-btn" data-id="${d.id}"><i class="fas fa-trash mr-1"></i> Remove</button>
                        </div>
                    </div>
                `).join('')}
            </div>`;
        }

        c.innerHTML = h;

        document.getElementById('reg-device-modal-btn')?.addEventListener('click', openRegisterModal);
        document.querySelectorAll('.edit-d-btn').forEach(btn => btn.addEventListener('click', () => {
            updateDevice(parseInt(btn.dataset.id), btn.dataset.name, btn.dataset.trusted === 'true');
        }));
        document.querySelectorAll('.del-d-btn').forEach(btn => btn.addEventListener('click', () => {
            deleteDevice(parseInt(btn.dataset.id));
        }));
    }

    return {
        render() { return '<div id="devices-content">' + Comp.pageLoader() + '</div>'; },
        init() { load(); }
    };
})();
