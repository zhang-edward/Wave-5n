using UnityEngine;
using System.Collections;

public class KnightHero : PlayerHero {

	[Header("Class-Specific")]
	public GameObject rushEffect;
	public GameObject areaAttackEffect;
	public Sprite hitEffect;
	public float areaAttackRange = 2.0f;

	private bool killBox = false;

	[Header("Audio")]
	public AudioClip rushSound;
	public AudioClip[] hitSounds;
	public AudioClip areaAttackSound;

	public void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 1f);
	}

	public override void Init (Player player, EntityPhysics body, Animator anim)
	{
		abilityCooldowns = new float[2];
		base.Init (player, body, anim);
		heroName = PlayerHero.KNIGHT;
	}

	// Dash attack
	public override void HandleSwipe()
	{
		// if cooldown has not finished
		if (abilityCooldowns[0] > 0)
			return;
		ResetCooldown (0);
		// Sound
		SoundManager.instance.RandomizeSFX (rushSound);
		// Animation
		anim.SetBool ("Attacking", true);
		// Effects
		PlayRushEffect ();
		// Player properties
		player.input.isInputEnabled = false;
		killBox = true;
		body.moveSpeed = 10;
		body.Move(player.dir.normalized);
		player.isInvincible = true;
		// reset ability
		Invoke ("ResetDashAbility", 0.5f);
	}

	// Area attack
	public override void HandleTapRelease ()
	{
		if (abilityCooldowns [1] > 0)
			return;
		ResetCooldown (1);
		// Sound
		SoundManager.instance.RandomizeSFX (areaAttackSound);
		// Animation
		anim.SetTrigger ("AreaAttack");
		// Effects
		PlayAreaAttackEffect ();
		// Properties
		player.isInvincible = true;
		body.Move (Vector2.zero);
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, areaAttackRange);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				DamageEnemy (e);
			}
		}
		// Reset Ability
		Invoke ("ResetAreaAttackAbility", 0.5f);
	}

	public void ResetDashAbility()
	{
		rushEffect.GetComponent<TempObject> ().Deactivate ();
		killBox = false;
		body.moveSpeed = player.DEFAULT_SPEED;

		player.isInvincible = false;
		player.input.isInputEnabled = true;
		anim.SetBool ("Attacking", false);
	}

	public void ResetAreaAttackAbility()
	{
		anim.SetBool ("AreaAttack", false);
		player.isInvincible = false;
	}

	private void PlayRushEffect()
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

	private void PlayAreaAttackEffect()
	{
		TempObject effect = areaAttackEffect.GetComponent<TempObject> ();
		SimpleAnimationPlayer animPlayer = areaAttackEffect.GetComponent<SimpleAnimationPlayer> ();

		TempObjectInfo info = new TempObjectInfo ();
		info.isSelfDeactivating = true;
		info.lifeTime = animPlayer.anim.TimeLength;
		info.targetColor = new Color (1, 1, 1, 1f);
		effect.Init (
			Quaternion.identity,
			transform.position,
			animPlayer.anim.frames[0],
			info
		);
		animPlayer.Play ();
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (killBox)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				DamageEnemy (e);
			}
		}
	}

	private void DamageEnemy(Enemy e)
	{
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
