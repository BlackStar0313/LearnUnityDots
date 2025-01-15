using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TankConfigData : IComponentData
{
	public Entity TankPrefab;
	public Entity ShellPrefab;
	public Entity ShellBoomPrefab;
	public Entity BoomPrefab;
	public int EnemyCount;
	public int PlayerHp;
	public int EnemyHp;
}

public struct TankShellBoomPosCollection : IBufferElementData
{
	public float3 Position;
}

public struct TankBoomPosCollection : IBufferElementData
{
	public float3 Position;
}

public class TankConfigColorData : IComponentData
{
	public Material TankMaterial;
}