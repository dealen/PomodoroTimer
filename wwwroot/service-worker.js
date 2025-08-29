const CACHE_NAME = 'pomodoro-timer-v1';
const urlsToCache = [
  '/',
  '/css/app.css',
  '/manifest.json',
  '/icon-192.png',
  '/icon-512.png',
  '/presets.json'
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(urlsToCache))
  );
});

self.addEventListener('fetch', event => {
  event.respondWith(
    caches.match(event.request)
      .then(response => {
        // Return cached version or fetch from network
        return response || fetch(event.request);
      }
    )
  );
});
