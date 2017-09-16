using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;

namespace WaterPhysics
{
    public class TopographyGrid : MapComponent
    {
        private class TopographyCellGiver : Verse.ICellBoolGiver
        {
            private Map map;            

            private ByteGrid byteGrid;

            private Color lowerColor;
            private Color upperColor;

            public Color Color
            {
                get
                {
                    return Color.white;
                }
            }

            public TopographyCellGiver(Map map, ByteGrid byteGrid, Color lowerColor, Color upperColor)
            {
                this.map = map;                
                this.byteGrid = byteGrid;
                this.lowerColor = lowerColor;
                this.upperColor = upperColor;
            }

            public bool GetCellBool(int index)
            {                           
                return true;
            }

            public Color GetCellExtraColor(int index)
            {                              
                return Color.Lerp(
                    lowerColor,
                    upperColor,
                    this.byteGrid[this.map.cellIndices.IndexToCell(index)] / 256f
                );                                          
            }
        }
     
        private ByteGrid elevation;
        private ByteGrid moisture;
        private ByteGrid waterLevel;

        private CellBoolDrawer elevationDrawer;
        private CellBoolDrawer moistureDrawer;
        private CellBoolDrawer waterLevelDrawer;

        public TopographyGrid(Map map) : base(map)
        {
            this.elevation = new ByteGrid(map);
            this.moisture = new ByteGrid(map);
            this.waterLevel = new ByteGrid(map);             
        }

        public override void ExposeData()
        {
            this.elevation.ExposeData();
            this.moisture.ExposeData();
            this.waterLevel.ExposeData();
        }       

        public override void MapComponentUpdate()
        {
            if (this.elevationDrawer == null)
            {
                elevationDrawer = new CellBoolDrawer(new TopographyGrid.TopographyCellGiver(map, elevation, Color.blue, new Color(69f / 256f, 56f / 256f, 35f / 256f)), this.map.Size.x, this.map.Size.z);
            }
            if (this.moistureDrawer == null) {
                moistureDrawer = new CellBoolDrawer(new TopographyGrid.TopographyCellGiver(map, moisture, Color.blue, Color.white), this.map.Size.x, this.map.Size.z);
            }
            if (this.waterLevelDrawer == null) {
                waterLevelDrawer = new CellBoolDrawer(new TopographyGrid.TopographyCellGiver(map, waterLevel, Color.blue, Color.white), this.map.Size.x, this.map.Size.z);
            }            

            if ( SettingsController.ShowElevationMap )
            {
                this.elevationDrawer.MarkForDraw();
                this.elevationDrawer.CellBoolDrawerUpdate();
            }
            if (SettingsController.ShowMoistureMap) {
                this.moistureDrawer.MarkForDraw();
                this.moistureDrawer.CellBoolDrawerUpdate();
            }
            if (SettingsController.ShowWaterLevelMap)
            {
                this.waterLevelDrawer.MarkForDraw();
                this.waterLevelDrawer.CellBoolDrawerUpdate();
            }            
        }

        public override void MapComponentTick()
        {            
            int index = Rand.Range(0, this.map.cellIndices.NumGridCells);
            IntVec3 vector = this.map.cellIndices.IndexToCell(index);
            if ( this.moisture[vector] < 254 && this.waterLevel[vector] > 0)
            {
                this.moisture[vector]++;
                this.waterLevel[vector]--;
            }

            for (int i = 0; i < this.map.cellIndices.NumGridCells; i++) {
                vector = this.map.cellsInRandomOrder.Get(i);
                if (this.moisture[vector] < 254 && this.waterLevel[vector] > 0)
                {
                    this.moisture[vector]++;
                    this.waterLevel[vector]--;
                }
            }
        }

        public void SetElevationFromElevation(IntVec3 pos, float val)
        {
            Byte valueAsByte = RatioToByte(val);

            SetElevation(pos, valueAsByte);
        }

        public void SetElevation(IntVec3 pos, Byte val)
        {
            this.elevation[pos] = val;               
            if (this.elevationDrawer != null)
            {
                this.elevationDrawer.SetDirty();
            }
        }

        public void SetMoistureFromElevation(IntVec3 pos, float val)
        {
            Byte valueAsByte = RatioToByte(val);

            SetMoisture(pos, valueAsByte);
        }

        public void SetMoisture(IntVec3 pos, Byte val)
        {
            this.moisture[pos] = val;
            if (this.moistureDrawer != null)
            {
                this.moistureDrawer.SetDirty();
            }
        }

        public void SetWaterLevelFromElevation(IntVec3 pos, float val)
        {
            Byte valueAsByte = RatioToByte(val);

            SetWaterLevel(pos, valueAsByte);
        }

        public void SetWaterLevel(IntVec3 pos, Byte val)
        {
            this.waterLevel[pos] = val;
            if (this.waterLevelDrawer != null)
            {
                this.waterLevelDrawer.SetDirty();
            }
        }

        private Byte RatioToByte(float val)
        {
            return (Byte)Math.Max((float)Byte.MinValue, Math.Min((float)Byte.MaxValue, val * 256f));
        }
    }
}
