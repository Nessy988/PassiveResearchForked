using RimWorld;
using System.Reflection;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace PassiveResearchForked
{
    public class PassiveResearchMod : Mod
    {
        public static PassiveResearchSettings Settings;

        public PassiveResearchMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<PassiveResearchSettings>();
        }

        public override string SettingsCategory() => "Passive Research Forked";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var list = new Listing_Standard();
            list.Begin(inRect);

            list.Label("Curve Multiplier:");

            float[] presetValues = { 0.1f, 0.25f, 0.5f, 1f, 1.5f, 2f, 5f, 10f };

            // Find current index
            int currentIndex = System.Array.IndexOf(presetValues, Settings.CurveMultiplier);
            if (currentIndex < 0) currentIndex = 3; // fallback to 1x

            // Draw the slider (Widgets.HorizontalSlider does NOT round, so we handle it)
            Rect sliderRect = list.GetRect(24f);
            float rawSliderValue = Widgets.HorizontalSlider(
                sliderRect,
                currentIndex,
                0, presetValues.Length - 1,
                label: $"x{presetValues[currentIndex]}",
                leftAlignedLabel: "0.1x",
                rightAlignedLabel: "10x"
            );

            // Apply the rounded index to get final multiplier
            int newIndex = Mathf.RoundToInt(rawSliderValue);
            Settings.CurveMultiplier = presetValues[newIndex];





            list.CheckboxLabeled(
                "Allow normal research (benches)",
                ref Settings.AllowActiveResearch,
                "If unchecked, disables research benches. Research only comes from passive colonist thinking."
            );
            list.CheckboxLabeled(
                "Disable research work type when active research is disabled",
                ref Settings.DisableResearchWorkType,
                "If enabled, all colonists will have the Research work type set to 0 when passive-only mode is active."
            );

            list.CheckboxLabeled(
                "Show colonist research breakdown",
                ref Settings.ShowColonistBreakdown,
                "When checked, shows each colonist’s previous-day passive research contribution."
            );

            list.GapLine();

            if (Settings.ShowColonistBreakdown)
            {
                list.Label("Colonist Research Breakdown (yesterday):");
                foreach (var kv in MapComponent_PassiveResearch.todayContributions)
                {
                    list.Label("Colonist Research Breakdown (today):");
                }
            }

            list.End();
            Settings.Write();
        }
    }
}
