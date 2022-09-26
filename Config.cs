using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TimetablePlus;

public record Config
{
    public record Class(Dictionary<SchoolSubject, string> Teachers);

    public int DefaultPeriodLength { get; set; }
    public TimeSpan StartTime { get; set; }
    public Dictionary<int, int> BreakMapping { get; set; }
    public Dictionary<int, int> TimeModMapping { get; set; }
    public string[] WeekdayNames { get; set; }
    public Dictionary<string, SchoolSubject> RegexSubjectMappings { get; set; }
    public Dictionary<string, Class> ClassBindings { get; set; }
    public Dictionary<SchoolSubject, string?> ColorMapping { get; set; }

    public static Config Default => new()
    {
        DefaultPeriodLength = 45,
        StartTime = TimeSpan.FromHours(8), // <=> TimeSpan.Parse("08:00:00"),
        BreakMapping = new Dictionary<int, int>
        {
            {3, 30},
            {5, 10},
        },
        TimeModMapping = new Dictionary<int, int>
        {
            {6, 40},
            {7, 35},
        },
        RegexSubjectMappings = new Dictionary<string, SchoolSubject>
        {
            {@"eng", SchoolSubject.English},
            {@"math", SchoolSubject.Math},
            {@"phys", SchoolSubject.Physics},
            {@"bio", SchoolSubject.Biology},
            {@"chem", SchoolSubject.Chemistry},
            {@"pe", SchoolSubject.PhysicalEducation},
            {@"ara", SchoolSubject.Arabic},
            {@"isl", SchoolSubject.Islamic},
            {@"moral", SchoolSubject.MoralEducation},
            {@"comp", SchoolSubject.Computer},
            {@"bus", SchoolSubject.Business},
            {@"ele", SchoolSubject.Elective},
        },
        WeekdayNames = new[]
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
        },
        ClassBindings = new Dictionary<string, Class>
        {
            {
                "12B", new Class(new Dictionary<SchoolSubject, string>
                {
                    {SchoolSubject.English, "Tala'at"},
                    {SchoolSubject.Math, "Amin"},
                    {SchoolSubject.Physics, "Rasheed"},
                    {SchoolSubject.Biology, "Zaina"},
                    {SchoolSubject.Chemistry, "Riham"},
                    {SchoolSubject.PhysicalEducation, "Mohammed"},
                    {SchoolSubject.Arabic, "Abd Al Salam"},
                    {SchoolSubject.Islamic, "Motaz"},
                    {SchoolSubject.MoralEducation, "Eman"},
                    {SchoolSubject.Computer, "Nasser"},
                    {SchoolSubject.Business, "Nadine"},
                    {SchoolSubject.Elective, "Varies"},
                })
            }
        },
        ColorMapping = new Dictionary<SchoolSubject, string?>
        {
            { SchoolSubject.English, Color.Yellow.ToHex()[..^2] },
            { SchoolSubject.Math, Color.Orange.ToHex()[..^2] },
            { SchoolSubject.Physics, Color.Cyan.ToHex()[..^2] },
            { SchoolSubject.Biology, Color.DarkBlue.ToHex()[..^2] },
            { SchoolSubject.Chemistry, Color.DarkGreen.ToHex()[..^2] },
            { SchoolSubject.PhysicalEducation, Color.Grey.ToHex()[..^2] },
            { SchoolSubject.Arabic, Color.SaddleBrown.ToHex()[..^2] },
            { SchoolSubject.Islamic, Color.MediumPurple.ToHex()[..^2] },
            { SchoolSubject.MoralEducation, Color.LightGreen.ToHex()[..^2] },
            { SchoolSubject.Computer, Color.Purple.ToHex()[..^2] },
            { SchoolSubject.Business, Color.Coral.ToHex()[..^2] },
            { SchoolSubject.Elective, Color.WhiteSmoke.ToHex()[..^2] },
            { SchoolSubject.Unknown, Color.Black.ToHex()[..^2] },
        }
    };
}