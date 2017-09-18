using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WaterPhysics
{
    public class SettingsController : Mod
    {
        public string viscosityBuffer;

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
            list.Label("Ticks Between Updates");            
            list.TextFieldNumeric<int>(ref Settings.Viscosity, ref viscosityBuffer, 0, 100000);
            list.End();
        }
    }

    class Settings : ModSettings
    {
        public static readonly int defaultTick = 256;
        public static int Viscosity = defaultTick;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref Viscosity, "WaterPhysics.Viscosity", defaultTick, false);
        }
    }
}
