using CommandLine;

namespace TimetablePlus;

public sealed class Options
{
    [Option('l', "use local",
        HelpText = "Uses the local version of the timetable instead of downloading it from the internet")]
    public bool UseLocal { get; set; } = false;

    [Option('d', "save", 
        HelpText = "Save the timetable used by the OCR to the desktop")]
    public bool SaveTimetable { get; set; } = false;

    [Option('w', "white mode", 
        HelpText = "Generates a bright/light version of the timetable")]
    public bool WhiteMode { get; set; } = false;
}