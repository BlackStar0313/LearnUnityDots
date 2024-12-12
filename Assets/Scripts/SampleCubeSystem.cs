using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial struct SampleCubeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        float deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>())
        {
            transform.ValueRW.Position.x += 1 * deltaTime;
        }
    }

}
