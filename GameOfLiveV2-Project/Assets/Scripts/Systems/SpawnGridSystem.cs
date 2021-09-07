﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    [UpdateAfter(typeof(DestroyGridSystem))]
    public class SpawnGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NewGridData>();
        }

        protected override void OnUpdate()
        {
            Debug.Log("Spawnting");
            var gameController = GetSingletonEntity<GameControllerTag>();
            var gridSpawnData = EntityManager.GetComponentData<CurrentGridData>(gameController);
            var newGridData = EntityManager.GetComponentData<NewGridData>(gameController);
            var cellGridReference = EntityManager.GetComponentData<CellGridReference>(gameController);
            
            //cellGridReference.Value.Dispose();
            
            gridSpawnData.GridDimensions = newGridData.NewGridSize;
            EntityManager.SetComponentData(gameController, gridSpawnData);
            CameraController.Instance.SetToGridFullscreen(gridSpawnData.GridDimensions);
            
            var numEntities = gridSpawnData.GridDimensions.x * gridSpawnData.GridDimensions.y;
            var newTiles = EntityManager.Instantiate(gridSpawnData.TilePrefab, numEntities, Allocator.Temp);
            var dataPrefabArchetype = EntityManager.CreateArchetype(typeof(TilePositionData), typeof(CellGridReference), typeof(ChangeNextFrame));
            var newDatas = EntityManager.CreateEntity(dataPrefabArchetype, numEntities, Allocator.Temp);

            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var cellGridBlobAsset = ref blobBuilder.ConstructRoot<CellBlobAssetX>();
            
            var xArray = blobBuilder.Allocate(ref cellGridBlobAsset.X, gridSpawnData.GridDimensions.x);
            
            var tileIndex = 0;
            for (var x = 0; x < gridSpawnData.GridDimensions.x; x++)
            {
                var yArray = blobBuilder.Allocate(ref xArray[x].Y, gridSpawnData.GridDimensions.y);
                
                for (var y = 0; y < gridSpawnData.GridDimensions.y; y++)
                {
                    var newTranslation = new Translation {Value = new float3(x, y, 0)};
                    newTranslation.Value += gridSpawnData.PositionOffset;
                    EntityManager.SetComponentData(newTiles[tileIndex], newTranslation);

                    var newTilePosition = new TilePositionData
                    {
                        Value = new int2(x, y),
                        IsAlive = false,
                    };
                    EntityManager.SetComponentData(newDatas[tileIndex], newTilePosition);

                    yArray[y] = new CellData {Value = newDatas[tileIndex], VisualValue = newTiles[tileIndex]};
                    tileIndex++;
                }
            }

            var blobAssetReference = blobBuilder.CreateBlobAssetReference<CellBlobAssetX>(Allocator.Persistent);
            
            var cellGridRefData = new CellGridReference {Value = blobAssetReference};
            EntityManager.SetComponentData(gameController, cellGridRefData);

            foreach (var newData in newDatas)
            {
                EntityManager.SetComponentData(newData, cellGridRefData);
            }

            EntityManager.RemoveComponent<NewGridData>(gameController);
        }
    }
}