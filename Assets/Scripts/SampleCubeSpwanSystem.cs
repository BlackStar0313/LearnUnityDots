using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

partial struct SampleCubeSpwanSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SampleCubeInitData>();
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        // Debug.Log("SampleCubeSpwanSystem OnUpdate");

        // BurstDebug.Log("SampleCubeSpwanSystem OnUpdate");

        var initEntity = SystemAPI.GetSingleton<SampleCubeInitData>();

        var curCubeArrQuery = SystemAPI.QueryBuilder().WithAll<SampleTag>().Build();
        var entityArray = curCubeArrQuery.ToEntityArray(Allocator.Temp);
        int waitForCreateNum = Math.Max(initEntity.Count - entityArray.Length, 0);

        // Debug.Log(waitForCreateNum);
        if (waitForCreateNum > 0)
        {
            var instances = state.EntityManager.Instantiate(initEntity.CubPrefab, waitForCreateNum, Allocator.Temp);
            for (int i = 0; i < instances.Length; i++)
            {
                var entity = instances[i];
                var sampleTag = state.EntityManager.GetComponentData<SampleTag>(entity);

                var randSpeed = new float3(
                    UnityEngine.Random.Range(-1.0f, 2.0f), // x value
                    0.0f, // y value
                    UnityEngine.Random.Range(-1.0f, 1.0f)  // z value
                );
                sampleTag.Speed = randSpeed;
                state.EntityManager.SetComponentData(entity, sampleTag);
            }
        }
    }
}

// public static class BurstDebug
// {
//     [Conditional("UNITY_EDITOR")]
//     public static void Log(string message)
//     {
//         UnityEngine.Debug.Log(message);
//     }
// }