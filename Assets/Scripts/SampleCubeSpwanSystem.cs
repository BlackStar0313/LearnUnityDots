using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

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
                var randValue = UnityEngine.Random.Range(0.5f, 2.0f);
                var sampleTag = state.EntityManager.GetComponentData<SampleTag>(entity);
                sampleTag.Speed = randValue;
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