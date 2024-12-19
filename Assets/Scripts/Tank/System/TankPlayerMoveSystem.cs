using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Logging;

namespace Tank
{
    partial struct TankPlayerMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankData>();
            state.RequireForUpdate<TankInput>();
            state.RequireForUpdate<TankIsPlayer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveJob = new TankPlayerMoveJob
            {
                tankInput = SystemAPI.GetSingleton<TankInput>(),
                deltaTime = SystemAPI.Time.DeltaTime
            };
            state.Dependency = moveJob.Schedule(state.Dependency);
        }
    }

    [WithAll(typeof(TankData), typeof(TankIsPlayer))]
    public partial struct TankPlayerMoveJob : IJobEntity
    {
        public TankInput tankInput;
        public float deltaTime;
        public void Execute(Entity entity, ref TankData tankData, ref LocalTransform transform)
        {
            var moveSpeed = tankData.moveSpeed;
            var rotateSpeed = tankData.rotateSpeed;
            var inputA = tankInput.InputA;
            var inputD = tankInput.InputD;
            var inputW = tankInput.InputW;
            var inputS = tankInput.InputS;

            // Log.Info($"Input: {inputA}, {inputD}, {inputW}, {inputS}");

            if (inputA != 0 || inputD != 0)
            {
                var rotation = quaternion.RotateY(math.radians((inputD + inputA) * rotateSpeed * deltaTime));
                transform.Rotation = math.mul(transform.Rotation, rotation);
            }

            var moveDir = new float3(0, 0, inputW + inputS);
            if (math.lengthsq(moveDir) < 0.01f)
            {
                return;
            }
            moveDir = math.normalize(moveDir);
            transform.Position += math.mul(transform.Rotation, moveDir) * moveSpeed * deltaTime;
        }
    }
}

