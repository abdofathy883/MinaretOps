using System.ComponentModel;

namespace Core.Enums
{
    public enum EmployeeType
    {
        [Description("Full Time")]
        FullTime = 0,
        [Description("Part Time")]
        PartTime = 1,
        [Description("Freelance")]
        Freelance = 2
    }
}
