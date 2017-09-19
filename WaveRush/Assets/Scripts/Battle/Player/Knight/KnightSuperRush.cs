using UnityEngine;
using PlayerAbilities;
using System.Collections;

public class KnightSuperRush : HeroPowerUp
{
	private KnightHero knight;
	private bool activated;
	private float chargeSpeed = 0.5f;

	public RushAbility rushAbility;
	public IndicatorEffect chargeEffect;
	public PlayerHero.InputAction storedOnSwipe;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.knight = (KnightHero)hero;
		rushAbility.Init(hero.player, knight.DamageEnemy);
		knight.onTapHoldDown += ActivateSuperRush;
		knight.onTapRelease += () =>
		{
			knight.anim.Play("Default");
			chargeEffect.gameObject.SetActive(false);
		};
		percentActivated = 0f;
	}

	public override void Deactivate()
	{
		base.Deactivate ();
		knight.OnKnightRush -= ActivateSuperRush;
	}

	public override void Stack ()
	{
		base.Stack ();
		chargeSpeed += 0.2f;
	}

	private void ActivateSuperRush()
	{
		if (activated)
			return;
		knight.anim.Play("SpecialPersist");
		percentActivated += chargeSpeed * Time.deltaTime;
		chargeEffect.gameObject.SetActive(true);
		if (percentActivated >= 1.0f)
		{
			chargeEffect.AnimateOut();
			percentActivated = 1f;
			activated = true;
			storedOnSwipe = knight.onSwipe;
			knight.onSwipe = SuperRush;	
		}
	}

	private void SuperRush()
	{
		// check cooldown
		if (!knight.CheckIfCooledDownNotify (0, true, playerHero.HandleSwipe))
			return;
		knight.ResetCooldownTimer (0);
		knight.damageMultiplier *= 2f;
		rushAbility.Execute();
		knight.onSwipe = storedOnSwipe;
		percentActivated = 0f;
		activated = false;
		Invoke("ResetRushAbility", rushAbility.duration);
	}

	private void ResetRushAbility()
	{
		knight.damageMultiplier *= 0.5f;
	}
}

