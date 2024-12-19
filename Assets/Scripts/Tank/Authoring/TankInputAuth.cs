using Unity.Entities;
using UnityEngine;

namespace Tank
{
    class TankInputAuth : MonoBehaviour
    {
        class TankInputAuthBaker : Baker<TankInputAuth>
        {
            public override void Bake(TankInputAuth authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TankInput
                {
                    InputW = 0,
                    InputS = 0,
                    InputA = 0,
                    InputD = 0
                });
            }
        }
    }

}

