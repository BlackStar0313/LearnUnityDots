using Unity.Entities;
using UnityEngine;

class TankBoomAuth : MonoBehaviour
{
    class TankBoomAuthBaker : Baker<TankBoomAuth>
    {
        public override void Bake(TankBoomAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TankBoomData
            {
                BoomTime = 2f
            });
        }
    }
}
