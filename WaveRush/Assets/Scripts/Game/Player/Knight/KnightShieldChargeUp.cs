using UnityEngine;
using System.Collections;

public class KnightShieldChargeUp : HeroPowerUp
{
	public KnightHero knight;
	public KnightShield shieldPowerUp;

	public override void Activate(PlayerHero hero)
	{
		base.Activate (hero);
		this.knight = (KnightHero)hero;
		shieldPowerUp = knight.GetComponentInChildren<KnightShield> ();
		shieldPowerUp.chargePerHit *= 1.5f;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		shieldPowerUp.chargePerHit = 1;
	}

	public override void Stack ()
	{
		base.Stack ();
		shieldPowerUp.chargePerHit *= 1.5f;
	}
}

