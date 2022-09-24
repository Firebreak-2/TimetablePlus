namespace TimetablePlus;

public readonly record struct Duration(TimeSpan StartTime, TimeSpan EndTime)
{
    public TimeSpan Length => EndTime - StartTime;
}