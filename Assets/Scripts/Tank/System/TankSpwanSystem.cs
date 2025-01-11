using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace Tank
{
    // [UpdateInGroup(typeof(Beginsi))]
    partial struct TankSpwanSystem : ISystem
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
                // var config = SystemAPI.GetSingleton<TankConfigData>();
                SystemAPI.SetSingleton(config);

                //create player tank 
                var tankQuery = SystemAPI.QueryBuilder().WithAll<TankIsPlayer>().Build();
                if (tankQuery.CalculateEntityCount() == 0)
                {
                    var playerTank = state.EntityManager.Instantiate(config.TankPrefab);
                    state.EntityManager.SetComponentEnabled<TankIsPlayer>(playerTank, true);

                    var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
                    var createJob = new TankPlayerInitJob
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

        [WithNone(typeof(TankIsInit))]
        [WithAll(typeof(TankIsPlayer))]
        public partial struct TankPlayerInitJob : IJobEntity
        {
            public EntityCommandBuffer ECB;
            public void Execute(Entity entity, ref TankData tankData, ref LocalTransform localTransform, ref TankHealth tankHealth)
            {
                tankData.moveSpeed = 2;
                tankData.type = TankTypes.Player;

                tankHealth.maxHp = 100;
                tankHealth.curHp = 100;

                localTransform.Position = new float3(0, 5, -4);
                ECB.SetComponentEnabled<TankIsInit>(entity, true);
            }
        }
    }
}

