using System.ComponentModel;

namespace Core.Enums.Leads
{
    public enum LeadResponsibility
    {
        [Description("Responsible - Decision Maker")]
        Responsible_DecisionMaker = 0,
        [Description("Responsible - NOT Decision Maker")]
        Responsible_NOT_DecisionMaker = 1,
        [Description("Not Responsible")]
        NotResponsible = 2
    }
}
