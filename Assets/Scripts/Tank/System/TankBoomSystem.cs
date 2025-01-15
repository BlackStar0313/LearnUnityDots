using Unity.Burst;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct TankBoomSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TankBoomPosCollection>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var configEntity = SystemAPI.GetSingletonEntity<TankConfigData>();
        DynamicBuffer<TankBoomPosCollection> buffer = state.EntityManager.GetBuffer<TankBoomPosCollection>(configEntity);
        if (state.EntityManager.HasBuffer<TankBoomPosCollection>(configEntity) && buffer.Length > 0)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                var pos = buffer[i];
                var configData = SystemAPI.GetSingleton<TankConfigData>();
                var boom = state.EntityManager.Instantiate(configData.BoomPrefab);
                var boomTransform = state.EntityManager.GetComponentData<LocalTransform>(boom);
                boomTransform.Position = new float3(pos.Position.x, pos.Position.y, pos.Position.z);
                state.EntityManager.SetComponentData(boom, boomTransform);

                var particleSystem = state.EntityManager.GetComponentObject<ParticleSystem>(boom);
                if (particleSystem != null)
                {
                    Log.Info($"TankBoomSystem Boom: {particleSystem}");
                    particleSystem.Play();

                    var timer = state.EntityManager.GetComponentData<TankBoomData>(boom);
                    timer.BoomTime = particleSystem.main.duration;
                    state.EntityManager.SetComponentData(boom, timer);
                }
            }
            buffer.Clear();
        }

        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (timer, entity) in SystemAPI.Query<RefRW<TankBoomData>>().WithEntityAccess())
        {
            timer.ValueRW.BoomTime -= deltaTime;
            if (timer.ValueRW.BoomTime <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

