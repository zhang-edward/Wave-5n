using UnityEngine;

public class Mage_Conflagration : HeroPowerUpCharged
{
	private MageHero mage;
	public int numBurnSpreads;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (MageHero)hero;
	}

	protected override void ActivateEffect()
	{
		mage.player.OnEnemyLastHit += BurnEnemy;
	}

	private void BurnEnemy(Enemy e)
	{
		// Add burn status
		GameObject burnObj = Instantiate(StatusEffectContainer.instance.GetStatus("Burn"));
		BurnStatus burn = burnObj.GetComponent<BurnStatus>();
		burn.damage = Mathf.CeilToInt(mage.damage * 0.1f);
		burn.numSpreads = numBurnSpreads;
		e.AddStatus(burnObj);

		// Reset Ability
		mage.player.OnEnemyLastHit -= BurnEnemy;
		DeactivateEffect();
	}
}