using Unity.Entities;

public struct TankData : IComponentData
{
	public float moveSpeed;
	public TankTypes type;
}

public struct TankIsPlayer : IComponentData, IEnableableComponent
{
}
