using System;
using System.Timers;
using System.Threading.Tasks;

public enum TimerPhase
{
    Idle,
    Work,
    Break,
    LongBreak,
    Completed,
}

public record TimerUpdate(TimerPhase Phase, TimeSpan Remaining, int CurrentCycle, bool IsRunning);

public class TimerService : IDisposable
{
    private readonly System.Timers.Timer _tickTimer;
    private DateTimeOffset? _phaseEnd;
    private TimeSpan _remainingOnPause = TimeSpan.Zero;
    private PomodoroConfig? _config;
    private int _currentCycle = 0;
    private TimerPhase _phase = TimerPhase.Idle;
    private bool _isRunning = false;

    public event Action<TimerUpdate>? OnTick;

    public TimerService()
    {
    _tickTimer = new System.Timers.Timer(250);
        _tickTimer.Elapsed += Tick;
    }

    public void Start(PomodoroConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        if (config.Work <= TimeSpan.Zero) throw new ArgumentException("Work duration must be > 0", nameof(config));

        _config = config;
        _currentCycle = 1;
        StartPhase(TimerPhase.Work, config.Work);
    }

    private void StartPhase(TimerPhase phase, TimeSpan duration)
    {
        _phase = phase;
        _phaseEnd = DateTimeOffset.UtcNow.Add(duration);
        _isRunning = true;
        _tickTimer.Start();
        RaiseTick();
    }

    public void Pause()
    {
        if (!_isRunning || _phase == TimerPhase.Idle) return;
        _remainingOnPause = _phaseEnd.HasValue ? (_phaseEnd.Value - DateTimeOffset.UtcNow) : TimeSpan.Zero;
        _tickTimer.Stop();
        _isRunning = false;
        RaiseTick();
    }

    public void Resume()
    {
        if (_isRunning || _phase == TimerPhase.Idle) return;
        _phaseEnd = DateTimeOffset.UtcNow.Add(_remainingOnPause);
        _isRunning = true;
        _tickTimer.Start();
        RaiseTick();
    }

    public void Reset()
    {
        _tickTimer.Stop();
        _phase = TimerPhase.Idle;
        _isRunning = false;
        _phaseEnd = null;
        _remainingOnPause = TimeSpan.Zero;
        _config = null;
        _currentCycle = 0;
        RaiseTick();
    }

    private void Tick(object? sender, ElapsedEventArgs e)
    {
        if (!_isRunning || _phase == TimerPhase.Idle) return;
        if (!_phaseEnd.HasValue) return;

        var remaining = _phaseEnd.Value - DateTimeOffset.UtcNow;
        if (remaining <= TimeSpan.Zero)
        {
            // phase complete
            OnPhaseComplete();
        }
        else
        {
            RaiseTick();
        }
    }

    private void OnPhaseComplete()
    {
        // stop briefly to avoid overlapping ticks
        _tickTimer.Stop();
        if (_config == null) { Reset(); return; }

        if (_phase == TimerPhase.Work)
        {
            // decide if long break or short break
            if (_config.LongBreak.HasValue && _config.LongBreakInterval > 0 && _currentCycle % _config.LongBreakInterval == 0)
            {
                StartPhase(TimerPhase.LongBreak, _config.LongBreak.Value);
                return;
            }
            else
            {
                StartPhase(TimerPhase.Break, _config.Break);
                return;
            }
        }
        else if (_phase == TimerPhase.Break || _phase == TimerPhase.LongBreak)
        {
            // if cycles complete, finish
            if (_currentCycle >= _config.Cycles)
            {
                _phase = TimerPhase.Completed;
                _isRunning = false;
                _phaseEnd = null;
                RaiseTick();
                return;
            }
            else
            {
                _currentCycle++;
                StartPhase(TimerPhase.Work, _config.Work);
                return;
            }
        }
    }

    private void RaiseTick()
    {
        var remaining = TimeSpan.Zero;
        if (_isRunning && _phaseEnd.HasValue)
        {
            remaining = _phaseEnd.Value - DateTimeOffset.UtcNow;
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
        }
        else if (!_isRunning && _phase != TimerPhase.Idle && _phaseEnd.HasValue)
        {
            remaining = _remainingOnPause;
        }

        OnTick?.Invoke(new TimerUpdate(_phase, remaining, _currentCycle, _isRunning));
    }

    public TimerUpdate GetState()
    {
        var remaining = TimeSpan.Zero;
        if (_phaseEnd.HasValue && _isRunning)
        {
            remaining = _phaseEnd.Value - DateTimeOffset.UtcNow;
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
        }
        else if (!_isRunning && _phaseEnd.HasValue)
        {
            remaining = _remainingOnPause;
        }

        return new TimerUpdate(_phase, remaining, _currentCycle, _isRunning);
    }

    public void Dispose()
    {
        _tickTimer.Dispose();
    }
}
