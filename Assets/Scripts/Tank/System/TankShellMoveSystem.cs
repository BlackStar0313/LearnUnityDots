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
                TankBoomPosLookup = state.GetBufferLookup<TankBoomPosCollection>(),
                ConfigEntity = SystemAPI.GetSingletonEntity<TankConfigData>(),
                TankShellLookup = SystemAPI.GetComponentLookup<TankShell>(),
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                TankDataLookup = SystemAPI.GetComponentLookup<TankData>()
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
        public BufferLookup<TankBoomPosCollection> TankBoomPosLookup;
        public Entity ConfigEntity;
        public ComponentLookup<TankShell> TankShellLookup;
        public ComponentLookup<LocalTransform> TransformLookup;
        public ComponentLookup<TankData> TankDataLookup;
        // public PhysicsWorld PhysicsWorld;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            Log.Info("TankShellCollisionEvents Execute");
            bool isShellA = TankShellLookup.HasComponent(entityA);
            bool isShellB = TankShellLookup.HasComponent(entityB);

            // 检查是否是敌方坦克
            bool isEnemyTankA = TankDataLookup.HasComponent(entityA) &&
                TankDataLookup[entityA].type == TankTypes.Enemy;
            bool isEnemyTankB = TankDataLookup.HasComponent(entityB) &&
                TankDataLookup[entityB].type == TankTypes.Enemy;

            bool isShellBoom = isShellA ^ isShellB;
            if (isShellBoom)
            {
                Entity shellEntity = isShellA ? entityA : entityB;
                var shellPosition = TransformLookup[shellEntity].Position;

                var buffer = BoomPosLookup[ConfigEntity];
                buffer.Add(new TankShellBoomPosCollection
                {
                    Position = shellPosition
                });


                bool isShellHitEnemyTank = isEnemyTankA ^ isEnemyTankB;
                if (isShellHitEnemyTank)
                {

                    Log.Info($"TankShellCollisionEvents Execute isShellHitEnemyTank: {isShellHitEnemyTank}");
                    Entity enemyShell = isEnemyTankA ? entityA : entityB;

                    var tankBoomBuffer = TankBoomPosLookup[ConfigEntity];
                    tankBoomBuffer.Add(new TankBoomPosCollection
                    {
                        Position = TransformLookup[enemyShell].Position
                    });
                    ECB.DestroyEntity(enemyShell);
                }

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