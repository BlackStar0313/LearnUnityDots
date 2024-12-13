using Unity.Entities;
using UnityEngine;

class SampleCubeInit : MonoBehaviour
{

    public GameObject CubePrefab;
    public int Count = 10;

    class Baker : Baker<SampleCubeInit>
    {
        public override void Bake(SampleCubeInit authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            Debug.Log("Entity created with ID: " + entity.Index);

            AddComponent(entity, new SampleCubeInitData
            {
                CubPrefab = GetEntity(authoring.CubePrefab, TransformUsageFlags.Dynamic),
                Count = authoring.Count
            });
        }
    }
}

public struct SampleCubeInitData : IComponentData
{
    public Entity CubPrefab;
    public int Count;
}

