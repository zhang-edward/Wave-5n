using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja_SneakAttack : HeroPowerUp {

	private NinjaHero ninja;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		ninja = (NinjaHero)hero;
		ninja.player.OnEnemyLastHit += DamageEnemyMore;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		ninja.player.OnEnemyLastHit -= DamageEnemyMore;
	}

	private void DamageEnemyMore(Enemy e)
	{
		if (e.health > 0 && e.GetStatus("Stun"))
			e.Damage(ninja.damage);
	}
}
