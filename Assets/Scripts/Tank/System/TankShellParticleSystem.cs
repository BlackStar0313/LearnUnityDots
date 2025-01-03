using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Logging;
using UnityEngine;

namespace Tank
{
    partial struct TankShellParticleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TankShellParticleTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var particlesShells = SystemAPI.QueryBuilder().WithAll<TankShellParticleTag, TankShell>().Build().ToEntityArray(Allocator.TempJob);
            foreach (var entity in particlesShells)
            {
                PlayParticleSystem(state.EntityManager, entity);
            }

            particlesShells.Dispose();
        }

        private void PlayParticleSystem(EntityManager entityManager, Entity entity)
        {
            var tankShell = entityManager.GetComponentData<TankShell>(entity);
            // 获取粒子系统组件并播放
            var particleSystem = entityManager.GetComponentObject<ParticleSystem>(tankShell.particleEntity);
            if (particleSystem != null)
            {
                particleSystem.Play();
                entityManager.SetComponentEnabled<TankShellParticleTag>(entity, false);
            }
            else
            {
                Debug.LogWarning($"Entity {entity} has a ParticleSystem component but it is null.");
            }
        }
    }
}

