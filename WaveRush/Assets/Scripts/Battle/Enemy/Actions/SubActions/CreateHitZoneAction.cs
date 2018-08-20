using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemyActions
{
	public class CreateHitZoneAction : EnemyAction
	{
		public PlayerDetectionCircle hitZone;
		public int baseDamage;

		public override void Interrupt()
		{
			CancelInvoke();
		}

		public override void Execute()
		{
			base.Execute();
			IDamageable player = hitZone.Activate();
			if (player != null) {
				int damage = Formulas.EnemyDamage(baseDamage, ((Player)player).hero.level - e.level);
				player.Damage(damage, e);	
			}
		}
	}
}
