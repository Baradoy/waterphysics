using Harmony;
using RimWorld;
using System.Reflection;
using Verse;

namespace WaterPhysics
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.waterphsics");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("WaterPhysics: Harmony Patch");
        }
    }    
}