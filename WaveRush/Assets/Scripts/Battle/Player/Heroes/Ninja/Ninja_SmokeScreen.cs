using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja_SmokeScreen : HeroPowerUp
{
	private bool activated;
	private NinjaHero ninja;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		ninja = (NinjaHero)hero;
		ninja.onParrySuccess += ActivateAbility;
		ninja.OnNinjaDash += SmokeBomb;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		ninja.onParrySuccess -= ActivateAbility;
		ninja.OnNinjaDash -= SmokeBomb;
	}

	private void ActivateAbility()
	{
		activated = true;
	}

	private void SmokeBomb()
	{
		if (activated)
		{
			ninja.SmokeBomb(1f);
			activated = false;
		}
	}
}
