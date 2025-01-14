using Unity.Entities;
using UnityEngine;

class TankColorChangeAuth : MonoBehaviour
{
    public GameObject TankPrefab;
    class TankColorChangeAuthBaker : Baker<TankColorChangeAuth>
    {
        public override void Bake(TankColorChangeAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TankColorChangeData
            {
                TankEntity = GetEntity(authoring.TankPrefab, TransformUsageFlags.Dynamic)
            });
            SetComponentEnabled<TankColorChangeData>(entity, true);
        }
    }
}

