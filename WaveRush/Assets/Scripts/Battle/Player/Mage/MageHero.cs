using UnityEngine;
using PlayerAbilities;
using Projectiles;
using System.Collections;

public class MageHero : PlayerHero {

	[HideInInspector]public ObjectPooler effectPool;
	[HideInInspector]public RuntimeObjectPooler projectilePool;
	[Header("Abilities")]
	public ShootProjectileAbility shootProjectileAbility;
	public Sprite hitEffect;
	public Map map;
	public GameObject projectilePrefab;
	private bool specialActivated;
	private AnimationSet defaultAnim;
	public AnimationSet magmaFormAnim;
	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;
	public AudioClip powerUpSound;
	public AudioClip powerDownSound;

	public delegate void MageAbilityActivated();
	public event MageAbilityActivated OnMageTeleportIn;
	public event MageAbilityActivated OnMageTeleportOut;
	public event MageAbilityActivated OnMageSpecialAbility;

	public Coroutine specialAbilityChargeRoutine;

	public delegate void MageCreatedObject (GameObject o);
	public event MageCreatedObject OnMageShotFireball;


	public override void Init(EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		map = GameObject.Find ("Map").GetComponent<Map>();
		projectilePool = (RuntimeObjectPooler)projectilePrefab.GetComponent<Projectile>().GetObjectPooler();
		shootProjectileAbility.Init(player, projectilePool);
		base.Init (body, player, heroData);

		onSwipe = ShootFireball;
		onTap = StartTeleport;
	}

	private void ShootFireball()
	{
		if (!IsCooledDown (0, true, HandleSwipe))
			return;
		ResetCooldownTimer (0);

		Vector2 dir = player.dir.normalized;
		Projectile fireball = shootProjectileAbility.ShootProjectile(dir);
		fireball.GetComponentInChildren<AreaDamageAction>().damage = damage;

		// recoil
		body.Move (dir);	// set the sprites flipX to the correct direction
		body.Rb2d.velocity = dir * -4f;

		// event
		if (OnMageShotFireball != null)
			OnMageShotFireball (fireball.gameObject);

		// Reset the ability
		Invoke ("ResetShootFireball", 0.5f);
	}

	public void ResetShootFireball()
	{
		body.moveSpeed = player.DEFAULT_SPEED;
	}

	public void StartTeleport()
	{
		if (!IsCooledDown (1))
			return;
		if (map.WithinOpenCells(player.transform.position + (Vector3)player.dir))
			StartCoroutine (Teleport ());
	}

	private IEnumerator Teleport()
	{
		ResetCooldownTimer (1);
		TeleportOut();
		// Wait for end of animation
		while (anim.player.isPlaying)
			yield return null;
		TeleportIn();
		// Wait for end of animation
		while (anim.player.isPlaying)
			yield return null;
		// reset properties
		player.isInvincible = false;
		player.input.isInputEnabled = true;
	}

	private void TeleportOut()
	{
		// Sound
		SoundManager.instance.RandomizeSFX(teleportOutSound);
		// Animation
		anim.Play("TeleOut");
		// Set properties
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		// event trigger
		if (OnMageTeleportOut != null)
			OnMageTeleportOut();
	}

	private void TeleportIn()
	{
		// Animation
		anim.Play("TeleIn");
		// Sound
		SoundManager.instance.RandomizeSFX(teleportInSound);
		// (animation triggers automatically)
		// Set position
		player.transform.parent.position = (Vector3)player.dir + player.transform.parent.position;
		// do area attack
		AreaAttack();
		// event trigger
		if (OnMageTeleportIn != null)
			OnMageTeleportIn();
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

	public override void SpecialAbility()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || specialActivated)
			return;
		anim = magmaFormAnim;
		anim.Play("Default");

	}

	// Damage an enemy and spawn an effect
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
			player.TriggerOnEnemyLastHitEvent (e);
		}
	}
}
