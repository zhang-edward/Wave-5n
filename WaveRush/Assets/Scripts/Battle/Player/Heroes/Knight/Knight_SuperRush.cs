using UnityEngine;
using PlayerActions;
using System.Collections;

public class Knight_SuperRush : HeroPowerUpCharged
{
	private const float PERCENT_CHARGE_PER_SECOND = 1.5f;
	private const float CHARGE_BUFFER_TIME = 0.3f;

	public PA_Rush rushAbility;

	private KnightHero knight;
	private PlayerHero.InputAction storedOnDragRelease;
	public float chargeBuffer;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		knight = (KnightHero)hero;
		knight.onDragHold += Charge;
		// Initialize the new ability
		rushAbility.Init(hero.player, DamageEnemy);
		OnStoppedCharging += ResetOnStopCharging;
	}

	private void Charge() {
		if (chargeBuffer < CHARGE_BUFFER_TIME)
			chargeBuffer += Time.deltaTime;
		else if (knight.cooldownTimers[0] > 0)
			return;
		else
			ChargePowerUp(PERCENT_CHARGE_PER_SECOND * Time.deltaTime);
	}

	private void ResetOnStopCharging() {
		chargeBuffer = 0;
		if (percentActivated < 1)
			ResetCharge();
	}
	protected override void ActivateEffect()
	{
		storedOnDragRelease = knight.onDragRelease;
		knight.onDragRelease = SuperRush;
	}

	public void SuperRush()
	{
		// check cooldown
		if (!knight.CheckIfCooledDownNotify (0, true, knight.HandleDragRelease))
			return;
		knight.ResetCooldownTimer (0);
		rushAbility.Execute();

		// Reset
		knight.onDragRelease = storedOnDragRelease;
		ResetCharge();		
	}

	private void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
			stun.duration = 2.0f;
			e.AddStatus(stun.gameObject);
			knight.DamageEnemy(e, (int)(knight.damage * 1.5f), knight.hitEffect, false, knight.hitSounds);
		}
	}
}

