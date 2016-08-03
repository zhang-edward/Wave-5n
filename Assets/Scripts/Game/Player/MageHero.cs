using UnityEngine;
using System.Collections;

public class MageHero : PlayerHero {

	[Header("Class-Specific")]
	public ObjectPooler effectPool;
	public ObjectPooler projectilePool;
	public Sprite projectileSprite;
	public Sprite hitEffect;
	public SimpleAnimation fireEffectAnimation;
	public SimpleAnimation fireballAnim;
	public Map map;

	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;

	//private float chargeTime;
	private bool sprayingFire;
	private float tapHoldTime;
	private const float minTapHoldTime = 0.2f;

	public override void Init(Player player, EntityPhysics body, Animator anim)
	{
		abilityCooldowns = new float[2];
		base.Init (player, body, anim);
		heroName = PlayerHero.MAGE;
	}

	public override void HandleSwipe ()
	{
		ShootFireball ();
	}

	public override void HandleTapRelease()
	{
		//tapHoldTime = 0;
		/*if (sprayingFire)
		{
			ResetAbility ();
		}*/
		//else
		//if (!sprayingFire)
			StartTeleport ();
	}

	public override void HandleHoldDown ()
	{
		/*tapHoldTime += Time.deltaTime;
		if (tapHoldTime > minTapHoldTime && !sprayingFire)
		{
			SprayFire ();
		}*/
	}

	private void SprayFire()
	{
		// if cooldown has not finished
		if (abilityCooldowns[0] > 0)
			return;
		ResetCooldown (0);
		SoundManager.instance.RandomizeSFX (shootSound);
		StartCoroutine ("SprayFireEffect");
		Invoke ("ResetAbility", 1.0f);
	}

	private IEnumerator SprayFireEffect()
	{
		// Coroutine stopped in ResetAbility
		sprayingFire = true;
		anim.SetBool ("Attack", true);
		while (sprayingFire)
		{
			Vector2 dir = player.dir.normalized;
			body.Move (dir);
			body.Rb2d.velocity = dir * -1f;

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

	private void ShootFireball()
	{
		//chargeTime = 0f;
		// if cooldown has not finished
		if (abilityCooldowns[0] > 0)
			return;
		ResetCooldown (0);

		Vector2 dir = player.dir.normalized;
		anim.SetBool ("Charge", false);
		SoundManager.instance.RandomizeSFX (shootSound);
		GameObject o = projectilePool.GetPooledObject ();
		PlayerProjectile p = o.GetComponent<PlayerProjectile> ();
		SimpleAnimationPlayer pAnim = o.GetComponent<SimpleAnimationPlayer> ();
		pAnim.anim = fireballAnim;
		body.Move (dir);
		body.Rb2d.velocity = dir * -2f;

		p.Init (transform.position, dir, projectileSprite, "Enemy", player, 3f, damage);
		anim.SetBool ("Attack", true);
		Debug.Log ("hello");
		Invoke ("ResetAbility", 0.5f);
	}

	private void StartTeleport()
	{
		if (map.WithinOpenCells(player.transform.position + (Vector3)player.dir) &&
			abilityCooldowns[1] <= 0)
			StartCoroutine (Teleport ());
	}
		
	public void ResetAbility()
	{
		//dirIndicator.gameObject.SetActive (false);
		sprayingFire = false;
		body.moveSpeed = player.DEFAULT_SPEED;
		StopCoroutine ("SprayFireEffect");
		//anim.ResetTrigger ("Charge");
		anim.SetBool("Attack", false);
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
				transform.position, 
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
