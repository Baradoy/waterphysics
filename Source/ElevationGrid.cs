using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;

namespace WaterPhysics
{
    public class TopographyGrid : MapComponent
    {
        private int ticks = 0;
        //Todo MouseoverReadout.MouseoverReadoutOnGUI override. 
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
                elevationDrawer = new CellBoolDrawer(new TopographyCellGiver(map, elevation, Color.yellow, Color.red), this.map.Size.x, this.map.Size.z);
            }
            if (this.moistureDrawer == null)
            {
                moistureDrawer = new CellBoolDrawer(new TopographyCellGiver(map, moisture, Color.yellow, Color.cyan), this.map.Size.x, this.map.Size.z);
            }
            if (this.waterLevelDrawer == null)
            {
                waterLevelDrawer = new CellBoolDrawer(new TopographyCellGiver(map, waterLevel, Color.yellow, Color.blue), this.map.Size.x, this.map.Size.z);
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
            ticks++;

            if (ticks > Settings.Viscosity)
            {
                ticks -= Settings.Viscosity;

                FlowWater();
                FlowGroundWater();
                SeepWater();                

                Dirty();
            }
        }

        // Water flows down from one cell to another. 
        private void FlowWater()
        {
            for (int i = 0; i < this.map.cellIndices.NumGridCells; i++)
            {
                IntVec3 cell = this.map.cellsInRandomOrder.Get(i);                
                IEnumerable<IntVec3> adjacents = GenAdj.CellsAdjacentCardinal(cell, Rot4.Random, IntVec2.One);
                
                foreach (IntVec3 adjacentCell in adjacents)
                {               
                    if (GenGrid.InBounds(adjacentCell,map))
                    {
                        int cellHeight = this.elevation[cell] + this.waterLevel[cell];
                        int adjacentHeight = this.elevation[adjacentCell] + this.waterLevel[adjacentCell];

                        if (this.waterLevel[cell] > 0 && this.waterLevel[adjacentCell] < 255 && cellHeight > adjacentHeight)
                        {
                            byte flow = (byte)Math.Max(1, (cellHeight - adjacentHeight) / 10);
                            this.waterLevel[adjacentCell] += flow;
                            this.waterLevel[cell] -= flow;
                        }
                    }                    
                }                
            }
        }

        // Moves groundwater spreads to adjacent cells
        private void FlowGroundWater()
        {
            for (int i = 0; i < this.map.cellIndices.NumGridCells; i++)
            {
                IntVec3 cell = this.map.cellsInRandomOrder.Get(i);
                IEnumerable<IntVec3> adjacents = GenAdj.CellsAdjacentCardinal(cell, Rot4.Random, IntVec2.One);

                foreach (IntVec3 adjacentCell in adjacents)
                {
                    if (GenGrid.InBounds(adjacentCell, map))
                    {
                        int cellHeight = this.elevation[cell] + this.moisture[cell];
                        int adjacentHeight = this.elevation[adjacentCell] + this.moisture[adjacentCell];

                        if (this.moisture[cell] > 0 && this.waterLevel[adjacentCell] < 255 && cellHeight > adjacentHeight)
                        {
                            this.waterLevel[adjacentCell]++;
                            this.moisture[cell]--;
                        }
                    }
                }
            }
        }

        // Water seeps into the soil.
        private void SeepWater()
        {
            for (int i = 0; i < this.map.cellIndices.NumGridCells; i++)
            {
                IntVec3 cell = this.map.cellsInRandomOrder.Get(i);
                if (this.moisture[cell] <= 255 && this.waterLevel[cell] > 0)
                {
                    this.moisture[cell]++;
                    this.waterLevel[cell]--;
                }
            }
        }

        private void Dirty()
        {
            if (this.moistureDrawer != null)
            {
                this.moistureDrawer.SetDirty();
            }
            if (this.waterLevelDrawer != null)
            {
                this.waterLevelDrawer.SetDirty();
            }
            if (this.elevationDrawer != null)
            {
                this.elevationDrawer.SetDirty();
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
