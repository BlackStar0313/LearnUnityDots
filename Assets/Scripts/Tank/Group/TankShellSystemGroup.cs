using Unity.Entities;
using Unity.Logging;
using UnityEngine;

namespace Tank
{
	public partial class TankShellSystemGroup : ComponentSystemGroup
	{
		private float deltaTime;
		private float updateInterval = 0.05f;
		protected override void OnUpdate()
		{
			deltaTime += SystemAPI.Time.DeltaTime;
			if (deltaTime > updateInterval)
			{
				base.OnUpdate();
				deltaTime -= updateInterval;

				// Log.Info("TankShellSystemGroup OnUpdate");
			}
		}
	}
}
