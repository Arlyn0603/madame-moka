const CACHE_NAME = "madame-moka-cache-v1";
const urlsToCache = [
    "/",
    "/css/styles.css",
    "/css/stylesipmrc.css",
    "/css/stylesmyp.css",
    "/css/stylesrye.css",
    "/icons/icon-192.png",
    "/icons/icon-512.png"
];


// Instalar service worker y guardar archivos en caché
self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                return cache.addAll(urlsToCache);
            })
            .catch(error => {
                console.error("Error al agregar archivos al caché:", error);
            })
    );
    self.skipWaiting();
});


// Activar y limpiar caché vieja
self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(
                keys.map(key => {
                    if (key !== CACHE_NAME) {
                        return caches.delete(key);
                    }
                })
            )
        )
    );
    self.clients.claim();
});

// Interceptar requests y responder desde caché o red
self.addEventListener("fetch", event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});
