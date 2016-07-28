using UnityEngine;
using System.Collections;

public class KnightHero : PlayerHero {

	[Header("Class-Specific")]
	public GameObject rushEffect;
	public Sprite hitEffect;

	private bool killBox = false;

	[Header("Audio")]
	public AudioClip rushSound;
	public AudioClip[] hitSounds;

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
		SoundManager.instance.RandomizeSFX (rushSound);
		Invoke ("ResetAbility", 0.5f);
	}

	public override void AbilityHoldDown ()
	{}

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
		TempObjectInfo info = new TempObjectInfo ();
		info.isSelfDeactivating = false;
		info.targetColor = new Color (1, 1, 1, 0.5f);
		effect.Init (
			Quaternion.Euler(new Vector3(0, 0, angle)),
			transform.position,
			animPlayer.anim.frames[0],
			info
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

					SoundManager.instance.RandomizeSFX (hitSounds[Random.Range(0, hitSounds.Length)]);
					player.TriggerOnEnemyDamagedEvent(damage);
				}
			}
		}
	}
}
