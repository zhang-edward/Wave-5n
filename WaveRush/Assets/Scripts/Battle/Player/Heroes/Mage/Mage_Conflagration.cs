using UnityEngine;
using Projectiles;

public class Mage_Conflagration : HeroPowerUpCharged
{
	private const float BURN_DAMAGE_MULTIPLIER = 0.1f;
	private const float BURN_DURATION = 10f;			
	private const int   NUM_BURN_SPREADS = 2;

	public Projectile lastShotFireball;
	private MageHero mage;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (MageHero)hero;
		chargeSpeed = 1 / 3f;		// 3 seconds charge time
	}

	protected override void ActivateEffect()
	{
		mage.OnMageShotFireball += SetFireballProperties;
	}

	private void SetFireballProperties(GameObject fireball)
	{
		lastShotFireball = fireball.GetComponent<Projectile>();
		lastShotFireball.OnDamagedTarget += BurnEnemy;
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
		lastShotFireball.OnDamagedTarget -= BurnEnemy;
	}
}