using UnityEngine;
using System.Collections;

public class MageAbility : PlayerAbility {

	public ObjectPooler projectilePool;
	public Sprite projectileSprite;

	public override void Ability()
	{
		// if cooldown has not finished
		if (abilityCooldown > 0)
			return;

		abilityCooldown = cooldownTime;

		Player player = this.GetComponentInParent<Player> ();

		GameObject o = projectilePool.GetPooledObject ();
		MageProjectile p = o.GetComponent<MageProjectile> ();
		p.Init (transform.position, body.Rb2d.velocity, projectileSprite, "Enemy", player, 5f, 1);

		anim.SetTrigger ("Attack");
		Invoke ("ResetAbility", 0.5f);
	}

	public override void AbilityHoldDown ()
	{
		if (abilityCooldown > 0)
			return;
		anim.SetTrigger ("Charge");

		body.moveSpeed = 0.5f;
	}

	public override void ResetAbility()
	{
		body.moveSpeed = player.DEFAULT_SPEED;
		anim.ResetTrigger ("Charge");
		anim.SetTrigger ("Move");
	}
}
