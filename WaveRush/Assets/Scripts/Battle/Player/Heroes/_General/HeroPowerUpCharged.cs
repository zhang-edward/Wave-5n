using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroPowerUpCharged : HeroPowerUp 
{
	public string chargeState;

	public IndicatorEffect chargeEffect;
	public IndicatorEffect indicatorEffect;

	public delegate void OnActivated();
	public event OnActivated onActivated;
	private bool chargeAnimationActive = false;

	public delegate void ChargeEvent();
	public event ChargeEvent OnStoppedCharging;

	public override void Activate(PlayerHero hero) {
		base.Activate(hero);
		percentActivated = 0;
	}

	protected void ChargePowerUp(float percent) {
		// If the ability is already activated, we don't have to charge
		if (percentActivated >= 1)
			return;
		
		if (!chargeAnimationActive)
			StartCoroutine(CheckCharging());

		percentActivated += percent;
		// On completed charge
		if (percentActivated >= 1) {
			percentActivated = 1f;
			ActivateEffect();
		}
	}

	private void StartChargeAnimation() {
		playerHero.anim.Play(chargeState);
		chargeEffect.gameObject.SetActive(true);
		chargeAnimationActive = true;
	}

	public void StopChargeAnimation() {
		chargeEffect.AnimateOut();
		chargeAnimationActive = false;
	}

	public void ResetCharge() {
		percentActivated = 0;
	}

	protected void DeactivateEffect() {
		StopChargeAnimation();
		percentActivated = 0f;
	}

	protected abstract void ActivateEffect();

	private IEnumerator CheckCharging() {
		StartChargeAnimation();
		float oldPercentActivated = -1;
		// When the percentActivated stops increasing, stop the charge animation
		while (oldPercentActivated < percentActivated) {
			oldPercentActivated = percentActivated;
			yield return null;
		}
		if (OnStoppedCharging != null)
			OnStoppedCharging();
		StopChargeAnimation();
	}
}
