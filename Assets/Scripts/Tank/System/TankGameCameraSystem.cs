using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Transforms;


namespace Tank
{
    partial struct TankGameCameraSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankGameCamera>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<TankGameCamera>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tankQuery = SystemAPI.QueryBuilder().WithAll<TankIsPlayer, LocalTransform>().Build();
            var tankEntities = tankQuery.ToEntityArray(Allocator.TempJob);
            if (tankQuery.CalculateEntityCount() > 0)
            {
                var tankPlayer = tankEntities[0];
                var tankTransform = SystemAPI.GetComponent<LocalTransform>(tankPlayer);

                var cameraJob = new TankGameCameraJob
                {
                    playerTransform = tankTransform
                };
                state.Dependency = cameraJob.Schedule(state.Dependency);
            }
            tankEntities.Dispose();
        }
    }

    [WithAll(typeof(TankGameCamera))]
    public partial struct TankGameCameraJob : IJobEntity
    {
        public LocalTransform playerTransform;
        public void Execute(Entity entity, ref LocalTransform localTransform)
        {
            localTransform.Position = playerTransform.Position + new float3(0, 10, -10);
        }
    }
}
