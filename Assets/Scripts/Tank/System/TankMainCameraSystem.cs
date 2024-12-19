using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Tank
{
    partial struct TankMainCameraSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankMainCarema>();
        }

        public void OnUpdate(ref SystemState state)
        {
            LocalTransform transform = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<TankMainCarema>());
            if (Camera.main)
            {
                // Camera.main.transform.SetPositionAndRotation(transform.Position, transform.Rotation);
                Camera.main.transform.position = transform.Position;
            }
        }
    }
}

