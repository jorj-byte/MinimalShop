// Persists the shopping cart in localStorage and syncs it between browser tabs.
// The cart itself lives in the .NET CartService; this script is its storage layer.
(function () {
    'use strict';

    var KEY = 'minimalshop.cart.v1';
    var storageHandler = null;

    function parse(raw) {
        try {
            var parsed = JSON.parse(raw);
            return Array.isArray(parsed) ? parsed : [];
        } catch {
            return [];
        }
    }

    function load() {
        try {
            var raw = window.localStorage.getItem(KEY);
            return raw ? parse(raw) : [];
        } catch {
            return []; // storage unavailable (e.g. private mode) -> memory-only cart
        }
    }

    function save(lines) {
        try {
            window.localStorage.setItem(KEY, JSON.stringify(lines || []));
        } catch {
            // ignore quota/unavailable storage errors
        }
    }

    function remove() {
        try {
            window.localStorage.removeItem(KEY);
        } catch {
            // ignore
        }
    }

    function watch(dispatcher) {
        unwatch();
        storageHandler = function (event) {
            if (!event || event.key !== KEY) return;
            var lines = event.newValue ? parse(event.newValue) : [];
            try {
                dispatcher.invokeMethodAsync('OnStorageChanged', lines)
                    .catch(function () { /* circuit may be gone */ });
            } catch {
                // dispatcher already disposed
            }
        };
        window.addEventListener('storage', storageHandler);
    }

    function unwatch() {
        if (storageHandler) {
            window.removeEventListener('storage', storageHandler);
            storageHandler = null;
        }
    }

    window.cartStorage = { load: load, save: save, remove: remove, watch: watch, unwatch: unwatch };
})();
