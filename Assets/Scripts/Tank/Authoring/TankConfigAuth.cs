using Unity.Entities;
using UnityEngine;


namespace Tank
{
    class TankConfigAuth : MonoBehaviour
    {
        public GameObject tankPrefab;
        public int enemyCount = 100;
        public int playerHp = 100;
        public int enemyHp = 100;

        class TankConfigAuthBaker : Baker<TankConfigAuth>
        {
            public override void Bake(TankConfigAuth authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TankConfigData
                {
                    tankPrefab = GetEntity(authoring.tankPrefab, TransformUsageFlags.Dynamic),
                    enemyCount = authoring.enemyCount,
                    playerHp = authoring.playerHp,
                    enemyHp = authoring.enemyHp
                });
            }
        }
    }
}