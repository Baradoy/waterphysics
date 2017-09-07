using UnityEngine;
using Verse;

namespace WaterPhysics
{
    [StaticConstructorOnStartup]
    internal class TexButton
    {

        public static readonly Texture2D Pause = ContentFinder<Texture2D>.Get("WaterPhysics_Icon", true);
    }
}
