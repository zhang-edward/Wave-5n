using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemyActions
{
	public class CreateHitZoneAction : EnemyAction
	{

		public PlayerDetectionCircle hitZone;
		public int damage;

		public override void Interrupt()
		{
			CancelInvoke();
		}

		public override void Execute()
		{
			base.Execute();
			Player player = hitZone.Activate();
			if (player != null)
				player.Damage(damage);
		}
	}
}
