using Unity.Entities;

namespace Tank
{
	public partial class TankIntervalSystemGroup : ComponentSystemGroup
	{

		private float deltaTime;
		protected virtual float UpdateInterval => 0.05f;
		protected override void OnUpdate()
		{
			deltaTime += SystemAPI.Time.DeltaTime;
			if (deltaTime > UpdateInterval)
			{
				base.OnUpdate();
				deltaTime -= UpdateInterval;

				// Log.Info("TankShellSystemGroup OnUpdate");
			}
		}
	}
}
