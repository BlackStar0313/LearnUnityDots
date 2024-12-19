using Unity.Entities;
using UnityEngine;


namespace Tank
{
    class TankGameCameraAuth : MonoBehaviour
    {
        class TankGameCameraAuthBaker : Baker<TankGameCameraAuth>
        {
            public override void Bake(TankGameCameraAuth authoring)
            {
                Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new TankGameCamera
                {

                });
                AddComponent(entity, new TankMainCarema { });
            }
        }
    }
}
