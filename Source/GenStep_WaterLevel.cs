using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;

namespace WaterPhysics
{
    public class GenStep_WaterLevel : GenStep
    {
        public override void Generate(Map map)
        {          
            TopographyGrid topographyGrid = map.GetComponent<TopographyGrid>();

            if (topographyGrid == null)
            {         
                topographyGrid = new TopographyGrid(map);             
            }
            
            foreach (IntVec3 current in map.AllCells)
            {           
                topographyGrid.SetWaterLevelFromElevation(current, MapGenerator.Elevation[current]);
            }            
        }
    }
}
