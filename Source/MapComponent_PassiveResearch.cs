using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;

namespace PassiveResearchForked
{
    public class MapComponent_PassiveResearch : MapComponent
    {
        private int lastProcessedDay = -1;
        private float rpPerTick = 0f;
        private bool hasInitialized = false;

        public static Dictionary<Pawn, int> todayContributions = new Dictionary<Pawn, int>();
        public static float LastTotalGain = 0f;

        // Curve defines RP gain *per tick* based on Intellectual level
        public static readonly SimpleCurve SkillToResearchCurve = new SimpleCurve
{
    new CurvePoint(0f,     0f),
    new CurvePoint(5f,     0.0005f), // 30 per day
    new CurvePoint(10f,    0.0010f), // 60 per day
    new CurvePoint(20f,    0.0020f), // 120 per day
    new CurvePoint(40f,    0.0030f), // 180 per day 
    new CurvePoint(60f,    0.0040f), // 240 per day
    new CurvePoint(80f,    0.0050f), // 300 per day
    new CurvePoint(100f,   0.0080f) // 480 per day
};

        public MapComponent_PassiveResearch(Map map) : base(map) { }

        public override void MapComponentTick()

        {
            if (Find.TickManager.TicksGame % 1000 == 0)
                // Log.Message("[PassiveResearch] MapComponent is ticking.");
            
            // Add RP every tick using precomputed value
            if (rpPerTick > 0f)
            {
                Find.ResearchManager.ResearchPerformed(rpPerTick, null);
            }

            // Recalculate once per day at 06:00
            if (GenLocalDate.HourInteger(map) == 6)
            {
                int currentDay = GenLocalDate.DayOfYear(map);
                if (currentDay != lastProcessedDay)
                {
                    lastProcessedDay = currentDay;
                    RecalculateDailyResearchRate();
                    // Log.Message($"[PassiveResearch] Calculated  RP/day. This is {rpPerTick:F4} RP/tick.");
                }
            }
        }

        public override void MapComponentUpdate()
        {
            // Calculate once on load
            if (!hasInitialized && Current.ProgramState == ProgramState.Playing)
            {
                hasInitialized = true;
                RecalculateDailyResearchRate();
            }
        }

        private void RecalculateDailyResearchRate()
        {
            int total = 0;
            todayContributions.Clear();

            foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
            {
                var intel = pawn.skills?.GetSkill(SkillDefOf.Intellectual);
                if (intel == null) continue;

                float rawGain = SkillToResearchCurve.Evaluate(intel.Level);
                float finalGain = rawGain * PassiveResearchMod.Settings.CurveMultiplier;

                int gain = Mathf.RoundToInt(finalGain);
                if (gain > 0)
                {
                    total += gain;
                    todayContributions[pawn] = gain;
                }
            }

            LastTotalGain = total;
            rpPerTick = total;

            if (Prefs.DevMode)
                Log.Message($"[PassiveResearch] Total RP/day = {total}, RP/tick = {rpPerTick:F4}");
            DisableResearchWorkIfNeeded();
        }
        private void DisableResearchWorkIfNeeded()
        {
            if (!PassiveResearchMod.Settings.AllowActiveResearch)
            {
                foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
                {
                    if (pawn.workSettings != null && pawn.workSettings.EverWork)
                    {
                        pawn.workSettings.SetPriority(WorkTypeDefOf.Research, 0);
                    }
                }
            }
        }

    }
}
