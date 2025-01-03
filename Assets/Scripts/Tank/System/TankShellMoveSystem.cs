using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;


namespace Tank
{
    partial struct TankShellMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankShell>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var destroyJob = new TankShellDestoryJob
            {
                EntityCommandBuffer = ecb.AsParallelWriter()
            };
            state.Dependency = destroyJob.Schedule(state.Dependency);
            state.Dependency.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

    }

    [BurstCompile]
    partial struct TankShellDestoryJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
        public void Execute(Entity entity, [EntityIndexInQuery] int index, [ReadOnly] ref LocalToWorld localToWorld, [ReadOnly] ref TankShell tankShell)
        {
            if (localToWorld.Position.y <= 0.5)
            {
                EntityCommandBuffer.DestroyEntity(index, entity);
            }
        }
    }

    // [WithAll(typeof(TankShell))]
    // public partial struct TankShellMoveJob : IJobEntity
    // {
    //     public TankData playerTankData;
    //     public void Execute(Entity entity, ref TankShell tankShell, ref LocalTransform transform)
    //     {
    //         transform.Position += transform.Forward * tankShell.moveSpeed * SystemAPI.Time.DeltaTime;
    //     }
    // }
}