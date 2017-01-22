using UnityEngine;
using System.Collections;

public class NinjaQuickStars : HeroPowerUp
{
	private NinjaHero ninja;
	private float totalMultiplier;

	public override void Activate (PlayerHero hero)
	{
		base.Activate (hero);
		ninja = (NinjaHero)hero;
		ninja.cooldownMultipliers [1] *= 0.85f;
		totalMultiplier *= 0.85f;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		ninja.cooldownMultipliers [1] /= totalMultiplier;
	}

	public override void Stack ()
	{
		base.Stack ();
		ninja.cooldownMultipliers [1] *= 0.85f;
		totalMultiplier *= 0.85f;
	}
}

