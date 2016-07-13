using UnityEngine;
using System.Collections;

public class KnightAbility : PlayerAbility {

	public override void Ability()
	{
		// if cooldown has not finished
		if (abilityCooldown > 0)
			return;

		abilityCooldown = cooldownTime;
		player.killBox = true;
		body.moveSpeed = 10;
		player.input.isInputEnabled = false;
		anim.SetBool ("Attacking", true);
		Invoke ("ResetAbility", 0.3f);
	}

	public override void AbilityHoldDown ()
	{}

	public override void ResetAbility()
	{
		player.killBox = false;
		body.moveSpeed = player.DEFAULT_SPEED;
		player.input.isInputEnabled = true;
		anim.SetBool ("Attacking", false);
	}
}
