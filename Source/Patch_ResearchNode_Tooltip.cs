using System.Reflection;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace PassiveResearchForked
{
    [HarmonyPatch]
    public static class Patch_ResearchNode_Tooltip
    {
        static MethodBase TargetMethod()
        {
            var method = AccessTools.Method("RimWorld.ResearchNode:DrawInfoInRect");
            if (method == null)
            {
                Log.Warning("[PassiveResearch] Could not find ResearchNode.DrawInfoInRect to patch.");
            }
            return method;
        }

        public static void Postfix(object __instance, Rect rect)
        {
            // safely exit if method patch failed
            if (__instance == null) return;

            ResearchProjectDef project = Traverse.Create(__instance).Field("project").GetValue<ResearchProjectDef>();
            if (project == null || project.IsFinished) return;

            float passivePerDay = MapComponent_PassiveResearch.LastTotalGain;
            if (passivePerDay <= 0.01f) return;

            float remaining = project.CostApparent * (1f - project.ProgressApparent);
            float estimatedDays = remaining / passivePerDay;

            TooltipHandler.TipRegion(rect, $"{project.LabelCap}\nEstimated time: {estimatedDays:F1} days (passive)");
        }
    }

}
