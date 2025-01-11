using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Logging;

namespace Tank
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    partial struct TankShellMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankShell>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            // var ecb = new EntityCommandBuffer(Allocator.TempJob);
            // var destroyJob = new TankShellDestoryJob
            // {
            //     EntityCommandBuffer = ecb.AsParallelWriter(),
            //     BoomPosCollection = state.GetBufferLookup<TankShellBoomPosCollection>(),
            //     ConfigEntity = SystemAPI.GetSingletonEntity<TankConfigData>(),
            // };
            // state.Dependency = destroyJob.Schedule(state.Dependency);
            // state.Dependency.Complete();
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();

            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            state.Dependency = new TankShellCollisionEvents
            {
                ECB = ecb,
                BoomPosLookup = state.GetBufferLookup<TankShellBoomPosCollection>(),
                ConfigEntity = SystemAPI.GetSingletonEntity<TankConfigData>(),
                TankShellLookup = SystemAPI.GetComponentLookup<TankShell>(),
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>()
            }.Schedule(simulation, state.Dependency);

            state.Dependency.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    partial struct TankShellCollisionEvents : ICollisionEventsJob
    {
        public EntityCommandBuffer ECB;
        public BufferLookup<TankShellBoomPosCollection> BoomPosLookup;
        public Entity ConfigEntity;
        public ComponentLookup<TankShell> TankShellLookup;
        public ComponentLookup<LocalTransform> TransformLookup;
        // public PhysicsWorld PhysicsWorld;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            Log.Info("TankShellCollisionEvents Execute");
            bool isShellA = TankShellLookup.HasComponent(entityA);
            bool isShellB = TankShellLookup.HasComponent(entityB);

            if (isShellA ^ isShellB)
            {
                Entity shellEntity = isShellA ? entityA : entityB;
                var shellPosition = TransformLookup[shellEntity].Position;

                var buffer = BoomPosLookup[ConfigEntity];
                buffer.Add(new TankShellBoomPosCollection
                {
                    Position = shellPosition
                });

                ECB.DestroyEntity(shellEntity);
            }
        }
    }

    // [BurstCompile]
    // partial struct TankShellDestoryJob : IJobEntity
    // {
    //     public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
    //     public BufferLookup<TankShellBoomPosCollection> BoomPosCollection;
    //     public Entity ConfigEntity;

    //     public void Execute(Entity entity, [EntityIndexInQuery] int index, [ReadOnly] ref LocalToWorld localToWorld, [ReadOnly] ref TankShell tankShell)
    //     {
    //         if (localToWorld.Position.y <= 0.5)
    //         {

    //             var buffer = BoomPosCollection[ConfigEntity];
    //             buffer.Add(new TankShellBoomPosCollection { Position = localToWorld.Position });

    //             EntityCommandBuffer.DestroyEntity(index, entity);
    //         }
    //     }
    // }

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