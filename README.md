# Pomodoro Timer

A modern, configurable Pomodoro timer web app built with Blazor WebAssembly. Installable as a PWA on mobile devices.

## Features

✅ **Core Timer Functionality**
- Start, pause, resume, and reset timer sessions
- Automatic phase transitions (work → break → long break)
- Configurable work/break/long break durations
- Customizable number of cycles

✅ **Configuration & Presets**
- Built-in presets (Classic Pomodoro, Quick Focus)
- Custom session configuration
- Persistent settings (saved to localStorage)
- Enable/disable long breaks

✅ **Progressive Web App (PWA)**
- Installable on mobile devices
- Offline functionality with service worker
- Native app-like experience

✅ **Notifications & Alerts**
- Browser notifications for phase transitions
- Permission-based notification system
- Visual and audio feedback

✅ **Modern UI**
- Responsive design for mobile/desktop
- Beautiful gradient timer display
- Intuitive controls and phase indicators

## How to run

```bash
cd /home/dealen/Dev/PomodoroTimer
dotnet restore
dotnet build
dotnet run
```

Open the URL printed by `dotnet run` (usually `https://localhost:5001`) to use the app.

## Installation as PWA

1. Open the app in a mobile browser (Chrome, Safari, etc.)
2. Look for "Add to Home Screen" or "Install App" option
3. Follow the prompts to install
4. Access the app like a native mobile app

## Usage

1. **Choose a preset** or configure custom durations
2. **Click Start** to begin your Pomodoro session
3. **Focus during work phases**, relax during breaks
4. **Get notified** when phases transition
5. **Complete cycles** to finish your session

## Development

The app includes:
- `TimerService` — Core timer logic and state management
- `LocalStorageService` — Persistent configuration storage
- `NotificationService` — Browser notification integration
- Component-based UI with Blazor
- xUnit tests for core timer functionality

## Testing

```bash
dotnet test ./PomodoroTimer.Tests
```

Built with ❤️ using Blazor WebAssembly and .NET 9
