using UnityEngine;
using Projectiles;
using PlayerActions;
using System.Collections.Generic;

public class NinjaHero : PlayerHero {

	private const int SHADOW_BACKUP_ACTIONFRAME = 3;
	private const int MAX_HIT = 5;

	[Header("Abilities")]
	public PA_ShootProjectile ninjaStarAbility;
	public PA_Teleport 		  dashAbility;
	public PA_AreaEffect 	  shadowBackupDetector;
	public PA_EffectCallback  shadowBackup;

	private Vector3 lastDashOutPos;
	private Vector3 lastDashInPos;
	private Queue<Enemy> enemiesToAttack = new Queue<Enemy>();

	private RuntimeObjectPooler projectilePool;
	public GameObject projectilePrefab;
	public GameObject specialProjectilePrefab;
	// public int smokeBombRange = 2;
	public float dashDistance = 4;

	public SimpleAnimation hitEffect;
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
		shadowBackupDetector.Init(player, SpawnShadowBackup);
		shadowBackup.Init		 (player, SHADOW_BACKUP_ACTIONFRAME);

		shadowBackup.onFrameReached += HandleShadowBackupFrameReached;
		dashAbility.OnTeleportIn += OnDashIn;
	}

	protected override void ParryEffect()
	{
		cooldownTimers[0] = 0f;
		cooldownTimers[1] = 0f;
		body.Move(UtilMethods.DegreeToVector2(Random.Range(0, 360f)));
	}

	public void DashAttack()
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
		//StartCoroutine(DashAttackRoutine ());
	}

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || activatedSpecialAbility)
			return;
		dashAbility.OnTeleportIn += DetectShadowBackup;
		// Sound
		/*SoundManager.instance.PlayImportantSound(powerUpSound);
		// Properties
		activatedSpecialAbility = true;
		projectilePool.SetPooledObject (specialProjectilePrefab);
		cooldownMultipliers[1] *= 0.3f;

		// Effects
		CameraControl.instance.StartShake (0.3f, 0.05f, true, true);
		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (Color.red, 0.3f);*/
		Invoke ("ResetSpecialAbility", 5.0f);
	}

	private void DetectShadowBackup()
	{
		shadowBackupDetector.SetPosition(transform.position);
		shadowBackupDetector.Execute();
	}

	private void SpawnShadowBackup(Enemy e)
	{
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
		// Sound
		/*SoundManager.instance.PlayImportantSound(powerDownSound);

		specialAbilityCharge = 0;
		activatedSpecialAbility = false;
		projectilePool.SetPooledObject (projectilePrefab);
		cooldownMultipliers[1] /= 0.3f;*/

		//CameraControl.instance.StartFlashColor (Color.white);
		//CameraControl.instance.SetOverlayColor (Color.clear, 0);
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
		ninjaStar.GetComponentInChildren<DamageAction>().damage = damage;

		// set direction
		body.Move(dir);
		body.rb2d.velocity = Vector2.zero;

		if (OnNinjaThrewStar != null)
			OnNinjaThrewStar();
	}

	// Special ability
	/*public void ShootNinjaStarFanPattern()
	{
		Vector2 dir = player.dir.normalized;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		float fanAngle1 = angle - 15;
		float fanAngle2 = angle + 15;
		InitNinjaStar (UtilMethods.DegreeToVector2 (fanAngle1));
		InitNinjaStar (UtilMethods.DegreeToVector2 (fanAngle2));
	}*/

	public GameObject InitNinjaStar(Vector2 dir)
	{
		Projectile ninjaStar = projectilePool.GetPooledObject ().GetComponent<Projectile>();
		ninjaStar.Init (transform.position, dir);
		return ninjaStar.gameObject;
	}

	public void ResetAbility()
	{
		body.moveSpeed = player.DEFAULT_SPEED;
	}

	/*private IEnumerator DashAttackRoutine()
	{
		int k = player.invincibility.Add(999);
		player.input.isInputEnabled = false;
		// Sound
		SoundManager.instance.RandomizeSFX(dashOutSound);
		// Animation
		anim.Play ("DashOut");
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.player.isPlaying)
			yield return null;

		// Animation
		anim.Play("DashIn");
		// Sound
		SoundManager.instance.RandomizeSFX(slashSound);
		// (Animation plays automatically)
		// Player properties
		player.invincibility.RemoveTimer(k);
		float distance = GetDashDistanceClamped (transform.position, player.dir.normalized);
		Vector3 dest = (Vector3)player.dir.normalized * distance 
			+ player.transform.parent.position;
		//DashCircleCast (transform.position, dest);
		player.transform.parent.position = dest;
		body.Move (player.dir.normalized);		



		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.player.isPlaying)
			yield return null;
		player.input.isInputEnabled = true;
	}*/

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
			SoundManager.instance.PlaySingle (hitSounds [Random.Range (0, hitSounds.Length)]);
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
				return hit.distance;		// compensate for linecast starting from middle of body
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
}
