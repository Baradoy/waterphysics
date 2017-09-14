using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;

namespace WaterPhysics
{
    public class TopographyGrid : MapComponent
    {
        private class ElevationCellGiver : Verse.ICellBoolGiver
        {
            private TopographyGrid grid;

            private Color color;

            private byte level;            

            public static int colorCount = 32;
            private Color[] colorSpectrum;

            public Color Color
            {
                get
                {
                    return Color.white;
                }
            }

            public ElevationCellGiver(TopographyGrid grid, byte level, Color color)
            {
                this.grid = grid;
                this.level = level;
                this.color = color;                

                colorSpectrum = new Color[colorCount];               

                for (var i = 0; i < colorCount; i++)
                {
                    var channelR = i / (float)colorCount;
                    
                    colorSpectrum[i] = new Color(channelR, 1f, 1f);
                }
            }

            public bool GetCellBool(int index)
            {                           
                return true;
            }

            public Color GetCellExtraColor(int index)
            {
                return new Color( this.grid.elevation[this.grid.map.cellIndices.IndexToCell(index)], 1f, 1f); 
                // return colorSpectrum[this.grid.grid[index]];
            }
        }

        private ByteGrid grid;

        private MapGenFloatGrid elevation;

        private CellBoolDrawer drawer;     

        public TopographyGrid(Map map) : base(map)
        {
            this.elevation = new MapGenFloatGrid(map);

            this.grid = new ByteGrid(map);
            for (int i = 0; i < map.cellIndices.NumGridCells; i++)
            {
                this.grid[i] = (Byte)(i % ElevationCellGiver.colorCount);
            }
        }

        public override void ExposeData()
        {
            this.grid.ExposeData();           
        }       

        public override void MapComponentUpdate()
        {          
            if ( SettingsController.ShowTopographyMap )
            {
                if( this.drawer == null )
                {
                    drawer = new CellBoolDrawer(new TopographyGrid.ElevationCellGiver(this, 1, Color.blue), this.map.Size.x, this.map.Size.z);
                }
                this.drawer.MarkForDraw();
                this.drawer.CellBoolDrawerUpdate();
            }
        }       

        public void SetValue(IntVec3 pos, float val)
        {
            this.elevation[pos] = val;           
            if (this.drawer != null)
            {
                this.drawer.SetDirty();
            }
        }    
    }
}
