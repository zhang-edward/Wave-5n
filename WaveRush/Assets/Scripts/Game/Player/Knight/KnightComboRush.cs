using UnityEngine;
using System.Collections;

public class KnightComboRush : HeroPowerUp
{
	private KnightHero knight;
	private float multiplier = 1f;		// the amount of speed that this powerup adds to the rush effect

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.knight = (KnightHero)hero;
		hero.player.OnEnemyDamaged += UpdateMultiplier;
	}

	private void UpdateMultiplier(float f)
	{
		float newMultiplier = 1f;
		if (playerHero.combo > 0)
			newMultiplier = 1f / (0.0075f * (playerHero.combo + 133));		// graph with Desmos.com
		//print (newMultiplier);
		if (newMultiplier < 0.8f)
			newMultiplier = 0.8f;
		for (int i = 0; i < playerHero.cooldownMultipliers.Length; i ++)
		{
			float dMultiplier = newMultiplier / multiplier;
			playerHero.cooldownMultipliers [i] *= dMultiplier;
		}
		multiplier = newMultiplier;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}
}

