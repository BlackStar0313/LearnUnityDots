using Unity.Entities;
using UnityEngine;


namespace Tank
{
    class TankConfigAuth : MonoBehaviour
    {
        public GameObject TankPrefab;
        public GameObject ShellPrefab;
        public GameObject ShellBoomPrefab;
        public GameObject BoomPrefab;
        public int EnemyCount = 100;
        public int PlayerHp = 100;
        public int EnemyHp = 100;

        public Material TankMaterial;

        class TankConfigAuthBaker : Baker<TankConfigAuth>
        {
            public override void Bake(TankConfigAuth authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TankConfigData
                {
                    TankPrefab = GetEntity(authoring.TankPrefab, TransformUsageFlags.Dynamic),
                    EnemyCount = authoring.EnemyCount,
                    PlayerHp = authoring.PlayerHp,
                    EnemyHp = authoring.EnemyHp,
                    ShellPrefab = GetEntity(authoring.ShellPrefab, TransformUsageFlags.Dynamic),
                    ShellBoomPrefab = GetEntity(authoring.ShellBoomPrefab, TransformUsageFlags.Dynamic),
                    BoomPrefab = GetEntity(authoring.BoomPrefab, TransformUsageFlags.Dynamic)
                });

                AddComponentObject(entity, new TankConfigColorData
                {
                    TankMaterial = authoring.TankMaterial
                });

                AddBuffer<TankShellBoomPosCollection>(entity);
                AddBuffer<TankBoomPosCollection>(entity);
            }
        }
    }
}