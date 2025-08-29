using System;

public class PomodoroConfig
{
    public string? Name { get; set; }
    public TimeSpan Work { get; set; }
    public TimeSpan Break { get; set; }
    public TimeSpan? LongBreak { get; set; }
    public int LongBreakInterval { get; set; } = 4;
    public int Cycles { get; set; } = 4;
}
