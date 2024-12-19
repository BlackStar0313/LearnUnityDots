using Unity.Entities;
using UnityEngine;

namespace Tank
{
    class TankHeathAuth : MonoBehaviour
    {
        class TankHeathAuthBaker : Baker<TankHeathAuth>
        {
            public override void Bake(TankHeathAuth authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TankHealth
                {
                    curHp = 100,
                    maxHp = 100
                });
            }
        }
    }
}
