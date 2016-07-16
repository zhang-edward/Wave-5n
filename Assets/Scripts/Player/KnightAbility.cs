using UnityEngine;
using System.Collections;

public class KnightAbility : PlayerAbility {

	public GameObject rushEffect;

	public override void Ability()
	{
		// if cooldown has not finished
		if (abilityCooldown > 0)
			return;
		PlayEffect ();
		abilityCooldown = cooldownTime;
		player.killBox = true;
		body.moveSpeed = 7;
		player.input.isInputEnabled = false;
		anim.SetBool ("Attacking", true);
		Invoke ("ResetAbility", 0.8f);
	}

	public override void AbilityHoldDown ()
	{}

	public override void ResetAbility()
	{
		rushEffect.GetComponent<TempObject> ().Deactivate ();
		player.killBox = false;
		body.moveSpeed = player.DEFAULT_SPEED;
		player.input.isInputEnabled = true;
		anim.SetBool ("Attacking", false);
	}

	private void PlayEffect()
	{
		TempObject effect = rushEffect.GetComponent<TempObject> ();
		SimpleAnimationPlayer animPlayer = rushEffect.GetComponent<SimpleAnimationPlayer> ();

		Vector2 dir = player.dir;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		effect.Init (
			Quaternion.Euler(new Vector3(0, 0, angle)),
			transform.position,
			animPlayer.anim.frames[0],
			false
		);

		animPlayer.Play ();

	}
}
