using System.ComponentModel;

namespace Core.Enums.Leads
{
    public enum ContactStatus
    {
        [Description("Not Contacted Yet")]
        NotContactedYet = 0,
        [Description("Contacted With No Reply")]
        ContactedWithNoReply = 1,
        [Description("Contacted And Replied")]
        ContactedAndReplied = 2,
        [Description("Wrong Number")]
        WrongNumber = 3
    }
}
