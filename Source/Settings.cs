using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WaterPhysics
{
    public class SettingsController : Mod
    {
        public static bool ShowElevationMap = false;
        public static bool ShowMoistureMap = false;    
        public static bool ShowWaterLevelMap = false;

        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "WaterPhysics";
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = canvas.width;
            list.Begin(canvas);
            list.Label("Viscosity");
            Settings.Viscosity = list.Slider(Settings.Viscosity, 0, 2);
            list.End();
        }
    }

    class Settings : ModSettings
    {
        public static float Viscosity = 1.0f;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<float>(ref Viscosity, "WaterPhysics.Viscosity", 1.0f, false);
        }
    }
}
