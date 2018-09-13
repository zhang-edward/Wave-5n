using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemyActions
{
	public class ESA_CreateDynamicHitZoneAction : EnemyAction
	{
		public PlayerDetectionCircle hitZone;
		public GenerateRandomPositionNearPlayerAction posGenerator;
		public int baseDamage;

		public override void Interrupt()
		{
			CancelInvoke();
		}

		public override void Execute()
		{
			base.Execute();
			hitZone.transform.position = posGenerator.GetGeneratedPosition();
			IDamageable player = hitZone.Activate();
			if (player != null) {
				int damage = Formulas.EnemyDamage(baseDamage, ((Player)player).hero.level - e.level);
				player.Damage(damage, e);	
			}
			if (onActionFinished != null)
				onActionFinished();
		}
	}
}
