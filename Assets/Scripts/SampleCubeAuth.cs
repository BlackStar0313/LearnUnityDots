using Unity.Entities;
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
            AddComponent(entity, new SampleTag { Value = 1.0f, Speed = 1 });
        }
    }
}


struct SampleTag : IComponentData
{
    public float Value;

    public float Speed;
}


