using Unity.Entities;
using UnityEngine;

class SampleCubeAuth : MonoBehaviour
{
    class Baker : Baker<SampleCubeAuth>
    {
        public override void Bake(SampleCubeAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            Debug.Log("Entity created with ID: " + entity.Index);
            AddComponent(entity, new SampleTag { Value = 1.0f });
        }
    }
}


struct SampleTag : IComponentData
{
    public float Value;
}