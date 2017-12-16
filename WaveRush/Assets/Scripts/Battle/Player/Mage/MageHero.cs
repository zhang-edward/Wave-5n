using UnityEngine;
using PlayerAbilities;
using Projectiles;
using System.Collections;

public class MageHero : PlayerHero {

	[HideInInspector]public RuntimeObjectPooler projectilePool;
	[Header("Abilities")]
	public RushAbility specialRushAbility;
	public ShootProjectileAbility shootProjectileAbility;
	[Header("Prefabs")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation specialHitEffect;
	public GameObject projectilePrefab;
	private float teleportRange = 4.0f;
	private bool specialActivated;
	private float specialRushCooldown = 0.5f;
	private float specialRushCooldowntimer = 0;
	private AnimationSet defaultAnim;
	[Header("Audio")]
	public AudioClip specialHitSound;
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;
	public AudioClip powerUpSound;
	public AudioClip transformSound;
	public AudioClip powerDownSound;
	[Header("Etc")]
	public Map map;
	public AnimationSet magmaFormAnim;
	public SimpleAnimationPlayer transformEffect;
	public IndicatorEffect teleportIndicator;

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
		specialRushAbility.Init(player, DamageEnemySpecial);
		base.Init (body, player, heroData);

		defaultAnim = anim;
		onDrag = ShootFireball;
		onTap = StartTeleport;
	}

	protected override void ParryEffect()
	{
		StartCoroutine(ParryEffectRoutine());
	}

	private IEnumerator ParryEffectRoutine()
	{
		SetAnimationSet(magmaFormAnim);
		player.dir = UtilMethods.DegreeToVector2(Random.Range(0, 360f));
		specialRushAbility.speed /= 2;
		specialRushAbility.Execute();
		player.input.isInputEnabled = false;
//		Debug.Log("Doing this!");
		yield return new WaitForSeconds(specialRushAbility.duration - 0.1f);
		specialRushAbility.speed *= 2;
		SetAnimationSet(defaultAnim);
//		Debug.Log("Doing this now!");
		yield return new WaitForSeconds(0.1f);
		player.input.isInputEnabled = true;
	}

	private void ShootFireball()
	{
		if (!CheckIfCooledDownNotify (0, true, HandleDrag))
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
		if (!CheckIfCooledDownNotify (1))
			return;
		player.dir = Vector2.ClampMagnitude(player.dir, teleportRange);
		if (CanTeleport(player.transform.position + (Vector3)player.dir))
			StartCoroutine (Teleport ());
	}

	private bool CanTeleport(Vector3 dest)
	{
		return map.WithinOpenCells(dest);
	}

	private IEnumerator Teleport()
	{
		ResetCooldownTimer (1);
		TeleportOut();
		teleportIndicator.transform.position = transform.position + (Vector3)player.dir;
		teleportIndicator.gameObject.SetActive(true);
		// Wait for end of animation
		while (anim.player.isPlaying)
			yield return null;
		teleportIndicator.gameObject.SetActive(false);
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
		StartCoroutine(SpecialAbilityRoutine());	
	}

	private IEnumerator SpecialAbilityRoutine()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || specialActivated)
			yield break;
		specialActivated = true;
		transformEffect.gameObject.SetActive(true);
		transformEffect.Play();
		sound.PlaySingle(powerUpSound);
		CameraControl.instance.StartShake(0.2f, 0.05f, true, false);
		player.isInvincible = true;
		yield return new WaitForSeconds(transformEffect.anim.SecondsPerFrame * 9f);
		sound.PlaySingle(transformSound);
		SetAnimationSet(magmaFormAnim);
		onDrag = SpecialRush;
		onTap = null;
		while (transformEffect.isPlaying)
			yield return null;
		transformEffect.gameObject.SetActive(false);

		if (OnMageSpecialAbility != null)
			OnMageSpecialAbility();

		yield return new WaitForSeconds(7f);
		sound.PlaySingle(powerDownSound);
		ResetSpecialAbility();
	}

	public void SpecialRush()
	{
		if (specialRushCooldowntimer > 0)
			return;
		specialRushCooldowntimer = specialRushCooldown;

		specialRushAbility.Execute();
	}

	public void ResetSpecialAbility()
	{
		SetAnimationSet(defaultAnim);
		onDrag = ShootFireball;
		onTap = StartTeleport;
		specialActivated = false;
		player.isInvincible = false;
		specialAbilityCharge = 0;
	}

	protected override void Update()
	{
		base.Update();
		if (specialRushCooldowntimer > 0)
			specialRushCooldowntimer -= Time.deltaTime;
	}

	private void DamageEnemySpecial(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			sound.RandomizeSFX(specialHitSound);
			DamageEnemy(e);
		}
	}

	// Damage an enemy and spawn an effect
	private void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			if (specialActivated)
				EffectPooler.PlayEffect(specialHitEffect, e.transform.position, true);
			else
				EffectPooler.PlayEffect(hitEffect, e.transform.position, true, 0.2f);

			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent (e);
		}
	}


	// ========== Helper Methods ==========

	private void SetAnimationSet(AnimationSet animSet)
	{
		animSet.Init(player.animPlayer);
		anim = animSet;
		anim.Play("Default");
	}
}
