using Unity.Entities;

public struct TankInput : IComponentData
{
	public float InputW;
	public float InputS;
	public float InputA;
	public float InputD;

	public bool Fire;

}
