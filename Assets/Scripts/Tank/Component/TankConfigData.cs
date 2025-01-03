using Unity.Entities;
using Unity.Mathematics;

public struct TankConfigData : IComponentData
{
	public Entity TankPrefab;
	public Entity ShellPrefab;
	public Entity ShellBoomPrefab;
	public int EnemyCount;
	public int PlayerHp;
	public int EnemyHp;
}

public struct TankShellBoomPosCollection : IBufferElementData
{
	public float3 Position;
}
