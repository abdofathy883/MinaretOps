using Core.Enums.Leads;

namespace Core.Helpers
{
    public static class LeadQualificationCalculator
    {
        /// <summary>
        /// Max 4 points: Below=0, Equal=2, Higher=4
        /// </summary>
        public static double GetBudgetScore(LeadBudget budget) => budget switch
        {
            LeadBudget.Below => 0,
            LeadBudget.Equal => 2,
            LeadBudget.Higher => 4,
            _ => 0
        };

        /// <summary>
        /// Max 2 points: NotResponsible=0, Responsible_NOT_DecisionMaker=1, Responsible_DecisionMaker=2
        /// </summary>
        public static double GetResponsibilityScore(LeadResponsibility responsibility) => responsibility switch
        {
            LeadResponsibility.NotResponsible => 0,
            LeadResponsibility.Responsible_NOT_DecisionMaker => 1,
            LeadResponsibility.Responsible_DecisionMaker => 2,
            _ => 0
        };

        /// <summary>
        /// Max 2 points: Cold=0, Warm=1, Hot=2
        /// </summary>
        public static double GetInterestLevelScore(InterestLevel interestLevel) => interestLevel switch
        {
            InterestLevel.Cold => 0,
            InterestLevel.Warm => 1,
            InterestLevel.Hot => 2,
            _ => 0
        };

        /// <summary>
        /// Max 1 point: Below=0, Equal=0.5, Higher=1
        /// </summary>
        public static double GetTimelineScore(LeadTimeline timeline) => timeline switch
        {
            LeadTimeline.Below => 0,
            LeadTimeline.Equal => 0.5,
            LeadTimeline.Higher => 1,
            _ => 0
        };

        /// <summary>
        /// Max 1 point: Below=0, Equal=0.5, Higher=1
        /// </summary>
        public static double GetNeedsExpectationScore(NeedsExpectation needsExpectation) => needsExpectation switch
        {
            NeedsExpectation.Below => 1,
            NeedsExpectation.Equal => 0.5,
            NeedsExpectation.Higher => 0,
            _ => 0
        };

        /// <summary>
        /// Total qualification score (1–10 scale).
        /// Budget=4, Responsibility=2, InterestLevel=2, Timeline=1, NeedsExpectation=1
        /// </summary>
        public static double Calculate(
            LeadBudget budget,
            LeadResponsibility responsibility,
            InterestLevel interestLevel,
            LeadTimeline timeline,
            NeedsExpectation needsExpectation)
        {
            return GetBudgetScore(budget)
                 + GetResponsibilityScore(responsibility)
                 + GetInterestLevelScore(interestLevel)
                 + GetTimelineScore(timeline)
                 + GetNeedsExpectationScore(needsExpectation);
        }
    }
}
