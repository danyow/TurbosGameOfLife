﻿using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    [UpdateAfter(typeof(ProcessLifeSystem))]
    public class ChangeVitalStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, int entityInQueryIndex,ref CellData cellData, ref ChangeVitalState changeVitalState) =>
            {
                if (changeVitalState.Value)
                {
                    cellData.IsAlive = !cellData.IsAlive;
                    changeVitalState.Value = false;
                }
            }).ScheduleParallel();
            Dependency.Complete();
        }
    }
}