using Unity.Entities;
using UnityEngine;

class TankShellAuth : MonoBehaviour
{

    // public TankShellParticleAuth shellParticleAuth; // 引用子节点的TankShellParticleAuth
    public ParticleSystem particleSystem;


    class TankShellAuthBaker : Baker<TankShellAuth>
    {
        public override void Bake(TankShellAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TankShell
            {
                moveSpeed = 1,
                particleEntity = GetEntity(authoring.particleSystem, TransformUsageFlags.Dynamic)
            });

            AddComponent<TankShellParticleTag>(entity);
            SetComponentEnabled<TankShellParticleTag>(entity, false);

        }
    }
}

