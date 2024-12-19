using Unity.Entities;

public struct TankData : IComponentData
{
	public float moveSpeed;
	public float rotateSpeed;
	public TankTypes type;
}

public struct TankIsPlayer : IComponentData, IEnableableComponent
{
}
