using Unity.Entities;
using UnityEngine;

namespace Tank
{
    class TankAuth : MonoBehaviour
    {
        public Transform firePos;

        class TankAuthBaker : Baker<TankAuth>
        {
            public override void Bake(TankAuth authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TankData
                {
                    moveSpeed = 5,
                    rotateSpeed = 90,
                    type = TankTypes.Player,
                    firePos = authoring.firePos.position
                });
                AddComponent<TankIsPlayer>(entity);
                SetComponentEnabled<TankIsPlayer>(entity, false);
            }
        }
    }
}
