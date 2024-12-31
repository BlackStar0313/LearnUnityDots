using Unity.Burst;
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

        // [BurstCompile]
        // public void OnUpdate(ref SystemState state)
        // {
        //     var moveJob = new TankShellMoveJob
        //     {
        //         playerTankData = SystemAPI.GetSingleton<TankData>()
        //     };
        //     state.Dependency = moveJob.Schedule(state.Dependency);
        // }
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