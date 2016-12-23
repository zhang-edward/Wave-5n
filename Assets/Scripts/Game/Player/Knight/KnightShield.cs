using UnityEngine;
using System.Collections;

public class KnightShield : HeroPowerUp
{
	public KnightHero knight;
	public bool shielded;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		this.knight = (KnightHero)hero;
		knight.rushMoveSpeed *= 1.1f;
		shielded = true;
		knight.player.OnPlayerDamaged += AbsorbDamage;
	}

	public void AbsorbDamage(int amt)
	{
		if (!shielded)
			return;

		knight.player.Heal (amt);
		Deactivate ();
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		shielded = false;
	}

	public override void Stack ()
	{
		base.Stack ();
	}
}

