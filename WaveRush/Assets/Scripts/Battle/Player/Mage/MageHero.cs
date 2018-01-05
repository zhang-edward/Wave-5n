using UnityEngine;
using PlayerActions;
using Projectiles;
using System.Collections;

public class MageHero : PlayerHero {

	/// <summary>
	/// Shoot a meteor projectile.
	/// NOTE: The target must be set with <see cref="PA_ShootProjectile.SetProjectileDirection"/> 
	/// before running <see cref="PlayerAction.Execute"/>.
	/// </summary>
	public class PA_ShootMeteorProjectile : PA_ShootProjectile
	{
		protected override void DoAction()
		{
			Vector3 origin = projectileDir + new Vector3(Random.Range(-3f, 3f), 10);
			SetProjectileOrigin(origin);
			SetProjectileDirection(projectileDir - origin);
			base.DoAction();
		}
	}

	private const float TELEPORT_RANGE = 4.0f;
	private const float SPECIAL_NUM_METEORS = 10f;

	private RuntimeObjectPooler fireballPool;
	private RuntimeObjectPooler meteorPool;

	[Header("Abilities")]
	//public PA_Rush specialRushAbility;
	public PA_ShootProjectile shootProjectileAbility;
	public PA_SpecialAbilityEffect specialAbilityEffect;
	public PA_ShootMeteorProjectile shootMeteorAbility = new PA_ShootMeteorProjectile();
	public PA_Teleport teleportAbility;
	[Header("Prefabs")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation specialHitEffect;
	public GameObject projectilePrefab;
	public GameObject meteorPrefab;
	private bool  specialActivated;
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
	public AudioClip spawnMeteorSound;
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
		fireballPool = (RuntimeObjectPooler)projectilePrefab.GetComponent<Projectile>().GetObjectPooler();
		meteorPool = (RuntimeObjectPooler)meteorPrefab.GetComponent<Projectile>().GetObjectPooler();

		base.Init (body, player, heroData);

		defaultAnim = anim;
		onDrag = ShootFireball;
		onTap = StartTeleport;
		InitAbilities();
	}

	private void InitAbilities()
	{
		shootProjectileAbility.Init(player, fireballPool);
		specialAbilityEffect.Init(player);
		shootMeteorAbility.Init(player, meteorPool);
		//specialRushAbility.Init(player, DamageEnemySpecial);
		teleportAbility.Init(player);
	}

	protected override void ParryEffect()
	{
		StartCoroutine(ParryEffectRoutine());
	}

	private IEnumerator ParryEffectRoutine()
	{
		SetAnimationSet(magmaFormAnim);
		player.dir = UtilMethods.DegreeToVector2(Random.Range(0, 360f));
		//specialRushAbility.movement.speed /= 2;
		//specialRushAbility.Execute();
		player.input.isInputEnabled = false;
		//yield return new WaitForSeconds(specialRushAbility.duration - 0.1f);
		//specialRushAbility.movement.speed *= 2;
		SetAnimationSet(defaultAnim);
		yield return new WaitForSeconds(0.1f);
		player.input.isInputEnabled = true;
	}

	private void ShootFireball()
	{
		if (!CheckIfCooledDownNotify (0, true, HandleDrag))
			return;
		ResetCooldownTimer (0);

		shootProjectileAbility.SetProjectileOrigin(player.transform.position);
		shootProjectileAbility.SetProjectileDirection(player.dir.normalized);
		shootProjectileAbility.Execute();
		Projectile fireball = shootProjectileAbility.GetProjectile();
		fireball.GetComponentInChildren<AreaDamageAction>().damage = damage;

		/** Recoil */
		Vector2 dir = player.dir.normalized;
		body.Move (dir);	// set the sprites flipX to the correct direction
		body.rb2d.velocity = dir * -4f;

		/** Event */
		if (OnMageShotFireball != null)
			OnMageShotFireball (fireball.gameObject);
	}

	public void StartTeleport()
	{
		if (!CheckIfCooledDownNotify (1))
			return;
		player.dir = Vector2.ClampMagnitude(player.dir, TELEPORT_RANGE);
		teleportAbility.OnTeleportIn += DisableTeleportIndicator;
		teleportIndicator.transform.position = transform.position + (Vector3)player.dir;
		teleportIndicator.gameObject.SetActive(true);
		if (CanTeleport(player.transform.position + (Vector3)player.dir))
			teleportAbility.Execute();
		ResetCooldownTimer(1);
	}

	private bool CanTeleport(Vector3 dest)
	{
		return map.WithinOpenCells(dest);
	}

	private void DisableTeleportIndicator()
	{
		teleportIndicator.gameObject.SetActive(false);
	}

	/*private IEnumerator Teleport()
	{
		ResetCooldownTimer (1);
		int k = player.invincibility.Add(999);
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
		player.invincibility.RemoveTimer(k);
		player.input.isInputEnabled = true;
	}

	private void TeleportOut()
	{
		// Sound
		SoundManager.instance.RandomizeSFX(teleportOutSound);
		// Animation
		anim.Play("TeleOut");
		// Set properties
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
	}*/

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
		if (specialAbilityCharge < specialAbilityChargeCapacity)
			return;
		StartCoroutine(SpecialAbilityRoutine());
	}

	private IEnumerator SpecialAbilityRoutine()
	{
		sound.RandomizeSFX(powerUpSound);
		specialAbilityEffect.Execute();
		yield return new WaitForSeconds(specialAbilityEffect.duration);
		for (int i = 0; i < SPECIAL_NUM_METEORS; i ++)
		{
			Vector3 randomMapPosition = new Vector3						// Random map position with a buffer of 5 units
				(Random.Range(5, Map.size - 5), Random.Range(5, Map.size - 5));
			shootMeteorAbility.SetProjectileDirection(randomMapPosition);
			shootMeteorAbility.Execute();
			shootMeteorAbility.OnExecutedAction += SetMeteorProperties;
			yield return new WaitForSeconds(Random.Range(0.5f, 1f));
		}
	}

	private void SetMeteorProperties()
	{
		Projectile meteor = shootMeteorAbility.GetProjectile();
		meteor.GetComponentInChildren<AreaDamageAction>().damage = Mathf.RoundToInt(damage);
		meteor.OnDie += MeteorImpactEvent;
		specialAbilityCharge = 0;
	}

	private void MeteorImpactEvent()
	{
		CameraControl.instance.StartShake(0.3f, 0.1f, true, false);
		CameraControl.instance.StartFlashColor(Color.white, 0.2f, 0, 0, 0.2f);
	}

	public void SpecialRush()
	{
		if (specialRushCooldowntimer > 0)
			return;
		specialRushCooldowntimer = specialRushCooldown;
	}

	public void ResetSpecialAbility()
	{
		SetAnimationSet(defaultAnim);
		onDrag = ShootFireball;
		onTap = StartTeleport;
		specialActivated = false;
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
