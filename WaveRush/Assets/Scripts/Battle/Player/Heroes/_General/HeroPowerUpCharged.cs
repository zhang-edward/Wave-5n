using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroPowerUpCharged : HeroPowerUp 
{
	private const float TAPHOLD_BUFFER = 0.3f;

	protected float  chargeSpeed = 0.5f;
	protected string chargeState = "Default";

	public IndicatorEffect chargeEffect;
	public IndicatorEffect indicatorEffect;

	public delegate void OnActivated();
	public event OnActivated onActivated;

	private float chargeBuffer;		// The player should hold tap for it this ability to begin charging.
									// Prevents tiny incremental charges when the player inputs regular taps


	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		// Assign new inputs
		playerHero.onTapHoldDown += ChargePowerUp;
		playerHero.onDragBegan += StopCharging;
		playerHero.onTapHoldRelease += StopCharging;
		percentActivated = 0;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		playerHero.onTapHoldDown -= ChargePowerUp;
		playerHero.onDragBegan -= StopCharging;
		playerHero.onTapHoldRelease -= StopCharging;
	}

	private void ChargePowerUp()
	{
		// Charge buffer
		/*if (chargeBuffer < TAPHOLD_BUFFER)
		{
			chargeBuffer += Time.deltaTime;
			return;
		}*/
		// If the ability is already activated, we don't have to charge
		if (percentActivated >= 1.0f)
			return;
		// Charge
		percentActivated += chargeSpeed * Time.deltaTime;
		// Animation and Effects
		playerHero.player.animPlayer.willResetToDefault = false;
		if (!playerHero.player.animPlayer.IsPlayingAnimation(chargeState))
			playerHero.anim.Play(chargeState);
		chargeEffect.gameObject.SetActive(true);
		// On completed charge
		if (percentActivated >= 1.0f)
		{
			percentActivated = 1f;
			chargeEffect.AnimateOut();
			indicatorEffect.gameObject.SetActive(true);
			if (onActivated != null)
				onActivated();
			ActivateEffect();
		}
	}

	private void StopCharging()
	{
		chargeBuffer = 0;
		playerHero.player.animPlayer.ResetToDefault();
		chargeEffect.gameObject.SetActive(false);
	}

	protected void DeactivateEffect()
	{
		indicatorEffect.AnimateOut();
		percentActivated = 0f;
	}

	protected abstract void ActivateEffect();
}
