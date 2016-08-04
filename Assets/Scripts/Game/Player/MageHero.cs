using UnityEngine;
using System.Collections;

public class MageHero : PlayerHero {

	[Header("Class-Specific")]
	public ObjectPooler effectPool;
	public RuntimeObjectPooler projectilePool;
	[Space]
	public Sprite projectileSprite;
	public Sprite hitEffect;
	public Map map;
	public GameObject projectilePrefab;

	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;

	//private float chargeTime;
	//private bool sprayingFire;
	private float tapHoldTime;
	private const float minTapHoldTime = 0.2f;

	public override void Init(Player player, EntityPhysics body, Animator anim)
	{
		abilityCooldowns = new float[2];
		base.Init (player, body, anim);
		heroName = PlayerHero.MAGE;
		projectilePool.SetPooledObject(projectilePrefab);
	}

	public override void HandleSwipe ()
	{
		ShootFireball ();
	}

	public override void HandleTapRelease()
	{
		StartTeleport ();
	}
		
	private void ShootFireball()
	{
		//chargeTime = 0f;
		// if cooldown has not finished
		if (abilityCooldowns[0] > 0)
			return;
		ResetCooldown (0);
		// Sound
		SoundManager.instance.RandomizeSFX (shootSound);
		// Animation
		anim.SetBool ("Charge", false);
		anim.SetBool ("Attack", true);

		// actual projectile stuff
		GameObject o = projectilePool.GetPooledObject ();
		Vector2 dir = player.dir.normalized;
		PlayerProjectile fireball = o.GetComponent<PlayerProjectile> ();
		fireball.Init (transform.position, dir, player);

		// recoil
		body.Move (dir);	// set the sprites flipX to the correct direction
		body.Rb2d.velocity = dir * -2f;

		// Reset the ability
		Invoke ("ResetShootFireball", 0.5f);
	}

	public void ResetShootFireball()
	{
		body.moveSpeed = player.DEFAULT_SPEED;
		anim.SetBool("Attack", false);
		anim.SetTrigger ("Move");
	}

	private void StartTeleport()
	{
		if (map.WithinOpenCells(player.transform.position + (Vector3)player.dir) &&
			abilityCooldowns[1] <= 0)
			StartCoroutine (Teleport ());
	}

	private IEnumerator Teleport()
	{
		ResetCooldown (1);
		// Sound
		SoundManager.instance.RandomizeSFX (teleportOutSound);
		// Animation
		anim.SetTrigger ("TeleOut");
		// Set properties
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		// Wait for end of animation
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("TeleportOut"))
			yield return null;
		// Sound
		SoundManager.instance.RandomizeSFX (teleportInSound);
		// (animation triggers automatically)
		// Set position
		player.transform.parent.position = (Vector3)player.dir + player.transform.parent.position;
		// do area attack
		AreaAttack ();
		// Wait for end of animation
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("TeleportIn"))
			yield return null;
		// reset properties
		player.isInvincible = false;
		player.input.isInputEnabled = true;
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

	// damage an enemy and spawn an effect
	private void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			player.effectPool.GetPooledObject ().GetComponent<TempObject> ().Init (
				Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360f))),
				transform.position, 
				hitEffect,
				true,
				0,
				0.2f,
				0.4f);

			player.TriggerOnEnemyDamagedEvent(damage);
		}
	}
}
