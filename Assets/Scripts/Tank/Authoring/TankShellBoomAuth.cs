using Unity.Entities;
using UnityEngine;

class TankShellBoomAuth : MonoBehaviour
{
    public ParticleSystem particleSystem;
    class TankShellBoomAuthBaker : Baker<TankShellBoomAuth>
    {
        public override void Bake(TankShellBoomAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TankShellBoom
            {
                particleEntity = GetEntity(authoring.particleSystem, TransformUsageFlags.Dynamic),
            });
            AddComponent(entity, new TankShellBoomTimer
            {
                TimeToLive = 10f,
            });
        }
    }
}

