using Unity.Entities;

public struct TankConfigData : IComponentData
{
	public Entity tankPrefab;
	public int enemyCount;
	public int playerHp;
	public int enemyHp;
}
