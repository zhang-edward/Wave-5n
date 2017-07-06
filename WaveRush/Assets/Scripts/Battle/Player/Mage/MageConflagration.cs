using UnityEngine;

public class MageConflagration : HeroPowerUp
{
	public float burnChance = 0.2f;
	public MageHero mage;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (MageHero)hero;
		hero.player.OnEnemyLastHit += ApplyBurn;
	}

	private void ApplyBurn(Enemy e)
	{
		if (Random.value >= burnChance)
			return;
		GameObject burn = Instantiate(StatusEffectContainer.instance.GetStatus("Burn"));
		burn.GetComponent<BurnStatus>().damage = Mathf.CeilToInt(mage.damage * 0.1f);
		e.AddStatus(burn.gameObject);
	}
}