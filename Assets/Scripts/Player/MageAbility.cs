using UnityEngine;
using System.Collections;

public class MageAbility : PlayerAbility {

	private ObjectPooler projectilePool;
	public Sprite projectileSprite;

	void Start()
	{
		projectilePool = ObjectPooler.GetObjectPooler ("Projectile");
	}

	public override void Ability()
	{
		// if cooldown has not finished
		if (abilityCooldown > 0)
			return;

		abilityCooldown = cooldownTime;

		GameObject o = projectilePool.GetPooledObject ();
		Projectile p = o.GetComponent<Projectile> ();
		p.Init (transform.position, body.Rb2d.velocity, projectileSprite, "Enemy", 5f, 1);

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
