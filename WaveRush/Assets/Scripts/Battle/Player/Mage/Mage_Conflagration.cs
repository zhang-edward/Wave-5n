using UnityEngine;
using Projectiles;

public class Mage_Conflagration : HeroPowerUpCharged
{
	private const float BURN_DAMAGE_MULTIPLIER = 0.2f;	// Burn deals 0.2x damage per second
	private const float BURN_DURATION = 5f;				// Burn lasts for 5 seconds
	private const int   NUM_BURN_SPREADS = 2;

	private MageHero mage;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (MageHero)hero;
	}

	protected override void ActivateEffect()
	{
		mage.OnMageShotFireball += SetFireballProperties;
	}

	private void SetFireballProperties(GameObject fireball)
	{
		fireball.GetComponent<Projectile>().OnDamagedTarget += BurnEnemy;
		// Reset Ability
		mage.OnMageShotFireball -= SetFireballProperties;
		DeactivateEffect();
	}

	private void BurnEnemy(IDamageable damageable, int damage)
	{
		// Add burn status
		GameObject burnObj = Instantiate(StatusEffectContainer.instance.GetStatus("Burn"));
		BurnStatus burn = burnObj.GetComponent<BurnStatus>();
		burn.damage = Mathf.CeilToInt(damage * BURN_DAMAGE_MULTIPLIER);
		burn.duration = BURN_DURATION;
		burn.numSpreads = NUM_BURN_SPREADS;
		((Enemy)damageable).AddStatus(burnObj);
	}
}