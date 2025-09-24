using System.ComponentModel;

namespace Core.Enums
{
    public enum TaskType
    {
        [Description("Planning")]
        Planning = 0,
        [Description("Content Strategy")]
        ContentStrategy = 1,
        [Description("Content Writing")]
        ContentWriting = 2,
        [Description("Logo Design")]
        LogoDesign = 3,
        [Description("Visual Identity")]
        VisualIdentity = 4,
        [Description("Design Directions")]
        DesignDirections = 5,
        [Description("SM Design")]
        SM_Design = 6,
        [Description("Printings Design")]
        PrintingsDesign = 7,
        [Description("Illustrations")]
        Illustrations = 8,
        [Description("Voiceover")]
        Voiceover = 9,
        [Description("Motion")]
        Motion = 10,
        [Description("Video Editing")]
        VideoEditing = 11,
        [Description("Publishing")]
        Publishing = 12,
        [Description("Moderation")]
        Moderation = 13,
        [Description("Ad Management")]
        Ad_Management = 14,
        [Description("E-mail Marketing")]
        E_mailMarketing = 15,
        [Description("WhatsApp Marketing")]
        WhatsAppMarketing = 16,
        [Description("UI/UX")]
        UI_UX = 17,
        [Description("WordPress")]
        WordPress = 18,
        [Description("Backend")]
        Backend = 19,
        [Description("Frontend")]
        Frontend = 20,
        [Description("SEO")]
        SEO = 21,
        [Description("Meeting")]
        Meeting = 22,
        [Description("Hosting Management")]
        HostingManagement = 23
    }
}
