using Unity.Entities;
using Unity.Mathematics;

public struct TankData : IComponentData
{
	public float moveSpeed;
	public float rotateSpeed;
	public TankTypes type;

	public float3 firePos;
}

public struct TankIsPlayer : IComponentData, IEnableableComponent
{
}
