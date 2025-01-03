using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Logging;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

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

                var playerTankArr = SystemAPI.QueryBuilder()
                    .WithAll<TankData>()
                    .WithAll<TankIsPlayer>()
                    .Build().ToEntityArray(Allocator.TempJob);

                var playerTank = playerTankArr[0];

                Log.Info($"playerTank length: {playerTankArr.Length}");
                var playerTankTransform = state.EntityManager.GetComponentData<LocalTransform>(playerTank);
                var tankData = state.EntityManager.GetComponentData<TankData>(playerTank);
                var shellTransform = state.EntityManager.GetComponentData<LocalTransform>(shell);

                // 计算firePos的世界坐标
                float3 firePosLocal = tankData.firePos;
                float3 firePosWorld = math.transform(playerTankTransform.ToMatrix(), firePosLocal);

                shellTransform.Position = firePosWorld;
                // 计算炮弹的初始旋转，上抬30度
                quaternion initialRotation = math.mul(playerTankTransform.Rotation, quaternion.RotateX(math.radians(-30f)));
                shellTransform.Rotation = initialRotation;
                // shellTransform.Rotation = playerTankTransform.Rotation;
                state.EntityManager.SetComponentData(shell, shellTransform);

                // Log.Info($"playerTankTransform.Position: {shellTransform.Position}");


                // 计算斜向上45度的力
                // 计算相对于坦克正面的斜向上45度的力
                float3 localForceDirection = math.normalize(new float3(0, 1, 1)); // 相对于坦克正面的方向
                float3 worldForceDirection = math.rotate(playerTankTransform.Rotation, localForceDirection);
                float forceMagnitude = 10f; // 力的大小
                float3 force = worldForceDirection * forceMagnitude;
                // 设置刚体的初始速度
                var physicsVelocity = new PhysicsVelocity
                {
                    Linear = force,
                    Angular = float3.zero
                };
                state.EntityManager.SetComponentData(shell, physicsVelocity);


                // 获取粒子系统组件
                // PlayFireParticleSystem(state.EntityManager, shell);
                state.EntityManager.SetComponentEnabled<TankShellParticleTag>(shell, true);
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
