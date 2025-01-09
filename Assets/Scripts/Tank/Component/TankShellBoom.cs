using Unity.Entities;

public struct TankShellBoom : IComponentData
{
	public Entity particleEntity;
}

public struct TankShellBoomTimer : IComponentData
{
	public float TimeToLive;
}
