using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemyActions
{
	public class CreateDynamicHitZoneAction : EnemyAction
	{
		public PlayerDetectionCircle hitZone;
		public GenerateRandomPositionNearPlayerAction posGenerator;
		public Enemy enemy;
		public int baseDamage;

		public override void Interrupt()
		{
			CancelInvoke();
		}

		public override void Execute()
		{
			base.Execute();
			hitZone.transform.position = posGenerator.GetGeneratedPosition();
			Player player = hitZone.Activate();
			if (player != null) {
				int damage = Formulas.EnemyDamageFormula(baseDamage, player.hero.level - enemy.level);
				player.Damage(damage, e);	
			}
		}
	}
}
