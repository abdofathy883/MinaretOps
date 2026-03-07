using System.ComponentModel;

namespace Core.Enums.Leads
{
    public enum CurrentLeadStatus
    {
        [Description("New Lead")]
        NewLead = 0,
        [Description("First Call")]
        FirstCall = 1,
        [Description("Interested")]
        Interested = 2,
        [Description("Meeting Agreed")]
        MeetingAgreed = 3,
        [Description("Potential")]
        Potential = 4,
        [Description("Deal")]
        Deal = 5,
        [Description("Not Potential")]
        NotPotential = 6
    }
}
