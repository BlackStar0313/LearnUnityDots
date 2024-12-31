using Unity.Burst;
using Unity.Entities;
using Unity.Logging;

namespace Tank
{
    [UpdateInGroup(typeof(TankShellSystemGroup))]
    partial struct TankShellSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankConfigData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<TankConfigData>();


            // Log.Info("TankShellSpawnSystem OnUpdate");
            var input = SystemAPI.GetSingleton<TankInput>();
            if (input.Fire == true)
            {
                var shell = state.EntityManager.Instantiate(config.shellPrefab);
                var tankShell = state.EntityManager.GetComponentData<TankShell>(shell);
                tankShell.moveSpeed = 10;
            }

            // var shellQuery = SystemAPI.QueryBuilder().WithAll<TankShell>().Build();
            // if (shellQuery.CalculateEntityCount() == 0)
            // {
            //     var shell = state.EntityManager.Instantiate(config.shellPrefab);
            //     var tankCell = state.EntityManager.GetComponentData<TankShell>(shell);
            //     tankCell.moveSpeed = 10;
            // }
        }
    }
}
