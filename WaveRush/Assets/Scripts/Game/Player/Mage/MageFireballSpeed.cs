using UnityEngine;
using System.Collections;

public class MageFireballSpeed : HeroPowerUp
{
	private const float MULTIPLIER = 1.1f;

	private MageHero mage;
	private float totalSpeedMultiplier;		// the amount of speed that this powerup adds to the rush effect

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.mage = (MageHero)hero;
		mage.fireballSpeedMultiplier *= MULTIPLIER;
		totalSpeedMultiplier = MULTIPLIER;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		mage.fireballSpeedMultiplier /= totalSpeedMultiplier;
	}

	public override void Stack ()
	{
		base.Stack ();
		mage.fireballSpeedMultiplier *= MULTIPLIER;
		totalSpeedMultiplier *= MULTIPLIER;
	}
}

