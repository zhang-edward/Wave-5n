using UnityEngine;

public class MageConflagration : HeroPowerUp
{
	public float burnChance = 0.2f;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		hero.player.OnEnemyLastHit += ApplyBurn;
	}

	private void ApplyBurn(Enemy e)
	{
		if (Random.value >= burnChance)
			return;
		GameObject burn = Instantiate(StatusEffectContainer.instance.GetStatus("Burn"));
		e.AddStatus(burn.gameObject);
	}
}