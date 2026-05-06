pages.notFound = {
    render() {
        return `<div class="flex flex-col items-center justify-center py-32 text-center">
            <div class="w-24 h-24 rounded-full bg-vs-card flex items-center justify-center mb-6">
                <i class="fas fa-question text-4xl text-vs-muted"></i>
            </div>
            <h1 class="font-display font-black text-5xl text-vs-text mb-3">404</h1>
            <p class="text-lg text-vs-muted mb-8 max-w-md">The page you're looking for doesn't exist or has been moved.</p>
            <a href="#/" class="px-6 py-3 bg-vs-accent hover:bg-vs-accentHover text-vs-bg font-bold rounded-xl transition-colors">
                <i class="fas fa-home mr-2"></i>Go Home
            </a>
        </div>`;
    },
    init() { }
};