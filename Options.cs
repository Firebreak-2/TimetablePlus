using CommandLine;

namespace TimetablePlus;

public sealed class Options
{
    [Value(0, 
        HelpText = "The class to parse the timetable of (case sensitive)")]
    public string ClassID { get; set; }
    [Option('l', "use local",
        HelpText = "Uses the local version of the timetable " +
                   "instead of downloading it from the internet")]
    public bool UseLocal { get; set; } = false;

    [Option('d', "save", 
        HelpText = "Save the timetable used by the OCR to the desktop")]
    public bool SaveTimetable { get; set; } = false;

    [Option('m', "monochrome",
        HelpText = "Ignores all subject colorings and instead uses the default foreground color")]
    public bool Monochrome { get; set; } = false;
}