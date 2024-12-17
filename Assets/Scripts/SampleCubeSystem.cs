using Unity.Burst;
using Unity.Burst.Intrinsics;
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

        // foreach (var (transform, sampleTag) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<SampleTag>>())
        // {
        //     transform.ValueRW.Position += sampleTag.ValueRO.Speed * deltaTime;
        // }

        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        // foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithEntityAccess())
        // {
        //     if (math.lengthsq(transform.ValueRW.Position) > 100)
        //     {
        //         ecb.DestroyEntity(entity);
        //     }
        // }
        // ecb.Playback(state.EntityManager);
        // ecb.Dispose();




        var entityQuery = SystemAPI.QueryBuilder().WithAll<SampleTag>().Build();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();


        var localTransformTypeHandle = state.GetComponentTypeHandle<LocalTransform>(false);
        var sampleTagTypeHandle = state.GetComponentTypeHandle<SampleTag>(true);
        var entityTypeHandle = state.GetEntityTypeHandle();

        var moveJob = new SampleCubeMoveJob
        {
            DeltaTime = deltaTime,
            LocalTransformTypeHandle = localTransformTypeHandle,
            SampleTagTypeHandle = sampleTagTypeHandle
        };
        state.Dependency = moveJob.ScheduleParallel(entityQuery, state.Dependency);


        var deleteJob = new SampleCubeDeleteJob
        {
            EntityCommandBuffer = ecb,
            LocalTransformTypeHandle = localTransformTypeHandle,
            EntityTypeHandle = entityTypeHandle
        };
        state.Dependency = deleteJob.ScheduleParallel(entityQuery, state.Dependency);

        // var deleteJob = new SampleCubeDeleteJob
        // {
        //     EntityCommandBuffer = ecb
        // };
        // deleteJob.Schedule();
    }
}


//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<  使用JobEntity  >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
// [BurstCompile]
// partial struct SampleCubeMoveJob : IJobEntity
// {
//     public float DeltaTime;
//     public void Execute(Entity entity, ref LocalTransform transform, ref SampleTag sampleTag)
//     {
//         transform.Position += sampleTag.Speed * DeltaTime;
//     }
// }

// [BurstCompile]
// partial struct SampleCubeDeleteJob : IJobEntity
// {
//     public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
//     public void Execute(Entity entity, LocalTransform transform, [ChunkIndexInQuery] int chunkIndex)
//     {
//         if (math.lengthsq(transform.Position) > 100)
//         {
//             EntityCommandBuffer.DestroyEntity(chunkIndex, entity);
//         }
//     }
// }
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

[BurstCompile]
partial struct SampleCubeMoveJob : IJobChunk
{
    public float DeltaTime;
    public ComponentTypeHandle<LocalTransform> LocalTransformTypeHandle;
    [ReadOnly] public ComponentTypeHandle<SampleTag> SampleTagTypeHandle;

    public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnableMask,
            in v128 chunkEnabledMask)
    {
        var localTransforms = chunk.GetNativeArray(ref LocalTransformTypeHandle);
        var sampleTags = chunk.GetNativeArray(ref SampleTagTypeHandle);

        for (int i = 0; i < chunk.Count; i++)
        {
            var localTransform = localTransforms[i];
            var sampleTag = sampleTags[i];

            localTransform.Position += sampleTag.Speed * DeltaTime;

            localTransforms[i] = localTransform;
        }
    }
}

[BurstCompile]
partial struct SampleCubeDeleteJob : IJobChunk
{
    public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
    public ComponentTypeHandle<LocalTransform> LocalTransformTypeHandle;
    public EntityTypeHandle EntityTypeHandle;

    public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnableMask,
            in v128 chunkEnabledMask)
    {
        var localTransforms = chunk.GetNativeArray(ref LocalTransformTypeHandle);
        var entities = chunk.GetNativeArray(EntityTypeHandle);

        for (int i = 0; i < chunk.Count; i++)
        {
            var localTransform = localTransforms[i];

            if (math.lengthsq(localTransform.Position) > 100)
            {
                EntityCommandBuffer.DestroyEntity(unfilteredChunkIndex, entities[i]);
            }
        }
    }
}