# Pomodoro Timer — Implementation Plan

Brief: Plan to build a Pomodoro timer web app using the .NET stack, targeted for phone use (PWA) with an option to pivot to .NET MAUI later.

## Current Status & Updated Checklist
```markdown
- [x] Project scaffolding: Blazor WASM project created (net9.0)
- [x] TimerService: core state machine implemented (Start/Pause/Resume/Reset)
- [x] Basic UI: enhanced timer display with modern styling
- [x] Create README with run steps
- [x] Add minimal unit tests for timer logic
- [x] Config: set work period length (ConfigEditor component)
- [x] Config: set break period length (ConfigEditor component)
- [x] Config: set (or disable) longer break in the middle (ConfigEditor component)
- [x] Choose pre-saved configurations (PresetsList component loads from JSON)
- [x] Persist user config to localStorage (LocalStorageService implemented)
- [x] PWA manifest and service worker for installability
- [x] Notifications when phases complete
- [x] Save custom user configurations as presets (ConfigStorageService implemented)
- [ ] Add more comprehensive tests
- [ ] Sound alerts (optional enhancement)
- [ ] Enhanced mobile optimization
```

## Next Priority: Testing & Polish
Core functionality now complete! Next priorities:

1. **Enhanced testing** - more comprehensive TimerService tests, edge cases
2. **Sound alerts** - optional audio notifications for phase transitions  
3. **Performance optimization** - ensure smooth experience on mobile
4. **Documentation** - update README with features and installation guide

## Architecture & technologies
- Frontend: Blazor WebAssembly (net9.0) with PWA manifest
- State: TimerService (singleton DI service) ✅ IMPLEMENTED
- Storage: JSON file for shipped presets; user-selected presets persisted in browser localStorage
- Tests: xUnit for timer logic ✅ BASIC TESTS ADDED

## JSON presets schema (example)
```json
{
  "presets": [
    {
      "id": "classic",
      "name": "Classic Pomodoro",
      "workMinutes": 25,
      "breakMinutes": 5,
      "longBreakMinutes": 15,
      "longBreakInterval": 4,
      "cycles": 4
    }
  ]
}
```

Store this file at `wwwroot/presets.json` for the web app to fetch.

## Timer state machine (recommended)
States:
- Idle (not started)
- Running.Work
- Running.Break
- Running.LongBreak
- Paused (with sub-state of which running phase was paused)
- Completed

Transitions:
- Idle -> Running.Work (Start)
- Running.* -> Paused (Pause)
- Paused -> Running.* (Resume)
- Running.* -> Running.* (phase-complete -> next phase)
- Any -> Idle (Reset)

Timekeeping strategy:
- On Start: store absolute end timestamp for current phase (UTC Now + phaseDuration)
- On Tick or visibility change: compute remaining = endTimestamp - UTC Now
- On Pause: save remaining duration, clear active endTimestamp
- On Resume: set new endTimestamp = UTC Now + remaining
This prevents drift and survives backgrounding.

## UI screens / components
- Main player view: shows current phase, large timer, controls (Start / Pause / Resume / Reset), progress (cycle count), next-phase hint
- Config editor: set work/break/long break lengths, enable/disable long break, longBreakInterval, cycles
- Presets list: load from `presets.json`, allow selecting and applying preset
- Settings (optional): sound/vibration toggles, notifications

## Implementation steps (milestones)
1. Project scaffolding: create Blazor WASM PWA project
2. Add `presets.json` to `wwwroot` and load on startup
3. Implement TimerService (state machine + timekeeping) with public API: Start(config), Pause(), Resume(), Reset(), Subscribe/OnTick
4. Create UI components: Player, ConfigEditor, PresetsList
5. Persist last-used config in `localStorage`
6. Add unit tests for TimerService (xUnit)
7. Make app a PWA and test install on phone
8. Polish: sounds, vibration, notifications, accessibility

## Minimal API / service surface
TimerService:
- Start(PomodoroConfig config)
- Pause()
- Resume()
- Reset()
- GetState(): TimerState
- OnTick: event or IObservable<TimerUpdate>

Data types:
- PomodoroConfig
  - string Name   
  - TimeSpan Work
  - TimeSpan Break
  - TimeSpan? LongBreak
  - int LongBreakInterval
  - int Cycles

- TimerUpdate
  - TimerPhase (Work/Break/LongBreak)
  - TimeSpan Remaining
  - int CurrentCycle
  - bool IsRunning

## Testing strategy
- Unit tests for TimerService: start -> tick -> phase transitions, pause/resume correctness, reset
- Edge tests: resume after long background pause, invalid config rejects start

## Quality gates
- Build: `dotnet build`
- Tests: `dotnet test`
- Lint/type: rely on compiler warnings; consider adding Roslyn analyzers later

## Commands to scaffold (quick start)
```bash
dotnet new blazorwasm -o PomodoroTimer --pwa
cd PomodoroTimer
# run
dotnet run
```

## Deliverables (MVP)
- Blazor WebAssembly PWA project with working timer and UI
- `wwwroot/presets.json` with sample presets
- Unit tests for core timer logic
- `README.md` with run instructions
- `PLAN.md` (this file)

## Timeline suggestion
- Day 0: scaffold + presets + TimerService core
- Day 1: UI player + start/pause/resume/reset wired
- Day 2: presets UI + persistence + tests + PWA tweaks

## Next steps (what I will do if you want me to continue)
- Scaffold project and implement TimerService with tests
- Create UI skeleton and wire player controls
- Add sample `presets.json` and README

---

*Prepared plan saved as `PLAN.md`.*
