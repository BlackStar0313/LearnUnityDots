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
                    var playerTank = state.EntityManager.Instantiate(config.tankPrefab);
                    state.EntityManager.SetComponentEnabled<TankIsPlayer>(playerTank, true);

                    var createJob = new TankPlayerInitJob();
                    state.Dependency = createJob.Schedule(state.Dependency);
                }
            }
        }

        public partial struct TankPlayerInitJob : IJobEntity
        {
            public void Execute(Entity entity, ref TankData tankData, ref LocalTransform localTransform, ref TankHealth tankHealth)
            {
                tankData.moveSpeed = 2;
                tankData.type = TankTypes.Player;

                tankHealth.maxHp = 100;
                tankHealth.curHp = 100;

                localTransform.Position = new float3(0, 0, -4);
            }
        }
    }
}

