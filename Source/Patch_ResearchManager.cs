using HarmonyLib;
using RimWorld;

namespace PassiveResearchForked
{
    [HarmonyPatch(typeof(ResearchManager), nameof(ResearchManager.ResearchPerformed))]
    public static class Patch_ResearchManager_ResearchPerformed
    {
        public static bool Prefix(ref float amount, ResearchManager __instance)
        {
            // If passive-only mode and this is not our own mod's tick
            if (!PassiveResearchMod.Settings.AllowActiveResearch)
            {
                if (!IsPassiveResearchSource())
                {
                    return false; // Block research gain
                }
            }

            return true; // Allow gain
        }

        private static bool IsPassiveResearchSource()
        {
            // Called directly from our mod? Allow it.
            var stackTrace = new System.Diagnostics.StackTrace();
            foreach (var frame in stackTrace.GetFrames())
            {
                var method = frame.GetMethod();
                if (method?.DeclaringType?.Name?.Contains("MapComponent_PassiveResearch") == true)
                    return true;
            }
            return false;
        }
    }
}
