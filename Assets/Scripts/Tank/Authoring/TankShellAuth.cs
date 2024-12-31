using Unity.Entities;
using UnityEngine;

class TankShellAuth : MonoBehaviour
{
    class TankShellAuthBaker : Baker<TankShellAuth>
    {
        public override void Bake(TankShellAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TankShell
            {
                moveSpeed = 1
            });
        }
    }
}

