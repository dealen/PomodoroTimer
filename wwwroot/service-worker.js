const CACHE_NAME = 'pomodoro-timer-v1';
const baseHref = document.getElementsByTagName('base')[0]?.href ?? '/PomodoroTimer/';
const urlsToCache = [
  '/PomodoroTimer/',
  '/PomodoroTimer/css/app.css',
  '/PomodoroTimer/manifest.json',
  '/PomodoroTimer/icon-192.png',
  '/PomodoroTimer/icon-512.png',
  '/PomodoroTimer/presets.json'
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
