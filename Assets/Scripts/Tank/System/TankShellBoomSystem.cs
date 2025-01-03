using Unity.Burst;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct TankShellBoomSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TankShellBoomPosCollection>();
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var configEntity = SystemAPI.GetSingletonEntity<TankConfigData>();
        DynamicBuffer<TankShellBoomPosCollection> buffer = state.EntityManager.GetBuffer<TankShellBoomPosCollection>(configEntity);
        if (state.EntityManager.HasBuffer<TankShellBoomPosCollection>(configEntity) && buffer.Length > 0)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                var pos = buffer[i];

                var configData = SystemAPI.GetSingleton<TankConfigData>();

                var boom = state.EntityManager.Instantiate(configData.ShellBoomPrefab);
                var boomTransform = state.EntityManager.GetComponentData<LocalTransform>(boom);
                boomTransform.Position = new float3(1, 1, 1);
                state.EntityManager.SetComponentData(boom, boomTransform);

                // 获取粒子系统组件并播放
                var shellBoom = state.EntityManager.GetComponentData<TankShellBoom>(boom);
                var particleSystem = state.EntityManager.GetComponentObject<ParticleSystem>(boom);
                if (particleSystem != null)
                {
                    Log.Info($"Boom: {particleSystem}");
                    particleSystem.Play();
                }
                // Debug.Log($"Boom: {pos.Position}");
            }
            buffer.Clear();
        }
    }
}
