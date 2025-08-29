using System;
using System.Threading.Tasks;
using Xunit;

public class TimerServiceTests
{
    [Fact]
    public async Task StartPauseResumeCycleProgresses()
    {
        var svc = new TimerService();
        var config = new PomodoroConfig { Work = TimeSpan.FromSeconds(1), Break = TimeSpan.FromSeconds(1), Cycles = 1 };

        TimerUpdate? last = null;
        svc.OnTick += update => last = update;

        svc.Start(config);
        Assert.True(last?.Phase == TimerPhase.Work);

        // wait half the work duration
        await Task.Delay(600);
        svc.Pause();
        Assert.False(last?.IsRunning);
        var remainingAfterPause = last?.Remaining;

        svc.Resume();
        Assert.True(last?.IsRunning);

        // wait enough for the work to finish
        await Task.Delay(1500);
        Assert.True(last?.Phase == TimerPhase.Completed || last?.Phase == TimerPhase.Break);
    }
}
