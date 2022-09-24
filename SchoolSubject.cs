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
    public static Argb32 GetColor(this SchoolSubject subject)
    {
        return subject switch
        {
            SchoolSubject.English => Color.Yellow,
            SchoolSubject.Math => Color.Orange,
            SchoolSubject.Physics => Color.Cyan,
            SchoolSubject.Biology => Color.DarkBlue,
            SchoolSubject.Chemistry => Color.DarkGreen,
            SchoolSubject.PhysicalEducation => Color.Grey,
            SchoolSubject.Arabic => Color.SaddleBrown,
            SchoolSubject.Islamic => Color.MediumPurple,
            SchoolSubject.MoralEducation => Color.LightGreen,
            SchoolSubject.Computer => Color.Purple,
            SchoolSubject.Business => Color.Coral,
            SchoolSubject.Elective => Color.WhiteSmoke,
            _ => Color.Black
        };
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
            _ => "N/A"
        };
    }
}