using System.ComponentModel;

namespace Core.Enums.Leads
{
    public enum LeadBudget
    {
        [Description("Below")]
        Below = 0,
        [Description("Equal")]
        Equal = 1,
        [Description("Higher")]
        Higher = 2
    }
}
