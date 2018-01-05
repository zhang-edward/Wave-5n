using UnityEngine;
using PlayerActions;
using System.Collections;

public class Knight_SuperShield : HeroPowerUpCharged
{
	public PA_AreaEffect   areaAttackAbility;
	public GameObject 	   shieldIndicator;

	private KnightHero knight;
	private PlayerHero.InputAction storedOnTap;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		knight = (KnightHero)hero;
		// Initialize HeroPowerUpCharged properties
		chargeSpeed = 0.5f;
		chargeState = "Special";
		// Initialize the new ability
		areaAttackAbility.Init(hero.player, DamageEnemy);
	}


	/*private void ActivateSuperShield()
	{
		// If the ability is already activated, we don't have to charge
		if (percentActivated >= 1.0f)
			return;
		// Charge
		percentActivated += chargeSpeed * Time.deltaTime;
		// Animation and Effects
		knight.player.animPlayer.willResetToDefault = false;
		knight.anim.Play("Special");
		chargeEffect.gameObject.SetActive(true);
		// On completed charge
		if (percentActivated >= 1.0f)
		{
			percentActivated = 1f;
			storedOnTap = knight.onTap;
			knight.onTap = SuperShield;
			indicatorEffect.gameObject.SetActive(true);
			chargeEffect.AnimateOut();
		}
	}*/

	protected override void ActivateEffect()
	{
		storedOnTap = knight.onTap;
		knight.onTap = SuperShield;
	}

	public void SuperShield()
	{
		// Check cooldown
		if (!knight.CheckIfCooledDownNotify(1))
			return;
		knight.ResetCooldownTimer(1);
		// Properties
		knight.AddShieldTimer(3f);
		areaAttackAbility.SetPosition(transform.position);
		areaAttackAbility.Execute();
		// Reset input action
		knight.onTap = storedOnTap;
		// Deactivate charge
		DeactivateEffect();
	}

	private void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
			stun.duration = 2.0f;
			print("Stunning enemy");
			e.AddStatus(stun.gameObject);
			knight.DamageEnemy(e, knight.damage, knight.hitEffect, true, knight.hitSounds);
		}
	}
}

