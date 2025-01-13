using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct TankEnemyMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TankData>();
        state.RequireForUpdate<TankIsInit>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerEntityQurey = SystemAPI.QueryBuilder().WithAll<TankData, TankIsInit>().Build();
        if (playerEntityQurey.CalculateEntityCount() > 0)
        {
            var playerEntity = playerEntityQurey.ToEntityArray(Allocator.TempJob)[0];
            var playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
            var moveJob = new TankEnemyMoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                PlayerPos = playerPos
            };
            state.Dependency = moveJob.Schedule(state.Dependency);
            state.Dependency.Complete();
        }
    }

    [WithAll(typeof(TankData))]
    [WithNone(typeof(TankIsPlayer))]
    [WithAll(typeof(TankIsInit))]
    [BurstCompile]
    public partial struct TankEnemyMoveJob : IJobEntity
    {
        public float DeltaTime;
        public float3 PlayerPos;

        void Execute(Entity entity, ref TankData tankData, ref LocalTransform transform)
        {
            if (tankData.type == TankTypes.Enemy)
            {
                var dirToPlayer = math.normalize(PlayerPos - transform.Position);
                transform.Position += dirToPlayer * tankData.moveSpeed * DeltaTime;

                // 让坦克朝向玩家
                var forward = new float3(0, 0, 1);
                transform.Rotation = quaternion.LookRotation(dirToPlayer, math.up());
            }
        }
    }
}
