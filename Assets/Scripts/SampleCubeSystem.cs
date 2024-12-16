using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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

        foreach (var (transform, sampleTag) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<SampleTag>>())
        {
            transform.ValueRW.Position += sampleTag.ValueRO.Speed * deltaTime;
        }

        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        // foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithEntityAccess())
        // {
        //     if (transform.ValueRW.Position.x > 10)
        //     {
        //         ecb.DestroyEntity(entity);
        //     }
        // }
        // ecb.Playback(state.EntityManager);
        // ecb.Dispose();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var deleteJob = new SampleCubeDeleteJob
        {
            EntityCommandBuffer = ecb
        };
        deleteJob.Schedule();
    }

}

[BurstCompile]
partial struct SampleCubeDeleteJob : IJobEntity
{
    public EntityCommandBuffer EntityCommandBuffer;
    public void Execute(Entity entity, LocalTransform transform)
    {
        if (math.lengthsq(transform.Position) > 100)
        {
            EntityCommandBuffer.DestroyEntity(entity);
        }
    }
}