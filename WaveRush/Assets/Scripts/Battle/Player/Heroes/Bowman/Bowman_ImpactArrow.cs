using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerActions;

public class Bowman_ImpactArrow : HeroPowerUp {

	private BowmanHero bowman;

	public override void Activate(PlayerHero hero) {
		base.Activate(hero);
		bowman = (BowmanHero)hero;
		bowman.OnPiercingArrow += PushEnemy;
	}

	public override void Deactivate() {
		base.Deactivate();
		bowman.OnPiercingArrow -= PushEnemy;
	}

	public void PushEnemy(Enemy e) {
		if (bowman.piercingArrowChargeLevel > 0) {
			PushEnemyBack(e, 24f, 0.5f);
		}
	}

	public void PushEnemyBack(Enemy e, float strength, float time)
	{
		Vector2 awayFromPlayerDir = (e.transform.position - transform.position).normalized;
		if (e.Disable(0.5f))
			e.body.AddImpulse(awayFromPlayerDir, strength);
	}
}
