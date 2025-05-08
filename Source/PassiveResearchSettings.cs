using Verse;

namespace PassiveResearchForked
{
    public class PassiveResearchSettings : ModSettings
    {
        public float CurveMultiplier = 1f;

        public bool AllowActiveResearch = true;
        public bool ShowColonistBreakdown = true;
        public bool DisableResearchWorkType = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref CurveMultiplier, "CurveMultiplier", 1f);
            Scribe_Values.Look(ref AllowActiveResearch, "AllowActiveResearch", true);
            Scribe_Values.Look(ref ShowColonistBreakdown, "ShowColonistBreakdown", true);
            Scribe_Values.Look(ref DisableResearchWorkType, "DisableResearchWorkType", true);
        }
    }
}
