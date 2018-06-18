using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemyActions
{
	public class CreateHitZoneAction : EnemyAction
	{
		public PlayerDetectionCircle hitZone;
		public Enemy enemy;
		public int baseDamage;

		public override void Interrupt()
		{
			CancelInvoke();
		}

		public override void Execute()
		{
			base.Execute();
			Player player = hitZone.Activate();
			if (player != null) {
					int damage = Formulas.EnemyDamageFormula(baseDamage, player.hero.level - e.level);
					player.Damage(damage);			
			}
		}
	}
}
