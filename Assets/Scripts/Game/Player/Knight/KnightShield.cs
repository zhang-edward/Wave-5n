using UnityEngine;
using System.Collections;

public class KnightShield : HeroPowerUp
{
	public KnightHero knight;
	public bool shielded;

	public override void Activate(PlayerHero hero)
	{
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
		shielded = false;
	}

	public override void Deactivate ()
	{
		knight.rushMoveSpeed /= 1.1f;
	}

	public override void Stack ()
	{
		base.Stack ();
		knight.rushMoveSpeed *= 1.1f;
	}
}

