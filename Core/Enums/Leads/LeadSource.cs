using System.ComponentModel;

namespace Core.Enums.Leads
{
    public enum LeadSource
    {
        [Description("Facebook")]
        Facebook = 0,
        [Description("Instagram")]
        Instagram = 1,
        [Description("LinkedIn")]
        LinkedIn = 2,
        [Description("Referral")]
        Referral = 3,
        [Description("Google Maps")]
        GoogleMaps = 4,
        [Description("Website")]
        Website = 5,
        [Description("Freelancing Platforms")]
        FreelancingPlatforms = 6
    }
}
