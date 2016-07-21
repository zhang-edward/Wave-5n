using UnityEngine;
using System.Collections;

public class KnightAbility : PlayerAbility {

	public GameObject rushEffect;
	public Sprite hitEffect;

	public bool killBox = false;
	public int damage = 1;

	public override void Ability()
	{
		// if cooldown has not finished
		if (abilityCooldown > 0)
			return;
		PlayEffect ();
		player.input.isInputEnabled = false;

		abilityCooldown = cooldownTime;
		killBox = true;
		body.moveSpeed = 8;
		body.Move(player.dir);
		player.isInvincible = true;
		anim.SetBool ("Attacking", true);
		Invoke ("ResetAbility", 0.5f);
	}

	public override void AbilityHoldDown ()
	{
	}

	public override void ResetAbility()
	{
		rushEffect.GetComponent<TempObject> ().Deactivate ();
		killBox = false;
		body.moveSpeed = player.DEFAULT_SPEED;

		player.isInvincible = false;
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

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Enemy"))
		{
			if (killBox)
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				if (!e.invincible && e.health > 0)
				{
					e.Damage (damage);
					/*Instantiate (hitEffect, 
						Vector3.Lerp (transform.position, e.transform.position, 0.5f), 
						Quaternion.identity);*/
					player.effectPool.GetPooledObject().GetComponent<TempObject>().Init(
						Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360f))),
						Vector3.Lerp (transform.position, e.transform.position, 0.5f), 
						hitEffect,
						true,
						0);

					player.TriggerOnEnemyDamagedEvent(damage);
				}
			}
		}
	}
}
