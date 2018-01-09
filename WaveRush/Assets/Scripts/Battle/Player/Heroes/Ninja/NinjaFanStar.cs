using UnityEngine;
using System.Collections;

public class NinjaFanStar : HeroPowerUp
{
	private NinjaHero ninja;
	private float activateChance;
	//private bool activated;

	public override void Activate (PlayerHero hero)
	{
		base.Activate (hero);
		ninja = (NinjaHero)hero;
		//ninja.OnNinjaThrewStar += ActivateFanStar;
		activateChance = 0.1f;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		activateChance = 0;
		//ninja.OnNinjaThrewStar -= ActivateFanStar;
	}

	public override void Stack ()
	{
		base.Stack ();
		activateChance += 0.08f;
	}

	/*public void ActivateFanStar()
	{
		if (activated)
		{
			ninja.OnNinjaThrewStar -= ninja.ShootNinjaStarFanPattern;
			activated = false;
		}
		if (Random.value < activateChance)
		{
			ninja.OnNinjaThrewStar += ninja.ShootNinjaStarFanPattern;
			activated = true;
		}
	}*/
}

