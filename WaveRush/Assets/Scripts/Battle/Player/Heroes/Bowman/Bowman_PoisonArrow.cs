using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerActions;

public class Bowman_PoisonArrow : HeroPowerUp {

	private BowmanHero bowman;

	public override void Activate(PlayerHero hero) {
		base.Activate(hero);
		bowman = (BowmanHero)hero;
		bowman.OnPiercingArrow += PoisonEnemy;
	}

	public override void Deactivate() {
		base.Deactivate();
		bowman.OnPiercingArrow -= PoisonEnemy;
	}

	public void PoisonEnemy(Enemy e) {
		if (bowman.piercingArrowChargeLevel == 2) {
			PoisonStatus poison = Instantiate(StatusEffectContainer.instance.GetStatus("Poison")).GetComponent<PoisonStatus>();
			poison.duration = 3.0f;
			poison.damage = (int)(bowman.noiselessDamage * 0.1f);
			e.AddStatus(poison.gameObject);
		}
	}
}
