using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

namespace Tank
{
    partial struct TankChangeColorSystem : ISystem
    {
        private BatchMaterialID _batchMaterialID;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankColorChangeData>();
        }



        public void OnUpdate(ref SystemState state)
        {
            if (_batchMaterialID.Equals(default))
            {
                var configDataEntity = SystemAPI.GetSingletonEntity<TankConfigData>();
                var materialRed = state.EntityManager.GetComponentObject<TankConfigColorData>(configDataEntity);
                var hybridRenderer = state.World.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
                _batchMaterialID = hybridRenderer.RegisterMaterial(materialRed.TankMaterial);
                Log.Info("TankChangeColorSystem _batchMaterialID: " + _batchMaterialID.ToString());
            }

            var entityArray = SystemAPI.QueryBuilder().WithAll<TankColorChangeData>().Build().ToEntityArray(Allocator.TempJob);
            // Log.Info("TankChangeColorSystem entityArray count: " + entityArray.Length.ToString());

            foreach (var entity in entityArray)
            {
                // Log.Info("TankChangeColorSystem entity: " + entity.ToString());

                var tankEntity = state.EntityManager.GetComponentData<TankColorChangeData>(entity).TankEntity;
                var tankData = state.EntityManager.GetComponentData<TankData>(tankEntity);
                if (tankData.type == TankTypes.Player)
                {
                    continue;
                }
                // 获取子实体
                DynamicBuffer<Child> childs = state.EntityManager.GetBuffer<Child>(entity);
                foreach (var child in childs)
                {
                    var childEntity = child.Value;

                    // 检查并修改子实体的材质
                    if (SystemAPI.HasComponent<MaterialMeshInfo>(childEntity))
                    {
                        var meshInfo = SystemAPI.GetComponent<MaterialMeshInfo>(childEntity);

                        // var beforeMaterialID = meshInfo.MaterialID;
                        meshInfo.MaterialID = _batchMaterialID;
                        SystemAPI.SetComponent<MaterialMeshInfo>(childEntity, meshInfo);
                        // Log.Info($"Changed color for child entity:   -> {_batchMaterialID}");
                    }
                }

                state.EntityManager.SetComponentEnabled<TankColorChangeData>(entity, false);
            }
            entityArray.Dispose();
        }
    }
}
