using System.Text.RegularExpressions;

namespace TimetablePlus;

public record Config
{
    public int DefaultPeriodLength { get; set; }
    public TimeSpan StartTime { get; set; }
    public Dictionary<int, int> BreakMapping { get; set; }
    public Dictionary<int, int> TimeModMapping { get; set; }
    public string[] WeekdayNames { get; set; }
    public Dictionary<string, SchoolSubject> RegexSubjectMappings { get; set; }

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
        WeekdayNames = new[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
        }
    };

}