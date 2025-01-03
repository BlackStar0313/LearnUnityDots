using Unity.Entities;

public struct TankShell : IComponentData
{
	public float moveSpeed;
	public Entity particleEntity;
}

public struct TankShellParticleTag : IEnableableComponent, IComponentData
{

}