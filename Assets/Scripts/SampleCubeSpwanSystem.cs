using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

partial struct SampleCubeSpwanSystem : ISystem
{

    private float createInterval;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SampleCubeInitData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        float interval = 0.5f;

        createInterval -= SystemAPI.Time.DeltaTime;
        if (createInterval > 0)
        {
            return;
        }
        createInterval = interval;
        // Debug.Log("SampleCubeSpwanSystem OnUpdate");

        // BurstDebug.Log("SampleCubeSpwanSystem OnUpdate");

        var initEntity = SystemAPI.GetSingleton<SampleCubeInitData>();

        var curCubeArrQuery = SystemAPI.QueryBuilder().WithAll<SampleTag>().Build();
        int curCubeCount = curCubeArrQuery.CalculateEntityCount();
        int waitForCreateNum = Math.Max(initEntity.Count - curCubeCount, 0);
        // int waitForCreateNum = initEntity.Count;

        // var cubeFinishInitQuery = SystemAPI.QueryBuilder().WithAll<SampleWaitForInit>().Build();
        // state.EntityManager.RemoveComponent<SampleWaitForInit>(cubeFinishInitQuery);

        if (waitForCreateNum > 0)
        {
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<直接创建entity <<<<<<<<<<<<<
            // var instances = state.EntityManager.Instantiate(initEntity.CubPrefab, waitForCreateNum, Allocator.Temp);
            // for (int i = 0; i < instances.Length; i++)
            // {
            //     var entity = instances[i];
            //     var sampleTag = state.EntityManager.GetComponentData<SampleTag>(entity);

            //     var randSpeed = new float3(
            //         UnityEngine.Random.Range(-1.0f, 2.0f), // x value
            //         0.0f, // y value
            //         UnityEngine.Random.Range(-1.0f, 1.0f)  // z value
            //     );
            //     sampleTag.Speed = randSpeed;
            //     state.EntityManager.SetComponentData(entity, sampleTag);
            // }
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var prefab = initEntity.CubPrefab;
            for (int i = 0; i < waitForCreateNum; i++)
            {
                ecb.Instantiate(prefab);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();


            // var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            // var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            // var job = new SampleCubeSpwanCreateJob
            // {
            //     ECB = ecb,
            //     Prefab = initEntity.CubPrefab
            // };
            // job.ScheduleParallel();



            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var Job = new InitSampleCubeEntityJob
            {
                ECB = ECB,
                Random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000)), // 初始化随机数生成器
            };
            Job.ScheduleParallel();
        }
        // entityArray.Dispose();
        state.Dependency.Complete();
    }
}

[WithAll(typeof(SampleWaitForInit))]
[BurstCompile]
partial struct InitSampleCubeEntityJob : IJobEntity
{
    public Unity.Mathematics.Random Random;
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute(Entity entity, [EntityIndexInQuery] int index, ref SampleTag sampleTag, ref SampleWaitForInit waitForInit)
    {
        var randSpeed = new float3(
            Random.NextFloat(-1.0f, 2.0f), // x value
            0.0f, // y value
            Random.NextFloat(-1.0f, 1.0f)  // z value
        );

        sampleTag.Speed = randSpeed;
        // 修改SampleWaitForInit组件为false
        waitForInit.isInit = true;
        ECB.SetComponentEnabled<SampleWaitForInit>(index, entity, false);
        //         // 使用条件编译和非Burst编译的代码块来打印日志
        // #if UNITY_EDITOR
        //         Debug.Print($"Entity {entity.Index} Speed: {randSpeed}");
        // #endif
    }
}

public partial struct SampleCubeSpwanCreateJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public Entity Prefab;

    public void Execute([EntityIndexInQuery] int index)
    {
        var entity = ECB.Instantiate(index, Prefab);
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