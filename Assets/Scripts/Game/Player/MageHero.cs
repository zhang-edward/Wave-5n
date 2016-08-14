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
	private bool activatedSpecialAbility;

	private bool isInFireSpreadMode = false;
	public MageFire mageFirePrefab;

	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;
	public AudioClip powerUpSound;
	public AudioClip powerDownSound;

	//private float chargeTime;
	//private bool sprayingFire;
	private float tapHoldTime;
	private const float minTapHoldTime = 0.2f;

	public override void Init(EntityPhysics body, Animator anim, Player player)
	{
		abilityCooldowns = new float[2];
		base.Init (body, anim, player);
		map = GameObject.Find ("Map").GetComponent<Map>();
		heroName = PlayerHero.MAGE;
		projectilePool = ObjectPooler.GetObjectPooler ("PlayerProjectile") as RuntimeObjectPooler;
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

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || activatedSpecialAbility)
			return;
		// Sound
		SoundManager.instance.PlayImportantSound(powerUpSound);
		activatedSpecialAbility = true;
		isInFireSpreadMode = true;

		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (new Color(1, 0.2f, 0), 0.2f);
		CameraControl.instance.StartShake (0.3f, 0.05f);

		Invoke ("ResetSpecialAbility", 10.0f);
	}
		
	private void ResetSpecialAbility()
	{
		// Sound
		SoundManager.instance.PlayImportantSound(powerDownSound);

		activatedSpecialAbility = false;
		specialAbilityCharge = 0;
		isInFireSpreadMode = false;

		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (Color.clear, 0f);
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
		body.Rb2d.velocity = dir * -4f;

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
		if (isInFireSpreadMode)
			CreateFire ();
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

	private void CreateFire()
	{
		Instantiate (mageFirePrefab, transform.position, Quaternion.identity);
	}

	// damage an enemy and spawn an effect
	private void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			player.effectPool.GetPooledObject ().GetComponent<TempObject> ().Init (
				Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360f))),
				e.transform.position, 
				hitEffect,
				true,
				0,
				0.2f,
				1.0f);

			player.TriggerOnEnemyDamagedEvent(damage);
		}
	}
}
