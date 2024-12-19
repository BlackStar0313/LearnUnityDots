using Unity.Entities;
using UnityEngine;

namespace Tank
{
    class TankAuth : MonoBehaviour
    {
        class TankAuthBaker : Baker<TankAuth>
        {
            public override void Bake(TankAuth authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TankData
                {
                    moveSpeed = 5,
                    rotateSpeed = 90,
                    type = TankTypes.Player
                });
                AddComponent<TankIsPlayer>(entity);
                SetComponentEnabled<TankIsPlayer>(entity, false);
            }
        }
    }
}
