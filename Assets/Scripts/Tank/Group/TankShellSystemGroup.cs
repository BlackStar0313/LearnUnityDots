using Unity.Entities;
using Unity.Logging;
using UnityEngine;

namespace Tank
{
	public partial class TankShellSystemGroup : TankIntervalSystemGroup
	{
		protected override float UpdateInterval => 0.1f;

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}
	}
}
