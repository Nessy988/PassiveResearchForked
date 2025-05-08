using Verse;
using HarmonyLib;

[StaticConstructorOnStartup]
public static class PassiveResearchHarmony
{
    static PassiveResearchHarmony()
    {
        new Harmony("mih0k.passiveresearchforked").PatchAll();
    }
}
