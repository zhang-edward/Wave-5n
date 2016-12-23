using UnityEngine;
using System.Collections;

public class KnightRushPowerUp : HeroPowerUp
{
	public KnightHero knight;
	private float totalSpeedMultiplier;		// the amount of speed that this powerup adds to the rush effect

	public override void Activate(PlayerHero hero)
	{
		this.knight = (KnightHero)hero;
		knight.rushMoveSpeed *= 1.1f;
		totalSpeedMultiplier = 1.1f;
	}

	public override void Deactivate ()
	{
		knight.rushMoveSpeed /= totalSpeedMultiplier;
	}

	public override void Stack ()
	{
		base.Stack ();
		knight.rushMoveSpeed *= 1.1f;
		totalSpeedMultiplier *= 1.1f;
	}
}

