using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tank
{
    [UpdateInGroup(typeof(TankEnemySpawnSystemGroup))]
    partial struct TankEnemySpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankConfigData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.TryGetSingleton(out TankConfigData config))
            {
                SystemAPI.SetSingleton(config);

                //create enemy  tank 
                var enemyTank = state.EntityManager.Instantiate(config.TankPrefab);
                state.EntityManager.SetComponentEnabled<TankIsPlayer>(enemyTank, false);
                var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

                var createJob = new TankEnemyInitJob
                {
                    ECB = ecb
                };
                state.Dependency = createJob.Schedule(state.Dependency);
                state.Dependency.Complete();
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }
        }

    }

    [BurstCompile]
    [WithAll(typeof(TankData))]
    [WithNone(typeof(TankIsPlayer))]
    [WithNone(typeof(TankIsInit))]
    public partial struct TankEnemyInitJob : IJobEntity
    {


        public EntityCommandBuffer ECB;
        public void Execute(Entity entity, ref TankData tankData, ref LocalTransform localTransform, ref TankHealth tankHealth)
        {
            var random = Random.CreateFromIndex((uint)entity.Index);

            var randPosition = new float3(
                random.NextFloat(-40.0f, 40.0f),
                5,
                random.NextFloat(-40.0f, 40.0f)
            );
            localTransform.Position = randPosition;

            tankData.moveSpeed = 2;
            tankData.type = TankTypes.Enemy;

            tankHealth.maxHp = 100;
            tankHealth.curHp = 100;
            ECB.SetComponentEnabled<TankIsInit>(entity, true);
        }
    }
}