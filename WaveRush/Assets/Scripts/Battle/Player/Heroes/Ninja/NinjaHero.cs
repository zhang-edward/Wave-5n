using UnityEngine;
using Projectiles;
using PlayerActions;
using System.Collections;
using System.Collections.Generic;

public class NinjaHero : PlayerHero {
	// private const float PARRY_SMOKEBOMB_RADIUS = 2;
	// private const int MAX_HIT = 3;
	private const int SHADOW_BACKUP_ACTIONFRAME = 3;
	private const float DASH_DISTANCE = 3.0f;
	private const float DASH_TIMEOUT = 1.5f;

	[Header("Abilities")]
	// Dash
	public PA_Teleport dashTeleportAbility;
	public PA_CircleCast dashAttackAbility;
	public PA_Rush rushAbility;
	// Ninja Star
	public PA_ShootProjectile ninjaStarAbility;
	// Shadow Backup
	public PA_AreaEffect shadowBackupDetector;
	public PA_EffectCallback shadowBackup;

	[Header("Prefabs")]
	public GameObject projectilePrefab;
	public GameObject specialProjectilePrefab;
	public GameObject smokeBombPrefab;
	public ContinuousAnimatedLine lightningTrail;

	[Header("Effects")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation smokeBombEffect;
	public SimpleAnimation ninjaStarAnim;
	public SimpleAnimation[] shadowBackupEffects;

	[HideInInspector]
	public bool activatedSpecialAbility = false;

	[Header("Audio")]
	public AudioClip[] hitSounds;
	public AudioClip shootSound;
	public AudioClip dashOutSound;
	public AudioClip slashSound;
	public AudioClip powerUpSound;
	public AudioClip powerDownSound;

	// Private members
	private Projectile lastShotNinjaStar;
	private Queue<Enemy> enemiesToAttack = new Queue<Enemy>();
	private RuntimeObjectPooler projectilePool;
	private int currentDashIndex;
	private float dashCooldown;

	// Events
	public delegate void NinjaCreatedObject(GameObject o);
	public delegate void NinjaActivatedAbility();
	public event NinjaActivatedAbility OnNinjaThrewStar;
	public event NinjaActivatedAbility OnNinjaDash;

	public override void Init(EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init(body, player, heroData);
		projectilePool = (RuntimeObjectPooler)projectilePrefab.GetComponent<Projectile>().GetObjectPooler();

		onTap = DoDashAttackSequential;
		onDragRelease = NinjaStar;
		InitAbilities();
	}

	private void InitAbilities()
	{
		ninjaStarAbility.Init(player, projectilePool);
		dashAttackAbility.Init(player, DamageEnemy);
		dashTeleportAbility.Init(player);
		rushAbility.Init(player, HandleRushHitEnemy);
		shadowBackupDetector.Init(player, StartSpawnShadowBackupRoutine);
		shadowBackup.Init(player, SHADOW_BACKUP_ACTIONFRAME);

		shadowBackup.onFrameReached += HandleShadowBackupFrameReached;
		dashTeleportAbility.OnTeleportIn += OnDashIn;
	}

	protected override void ParryEffect(IDamageable src)
	{
	}

	#region FlashStrike
	private void DoDashAttackSequential(Vector3 dir)
	{
		if (!CheckIfCooledDownNotify(0, HandleTap, dir))
			return;
		if (currentDashIndex == 0 || currentDashIndex == 2) {
			DashAttack(dir);
		}
		else
			RushAttack(dir);
		currentDashIndex = (currentDashIndex + 1) % 3;
		if (currentDashIndex == 0)
		{
			Invoke(nameof(ResetAnimation), 1.0f);
			dashCooldown = 0;
			ResetCooldownTimer(0);
		}
		else
			dashCooldown = DASH_TIMEOUT;
	}

	private void DashAttack(Vector3 dir)
	{
		// Get values
		float distance = GetDashDistanceBounded(transform.position, dir.normalized);
		Vector3 dest = (Vector3)dir.normalized * distance + player.transform.parent.position;
		// Assign origin and dest
		dashAttackAbility.SetCast(transform.position, dir, distance);
		lightningTrail.Init(transform.position, dest, true);
		// Execute Ability
		dashTeleportAbility.SetDestination(dest);
		dashTeleportAbility.Execute();
	}

	private void OnDashIn()
	{
		body.rb2d.drag = 9;
		body.Move(dashAttackAbility.dir.normalized);
		lightningTrail.CreateLine();
		if (OnNinjaDash != null)
			OnNinjaDash();
		dashAttackAbility.Execute();
	}

	/// <summary>
	/// Gets the distance for the dash, bounded to be within the map
	/// </summary>
	/// <param name="start"></param>
	/// <param name="dir"></param>
	private float GetDashDistanceBounded(Vector3 start, Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.Raycast(start, dir, DASH_DISTANCE, 1 << LayerMask.NameToLayer("MapCollider"));
		if (hit.collider != null)
		{
			if (hit.collider.CompareTag("MapBorder"))
			{
				//Debug.Log (hit);
				Debug.DrawRay(start, dir * hit.distance, new Color(1, 1, 1), 5.0f);
				return hit.distance - 0.5f;     // compensate for linecast starting from middle of body
			}
		}
		Debug.DrawRay(start, dir * DASH_DISTANCE, new Color(1, 1, 1), 5.0f);
		return DASH_DISTANCE;
	}

	private void RushAttack(Vector3 dir)
	{
		rushAbility.SetDirection(dir);
		rushAbility.Execute();
	}

	protected override void Update()
	{
		base.Update();
		if (dashCooldown > 0)
			dashCooldown -= Time.deltaTime;
		if (dashCooldown < 0)
		{
			dashCooldown = 0;
			currentDashIndex = 0;
			ResetCooldownTimer(0);
			anim.player.ResetToDefault();
		}
	}

	private void ResetAnimation()
	{
		anim.player.ResetToDefault();
	}
	#endregion

	public override void SpecialAbility()
	{
		if (specialAbilityCharge < SPECIAL_ABILITY_CHARGE_CAPACITY || activatedSpecialAbility)
			return;
		dashTeleportAbility.OnTeleportIn += DetectShadowBackup;
		Invoke("ResetSpecialAbility", 5.0f);
	}

	private void DetectShadowBackup()
	{
		shadowBackupDetector.SetPosition(transform.position);
		shadowBackupDetector.Execute();
	}

	private void StartSpawnShadowBackupRoutine(Enemy e)
	{
		StartCoroutine(SpawnShadowBackup(e));
	}

	private IEnumerator SpawnShadowBackup(Enemy e)
	{
		yield return new WaitForSeconds(Random.Range(0, 1f));
		enemiesToAttack.Enqueue(e);
		float f = UtilMethods.RandSign();   // Which side the shadow attacks from
											// Set action properties and execute
		Vector3 position = e.transform.position + new Vector3(f * 0.5f, 0); // offset sprite
		shadowBackup.effect = shadowBackupEffects[Random.Range(0, shadowBackupEffects.Length)];
		shadowBackup.SetPosition(position);
		shadowBackup.Execute();
		// Set effect properties
		SpriteRenderer sr = shadowBackup.GetLastPlayedEffect().GetComponent<SpriteRenderer>();
		sr.flipX = f > 0;
	}

	private void HandleShadowBackupFrameReached(int frame)
	{
		if (frame != SHADOW_BACKUP_ACTIONFRAME)
			return;
		Enemy e = enemiesToAttack.Dequeue();
		DamageEnemy(e);
	}

	private void ResetSpecialAbility()
	{
		dashTeleportAbility.OnTeleportIn -= shadowBackupDetector.Execute;
	}

	private void NinjaStar(Vector3 dir)
	{
		// if cooldown has not finished
		if (!CheckIfCooledDownNotify(1))
			return;
		ResetCooldownTimer(1);
		ShootStar(dir.normalized);
	}

	private void ShootStar(Vector2 dir)
	{
		ninjaStarAbility.SetProjectileOrigin(player.transform.position);
		ninjaStarAbility.SetProjectileDirection(dir);
		ninjaStarAbility.Execute();

		Projectile ninjaStar = ninjaStarAbility.GetProjectile();
		ninjaStar.GetComponentInChildren<DamageAction>().damage = Mathf.CeilToInt(damage);
		lastShotNinjaStar = ninjaStar;

		// set direction
		body.rb2d.drag = 10f;
		body.moveSpeed = 5;
		body.Move(dir.normalized * 3f);
		anim.Play("Shoot");

		if (OnNinjaThrewStar != null)
			OnNinjaThrewStar();
	}

	public GameObject InitNinjaStar(Vector2 dir)
	{
		Projectile ninjaStar = projectilePool.GetPooledObject().GetComponent<Projectile>();
		ninjaStar.Init(transform.position, dir, player);
		return ninjaStar.gameObject;
	}

	private void HandleRushHitEnemy(Enemy e)
	{
		DamageEnemy(e);
		// LightningEnemy(e);
	}

	public void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage(damage, player, true);
			EffectPooler.PlayEffect(hitEffect, e.transform.position, true, 0.2f);

			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent(e);
		}
	}

	private void StunEnemySmokeBomb(EnemyDetectionZone zone, Enemy e)
	{
		GameObject o = Instantiate(StatusEffectContainer.instance.GetStatus("Stun"));
		StunStatus stun = o.GetComponent<StunStatus>();
		stun.duration = 5.0f;
		e.AddStatus(o);
		e.body.AddImpulse(zone.transform.position - e.transform.position);
	}

	private void PoisonEnemy(IDamageable damageable, int amt)
	{
		Enemy e = (Enemy)damageable;
		GameObject o = Instantiate(StatusEffectContainer.instance.GetStatus("Poison"));
		PoisonStatus poison = o.GetComponent<PoisonStatus>();
		poison.duration = 5f;
		poison.damage = Mathf.CeilToInt(damage * 0.1f);
		e.AddStatus(o);
		lastShotNinjaStar.OnDamagedTarget -= PoisonEnemy;

	}

	private void LightningEnemy(IDamageable damageable)
	{
		Enemy e = (Enemy)damageable;
		GameObject o = Instantiate(StatusEffectContainer.instance.GetStatus("Lightning"));
		LightningStatus lightning = o.GetComponent<LightningStatus>();
		lightning.duration = 5f;
		lightning.lightningBounceRange = 5.0f;
		lightning.numBounces = 2;
		lightning.damage = (int)(noiselessDamage);
		e.AddStatus(o);
	}

	protected override Quests.Quest UnlockQuest(HeroTier tier)
	{
		switch (tier)
		{
			case HeroTier.tier1:
				return null;
			case HeroTier.tier2:
				return null;
			case HeroTier.tier3:
				return null;    // TODO: Do this
			default:
				return null;
		}
	}
}
