﻿using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CellData : IComponentData
    {
        public int2 GridPosition;
        public bool IsAlive;
    }
}