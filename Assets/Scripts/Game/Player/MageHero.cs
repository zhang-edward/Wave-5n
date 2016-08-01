using UnityEngine;
using System.Collections;

public class MageHero : PlayerHero {

	[Header("Class-Specific")]
	public ObjectPooler effectPool;
	public Sprite projectileSprite;
	public Sprite hitEffect;
	public SimpleAnimation fireEffectAnimation;
	public Map map;

	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;

	public override void Init(Player player, EntityPhysics body, Animator anim)
	{
		abilityCooldowns = new float[2];
		base.Init (player, body, anim);
	}

	public override void HandleSwipe (Vector2 dir)
	{
		base.HandleSwipe (dir);
		SprayFire (dir);
	}

	public override void HandleTapRelease()
	{
		StartTeleport ();
	}

	private void SprayFire(Vector2 dir)
	{
		// if cooldown has not finished
		if (abilityCooldowns[0] > 0)
			return;
		ResetCooldown (0);
		SoundManager.instance.RandomizeSFX (shootSound);
		StartCoroutine ("SprayFireEffect", dir);

		anim.SetTrigger ("Attack");
		Invoke ("ResetAbility", 0.3f);
	}

	private IEnumerator SprayFireEffect(Vector2 dir)
	{
		// Coroutine stopped in ResetAbility
		while (true)
		{
			body.Move (dir);
			body.Rb2d.velocity = -dir * 5f;

			RaycastHit2D[] hits = Physics2D.CircleCastAll (transform.position, 0.5f, dir, 2.0f);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.CompareTag("Enemy"))
				{
					Enemy e = hit.collider.GetComponentInChildren<Enemy> ();
					DamageEnemy (e);
				}
			}
			FireEffect (dir);
			yield return new WaitForSeconds (0.08f);
		}
	}

	private void FireEffect(Vector2 dir)
	{
		TempObject effect = effectPool.GetPooledObject().GetComponent<TempObject> ();
		SimpleAnimationPlayer animPlayer = effect.GetComponent<SimpleAnimationPlayer> ();
		animPlayer.anim = fireEffectAnimation;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		angle += Random.Range (-20f, 20f);
		Vector3 randOffset = new Vector3 (Random.Range (-0.3f, 0.3f), Random.Range (-0.3f, 0.3f));

		TempObjectInfo info = new TempObjectInfo ();
		info.isSelfDeactivating = true;
		info.fadeInTime = 0.1f;
		info.fadeOutTime = fireEffectAnimation.TimeLength;
		effect.Init (
			Quaternion.Euler(new Vector3(0, 0, angle)),
			transform.position + (Vector3)dir + randOffset,
			animPlayer.anim.frames[0],
			info
		);
		animPlayer.Play ();
	}

/*	private void ShootFireball(Vector2 dir)
	{
		// if cooldown has not finished
		if (abilityCooldowns[0] > 0)
			return;
		ResetCooldown (0);

		SoundManager.instance.RandomizeSFX (shootSound);
		GameObject o = projectilePool.GetPooledObject ();
		PlayerProjectile p = o.GetComponent<PlayerProjectile> ();
		body.Move (dir);
		body.Rb2d.velocity = -dir * 3f;

		p.Init (transform.position, dir, projectileSprite, "Enemy", player, map, 5f, damage);
		anim.SetTrigger ("Attack");
		Invoke ("ResetAbility", 0.5f);
	}*/

	private void StartTeleport()
	{
		if (map.WithinOpenCells(player.transform.position + (Vector3)player.dir) &&
			abilityCooldowns[1] <= 0)
			StartCoroutine (Teleport ());
	}

	public void ResetAbility()
	{
		//dirIndicator.gameObject.SetActive (false);
		body.moveSpeed = player.DEFAULT_SPEED;
		StopCoroutine ("SprayFireEffect");
		//anim.ResetTrigger ("Charge");
		anim.SetTrigger ("Move");
	}

	private IEnumerator Teleport()
	{
		ResetCooldown (1);
		anim.SetTrigger ("TeleOut");
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		// play teleportOut animation
		SoundManager.instance.RandomizeSFX (teleportOutSound);
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("TeleportOut"))
			yield return null;
		player.transform.parent.position = (Vector3)player.dir + player.transform.parent.position;

		// play teleportIn animation
		SoundManager.instance.RandomizeSFX (teleportInSound);
		// do area attack
		AreaAttack ();
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("TeleportIn"))
			yield return null;
		player.isInvincible = false;
		player.input.isInputEnabled = true;
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

			player.TriggerOnEnemyDamagedEvent(damage);
		}
	}

	private void AreaAttack()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 1.5f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				DamageEnemy (e);
			}
		}
	}
}
