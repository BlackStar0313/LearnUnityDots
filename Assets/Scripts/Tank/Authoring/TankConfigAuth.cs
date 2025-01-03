using Unity.Entities;
using UnityEngine;


namespace Tank
{
    class TankConfigAuth : MonoBehaviour
    {
        public GameObject TankPrefab;
        public GameObject ShellPrefab;
        public GameObject ShellBoomPrefab;
        public int EnemyCount = 100;
        public int PlayerHp = 100;
        public int EnemyHp = 100;

        class TankConfigAuthBaker : Baker<TankConfigAuth>
        {
            public override void Bake(TankConfigAuth authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TankConfigData
                {
                    TankPrefab = GetEntity(authoring.TankPrefab, TransformUsageFlags.Dynamic),
                    EnemyCount = authoring.EnemyCount,
                    PlayerHp = authoring.PlayerHp,
                    EnemyHp = authoring.EnemyHp,
                    ShellPrefab = GetEntity(authoring.ShellPrefab, TransformUsageFlags.Dynamic),
                    ShellBoomPrefab = GetEntity(authoring.ShellBoomPrefab, TransformUsageFlags.Dynamic)
                });

                AddBuffer<TankShellBoomPosCollection>(entity);
            }
        }
    }
}