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


	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		hero.onTapHoldDown += Animate;
		hero.onTapHoldRelease += Activate;
		percentActivated = 0;
	}

	protected void ChargePowerUp(float percent)
	{
		// If the ability is already activated, we don't have to charge
		if (percentActivated >= 1)
			return;
		// Charge
		percentActivated += percent;
		// On completed charge
		if (percentActivated >= 1)
		{
			percentActivated = 1f;
			indicatorEffect.gameObject.SetActive(true);
		}
	}

	private void Animate()
	{
		if (percentActivated < 1)
			return;
		if (!playerHero.player.animPlayer.IsPlayingAnimation(chargeState))
			playerHero.anim.Play(chargeState);
		chargeEffect.gameObject.SetActive(true);
	}

	private void Activate()
	{
		if (percentActivated < 1)
			return;
		if (onActivated != null)
			onActivated();
		ActivateEffect();
		chargeEffect.AnimateOut();
		indicatorEffect.gameObject.SetActive(true);
	}

	protected void DeactivateEffect()
	{
		indicatorEffect.AnimateOut();
		percentActivated = 0f;
	}

	protected abstract void ActivateEffect();
}
