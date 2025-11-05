using System;

public readonly struct TimeRange
{
    public TimeSpan Start { get; }
    public TimeSpan End { get; }
    public TimeRange(TimeSpan start, TimeSpan end)
    {
        if (end <= start) throw new ArgumentException("End must be greater than Start");
        Start = start; End = end;
    }
    public bool Contains(TimeSpan t) => t >= Start && t < End;
    public override string ToString() => $"[{Start}-{End})";
}
