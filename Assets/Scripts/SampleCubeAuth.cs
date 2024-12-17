using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class SampleCubeAuth : MonoBehaviour
{
    class Baker : Baker<SampleCubeAuth>
    {
        public override void Bake(SampleCubeAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            // Debug.Log("Entity created with ID: " + entity.Index);

            // var randValue = Unity.Mathematics.Random.CreateFromIndex((uint)entity.Index).NextFloat(0f, 1.0f);
            // Debug.Log("Entity speed is  " + randValue);
            var randSpeed = new float3(
                UnityEngine.Random.Range(-1.0f, 1.0f), // x value
                0.0f, // y value
                UnityEngine.Random.Range(-1.0f, 1.0f)  // z value
            );
            AddComponent(entity, new SampleTag { Value = 1.0f, Speed = randSpeed });
            AddComponent(entity, new SampleWaitForInit());
        }
    }
}


struct SampleTag : IComponentData
{
    public float Value;

    public float3 Speed;
}

struct SampleWaitForInit : IComponentData
{

}


