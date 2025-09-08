/* Configurações adicionais para o Blazor */

// Configuração global para evitar erros de carregamento duplo
window.addEventListener('DOMContentLoaded', function() {
    // Configurações de retry para conexão SignalR
    if (window.Blazor && window.Blazor.defaultReconnectionHandler) {
        window.Blazor.defaultReconnectionHandler._reconnectCallback = function() {
            // Evitar múltiplas tentativas simultâneas
            return new Promise(resolve => setTimeout(resolve, 2000));
        };
    }
});

// Configuração de error handling global
window.addEventListener('error', function(e) {
    if (e.message && e.message.includes('blazor-ssr-end')) {
        // Suprimir erro conhecido do Blazor SSR
        e.preventDefault();
        return false;
    }
});

// Configuração de unhandled promise rejection
window.addEventListener('unhandledrejection', function(e) {
    if (e.reason && e.reason.toString().includes('blazor-ssr-end')) {
        // Suprimir erro conhecido do Blazor SSR
        e.preventDefault();
        return false;
    }
});
