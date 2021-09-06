﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class ClickToChangeSystem : SystemBase
    {
        private Camera mainCamera;
        private GridSpawnData _gridSpawnData;
        private CellGridReference _cellGridReference;
        //private CellMaterialData _cellMaterialData;
        
        protected override void OnStartRunning()
        {
            mainCamera = Camera.main;
            var gameController = GetSingletonEntity<GridSpawnData>();
            _gridSpawnData = EntityManager.GetComponentData<GridSpawnData>(gameController);
            _cellGridReference = EntityManager.GetComponentData<CellGridReference>(gameController);
            //_cellMaterialData = EntityManager.GetComponentData<CellMaterialData>(gameController);
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var tilePos = new int2
                {
                    x = (int) math.floor(worldPos.x),
                    y = (int) math.floor(worldPos.y)
                };
                if(!_gridSpawnData.IsValidCoordinate(tilePos)){return;}
                var entity = _cellGridReference.Value.Value.X[tilePos.x].Y[tilePos.y].Value;
                var posData = EntityManager.GetComponentData<TilePositionData>(entity);
                posData.IsAlive = !posData.IsAlive;
                EntityManager.SetComponentData(entity, posData);
                var visualEntity = _cellGridReference.Value.Value.X[tilePos.x].Y[tilePos.y].VisualValue;
                EntityManager.AddComponent<ChangeVisualsTag>(visualEntity);
            }
        }
    }
}