using UnityEngine;
using Projectiles;
using PlayerActions;
using System.Collections;
using System.Collections.Generic;

public class NinjaHero : PlayerHero {

	private const float PARRY_SMOKEBOMB_RADIUS = 2;
	private const int SHADOW_BACKUP_ACTIONFRAME = 3;
	private const int MAX_HIT = 5;

	[Header("Abilities")]
	public PA_ShootProjectile ninjaStarAbility;
	public PA_Teleport 		  dashAbility;
	public PA_AreaEffect 	  shadowBackupDetector;
	public PA_EffectCallback  shadowBackup;

	private Projectile lastShotNinjaStar;
	private Vector3 lastDashOutPos;
	private Vector3 lastDashInPos;
	private Queue<Enemy> enemiesToAttack = new Queue<Enemy>();

	private RuntimeObjectPooler projectilePool;
	[Header("Prefabs")]
	public GameObject projectilePrefab;
	public GameObject specialProjectilePrefab;
	public GameObject smokeBombPrefab;
	// public int smokeBombRange = 2;
	public float dashDistance = 4;

	[Header("Effects")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation smokeBombEffect;
	public SimpleAnimation ninjaStarAnim;

	[HideInInspector]
	public bool activatedSpecialAbility = false;

	[Header("Audio")]
	public AudioClip[] hitSounds;
	public AudioClip shootSound;
	public AudioClip dashOutSound;
	public AudioClip slashSound;
	public AudioClip powerUpSound;
	public AudioClip powerDownSound;

	//public int damage = 1;
	public delegate void NinjaCreatedObject(GameObject o);
	public delegate void NinjaActivatedAbility();
	public event NinjaActivatedAbility OnNinjaThrewStar;
	public event NinjaActivatedAbility OnNinjaDash;

	public override void Init(EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		projectilePool = (RuntimeObjectPooler)projectilePrefab.GetComponent<Projectile>().GetObjectPooler();

		onDragRelease = DashAttack;
		onTap = NinjaStar;
		InitAbilities();
	}

	private void InitAbilities()
	{
		ninjaStarAbility.Init	 (player, projectilePool);
		dashAbility.Init		 (player);
		shadowBackupDetector.Init(player, StartSpawnShadowBackupRoutine);
		shadowBackup.Init		 (player, SHADOW_BACKUP_ACTIONFRAME);

		shadowBackup.onFrameReached += HandleShadowBackupFrameReached;
		dashAbility.OnTeleportIn += OnDashIn;
	}

	protected override void ParryEffect()
	{
		body.Move(UtilMethods.DegreeToVector2(Random.Range(0, 360f)) * 0.5f);
		SmokeBomb(PARRY_SMOKEBOMB_RADIUS);
	}

	public void SmokeBomb(float radius)
	{
		GameObject smokeBomb = Instantiate(smokeBombPrefab, transform.position, Quaternion.identity) as GameObject;
		EffectPooler.PlayEffect(smokeBombEffect, transform.position, false, 0.2f);
		smokeBomb.transform.SetParent(ObjectPooler.GetObjectPooler("Effect").transform);
		smokeBomb.GetComponent<CircleCollider2D>().radius = radius;
		smokeBomb.GetComponent<EnemyDetectionZone>().SetOnDetectEnemyCallback(StunEnemySmokeBomb);
		// Particle Properties
		ParticleSystem particleSystem = smokeBomb.GetComponent<ParticleSystem>();
		ParticleSystem.ShapeModule shape = particleSystem.shape;
		ParticleSystem.MinMaxCurve rateOverTime = particleSystem.emission.rateOverTime;
		shape.radius = radius;
		rateOverTime.constant = radius * (2 / 5f);

	}

	private void DashAttack()
	{
		if (!CheckIfCooledDownNotify(0, true, HandleDragRelease))
			return;
		ResetCooldownTimer (0);

		// Get values
		float distance = GetDashDistanceClamped(transform.position, player.dir.normalized);
		Vector3 dest = (Vector3)player.dir.normalized * distance + player.transform.parent.position;
		// Assign origin and dest
		lastDashOutPos = transform.position;
		lastDashInPos = dest;
		// Execute Ability
		dashAbility.SetDestination(dest);
		dashAbility.Execute();
	}

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || activatedSpecialAbility)
			return;
		dashAbility.OnTeleportIn += DetectShadowBackup;
		Invoke ("ResetSpecialAbility", 5.0f);
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
		float f = UtilMethods.RandSign();	// Which side the shadow attacks from
		// Set action properties and execute
		Vector3 position = e.transform.position + new Vector3(f * 0.5f, 0); // offset sprite
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
		dashAbility.OnTeleportIn -= shadowBackupDetector.Execute;
	}

	private void NinjaStar()
	{
		// if cooldown has not finished
		if (!CheckIfCooledDownNotify(1))
			return;
		ResetCooldownTimer(1);
		ShootStar(player.dir.normalized);
	}

	private void ShootStar(Vector2 dir)
	{
		ninjaStarAbility.SetProjectileOrigin(player.transform.position);
		ninjaStarAbility.SetProjectileDirection(dir);
		ninjaStarAbility.Execute();

		Projectile ninjaStar = ninjaStarAbility.GetProjectile();
		ninjaStar.GetComponentInChildren<DamageAction>().damage = Mathf.CeilToInt(damage * 1.5f);
		ninjaStar.OnDamagedTarget += PoisonEnemy;
		lastShotNinjaStar = ninjaStar;

		// set direction
		body.Move(dir);
		body.rb2d.velocity = Vector2.zero;

		if (OnNinjaThrewStar != null)
			OnNinjaThrewStar();
	}

	public GameObject InitNinjaStar(Vector2 dir)
	{
		Projectile ninjaStar = projectilePool.GetPooledObject ().GetComponent<Projectile>();
		ninjaStar.Init (transform.position, dir, player.gameObject);
		return ninjaStar.gameObject;
	}

	/// <summary>
	/// Do the circle cast attack with origin being the original player position and dest
	/// the final player position before and after the dash attack
	/// </summary>
	/// <param name="origin">Origin.</param>
	/// <param name="dest">Destination.</param>
	private void OnDashIn()
	{
		body.Move (player.dir.normalized);		

		if (OnNinjaDash != null)
			OnNinjaDash();

		// Do Dash circle cast
		bool damagedEnemy = false;
		int numEnemiesHit = 0;
		RaycastHit2D[] hits = Physics2D.CircleCastAll (lastDashOutPos, 0.5f, (lastDashInPos - lastDashOutPos), dashDistance);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider.CompareTag("Enemy"))
			{
				numEnemiesHit++;
				if (numEnemiesHit < MAX_HIT)
				{
					damagedEnemy = true;
					Enemy e = hit.collider.GetComponentInChildren<Enemy> ();
					DamageEnemy (e);
				}
			}
		}
		if (damagedEnemy)
		{
			SoundManager.instance.PlaySingle(hitSounds[Random.Range(0, hitSounds.Length)]);
			cooldownTimers[1] -= 0.5f;
		}
	}

	private float GetDashDistanceClamped(Vector3 start, Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.Raycast (start, dir, dashDistance, 1 << LayerMask.NameToLayer("MapCollider"));
		if (hit.collider != null)
		{
			if (hit.collider.CompareTag ("MapBorder"))
			{
				//Debug.Log (hit);
				Debug.DrawRay(start, dir * hit.distance, new Color(1, 1, 1), 5.0f);
				return hit.distance - 0.5f;		// compensate for linecast starting from middle of body
			}
		}
		Debug.DrawRay(start, dir * dashDistance, new Color(1, 1, 1), 5.0f);
		return dashDistance;
	}

	public void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			EffectPooler.PlayEffect(hitEffect, e.transform.position, true, 0.2f);

			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent (e);
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
}
