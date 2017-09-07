using Harmony;
using RimWorld;
using Verse;

namespace WaterPhysics
{
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    public static class PlaySettings_ToggleableIcon
    {
        [HarmonyPostfix]
        static void PostFix(WidgetRow row, bool worldView)
        {

            if (worldView)
            {
                return;
            }

            if (row == null || TexButton.Pause == null)
            {
                return;
            }

            row.ToggleableIcon(ref SettingsController.ShowTopographyMap, TexButton.Pause,
                "Show Topography Map", SoundDefOf.MouseoverToggle);
        }
    }
}
