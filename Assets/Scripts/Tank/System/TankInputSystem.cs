using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Logging;

namespace Tank
{
    partial struct TankInputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var input = SystemAPI.GetSingleton<TankInput>();

            input.InputA = Input.GetKey(KeyCode.A) ? -1 : 0;
            input.InputD = Input.GetKey(KeyCode.D) ? 1 : 0;
            input.InputW = Input.GetKey(KeyCode.W) ? 1 : 0;
            input.InputS = Input.GetKey(KeyCode.S) ? -1 : 0;

            SystemAPI.SetSingleton(input);

            // Log.Info($"Input: {input.InputA}, {input.InputD}, {input.InputW}, {input.InputS}");
        }
    }
}