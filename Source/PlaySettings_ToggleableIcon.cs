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

            row.ToggleableIcon(ref SettingsController.ShowElevationMap, TexButton.Pause,
                "Show Elevation Topography Map", SoundDefOf.MouseoverToggle);

            row.ToggleableIcon(ref SettingsController.ShowMoistureMap, TexButton.Pause,
                "Show Moisture Topography Map", SoundDefOf.MouseoverToggle);

            row.ToggleableIcon(ref SettingsController.ShowWaterLevelMap, TexButton.Pause,
                "Show Water Level Topography Map", SoundDefOf.MouseoverToggle);
        }
    }
}
