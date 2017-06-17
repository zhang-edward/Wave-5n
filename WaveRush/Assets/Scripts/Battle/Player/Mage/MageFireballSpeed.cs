using UnityEngine;
using System.Collections;

public class MageFireballSpeed : HeroPowerUp
{
	private const float MULTIPLIER = 0.9f;

	private MageHero mage;
	private float totalSpeedMultiplier;		// the amount of speed that this powerup adds to the rush effect

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.mage = (MageHero)hero;
		mage.cooldownMultipliers[0] *= MULTIPLIER;
		totalSpeedMultiplier = MULTIPLIER;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		mage.cooldownMultipliers[0] /= totalSpeedMultiplier;
	}

	public override void Stack ()
	{
		base.Stack ();
		mage.cooldownMultipliers[0] *= MULTIPLIER;
		totalSpeedMultiplier *= MULTIPLIER;
	}
}

