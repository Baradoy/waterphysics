using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;

namespace WaterPhysics
{
    class TopographyCellGiver : Verse.ICellBoolGiver
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
}
