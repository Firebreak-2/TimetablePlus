using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace TimetablePlus;

public enum SchoolSubject
{
    English,
    Math,
    Physics,
    Biology,
    Chemistry,
    PhysicalEducation,
    Arabic,
    Islamic,
    MoralEducation,
    Computer,
    Business,
    Elective,
    Unknown
}

public static class SchoolSubjectImplementations
{
    public static Argb32 GetColor(this SchoolSubject subject, Config config, Options options)
    {
        if (options.Monochrome)
            return Color.ParseHex(config.ForegroundColor).ToPixel<Argb32>();
        
        if (config.ColorMapping.TryGetValue(subject, out string? hexCol)
            && hexCol is not null)
        {
            hexCol = hexCol.Length > 0 && hexCol[0] == '#' ? hexCol[1..] : hexCol;
        }
        else
        {
            hexCol = Config.Default.ColorMapping[SchoolSubject.Unknown]!;
        }

        return Color.ParseHex(hexCol);
    }

    public static string GetNickname(this SchoolSubject subject)
    {
        return subject switch
        {
            SchoolSubject.English => "Eng",
            SchoolSubject.Math => "Math",
            SchoolSubject.Physics => "Phys",
            SchoolSubject.Biology => "Bio",
            SchoolSubject.Chemistry => "Chem",
            SchoolSubject.PhysicalEducation => "PE",
            SchoolSubject.Arabic => "Arab",
            SchoolSubject.Islamic => "Islam",
            SchoolSubject.MoralEducation => "Moral",
            SchoolSubject.Computer => "Comp",
            SchoolSubject.Business => "Busin",
            SchoolSubject.Elective => "Elect",
            _ => "???"
        };
    }
}