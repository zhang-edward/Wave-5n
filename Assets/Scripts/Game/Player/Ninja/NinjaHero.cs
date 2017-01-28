using UnityEngine;
using System.Collections;

public class NinjaHero : PlayerHero {

	private const int MAX_HIT = 5;

	[Header("Class-Specific")]
	public RuntimeObjectPooler projectilePool;
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

	public override void Init(EntityPhysics body, Animator anim, Player player)
	{
		cooldownTimers = new float[2];
		base.Init (body, anim, player);
		heroName = PlayerHero.HERO_TYPES ["NINJA"];
		projectilePool = (RuntimeObjectPooler)ObjectPooler.GetObjectPooler ("PlayerProjectile");
		projectilePool.SetPooledObject(projectilePrefab);

		onSwipe = DashAttack;
		onTapRelease = ShootNinjaStar;
	}

	public void DashAttack()
	{
		if (!IsCooledDown (0, true, HandleSwipe))
			return;
		ResetCooldownTimer (0);
		StartCoroutine(DashAttackRoutine ());
	}

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || activatedSpecialAbility)
			return;
		// Sound
		SoundManager.instance.PlayImportantSound(powerUpSound);
		// Properties
		activatedSpecialAbility = true;
		projectilePool.SetPooledObject (specialProjectilePrefab);
		cooldownMultipliers[1] *= 0.3f;

		// Effects
		CameraControl.instance.StartShake (0.3f, 0.05f);
		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (Color.red, 0.3f);
		Invoke ("ResetSpecialAbility", 5.0f);
	}

	private void ResetSpecialAbility()
	{
		// Sound
		SoundManager.instance.PlayImportantSound(powerDownSound);

		specialAbilityCharge = 0;
		activatedSpecialAbility = false;
		projectilePool.SetPooledObject (projectilePrefab);
		cooldownMultipliers[1] /= 0.3f;

		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (Color.clear, 0);
	}

	public void ShootNinjaStar()
	{
		// if cooldown has not finished
		if (!IsCooledDown (1))
			return;
		ResetCooldownTimer (1);
		// Sound
		SoundManager.instance.RandomizeSFX (shootSound);
		// Animation
		anim.SetBool ("Attack", true);
		// Player properties
		Vector2 dir = player.dir.normalized;
		GameObject o = InitNinjaStar (dir);
		if (activatedSpecialAbility)
			ShootNinjaStarFanPattern ();
		// set direction
		body.Move (dir);
		body.Rb2d.velocity = Vector2.zero;

		if (OnNinjaThrewStar != null)
			OnNinjaThrewStar ();

		Invoke ("ResetAbility", 0.5f);
	}

	// Special ability
	public void ShootNinjaStarFanPattern()
	{
		Vector2 dir = player.dir.normalized;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		float fanAngle1 = angle - 15;
		float fanAngle2 = angle + 15;
		InitNinjaStar (UtilMethods.DegreeToVector2 (fanAngle1));
		InitNinjaStar (UtilMethods.DegreeToVector2 (fanAngle2));
	}

	public GameObject InitNinjaStar(Vector2 dir)
	{
		PlayerProjectile ninjaStar = projectilePool.GetPooledObject ().GetComponent<PlayerProjectile>();
		ninjaStar.Init (transform.position, dir, player);
		return ninjaStar.gameObject;
	}

	public void ResetAbility()
	{
		body.moveSpeed = player.DEFAULT_SPEED;
		anim.SetBool("Attack", false);
	}

	private IEnumerator DashAttackRoutine()
	{
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		// Sound
		SoundManager.instance.RandomizeSFX(dashOutSound);
		// Animation
		anim.SetTrigger ("DashOut");
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("DashOut"))
			yield return null;

		// Sound
		SoundManager.instance.RandomizeSFX(slashSound);
		// (Animation plays automatically)
		// Player properties
		float distance = GetDashDistanceClamped (transform.position, player.dir.normalized);
		Vector3 dest = (Vector3)player.dir.normalized * distance 
			+ player.transform.parent.position;
		DashCircleCast (transform.position, dest);
		player.transform.parent.position = dest;
		body.Move (player.dir.normalized);		
		player.isInvincible = false;

		if (OnNinjaDash != null)
			OnNinjaDash ();

		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("DashIn"))
			yield return null;
		player.input.isInputEnabled = true;
	}

	/// <summary>
	/// Do the circle cast attack with origin being the original player position and dest
	/// the final player position before and after the dash attack
	/// </summary>
	/// <param name="origin">Origin.</param>
	/// <param name="dest">Destination.</param>
	private void DashCircleCast(Vector3 origin, Vector3 dest)
	{
		bool damagedEnemy = false;
		int numEnemiesHit = 0;
		RaycastHit2D[] hits = Physics2D.CircleCastAll (origin, 0.5f, (dest - origin), dashDistance);
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
		Debug.DrawRay (start, dir * dashDistance, new Color(1, 1, 1), 5.0f);
//		Debug.Log (hit.collider.gameObject);
		if (hit.collider != null)
		{
			if (hit.collider.CompareTag ("MapBorder"))
			{
				return hit.distance - 0.5f;		// compensate for linecast starting from middle of body
			}
		}
		return dashDistance;
	}

	public void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			TempObject o = player.effectPool.GetPooledObject ().GetComponent<TempObject> ();
			SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer> ();
			anim.anim = hitEffect;
			o.Init (
				Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360f))),
				e.transform.position, 
				hitEffect.frames[0],
				true,
				0,
				0.2f);
			anim.Play ();
			

			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent (e);
		}
	}

	private void AreaAttack()
	{
		bool damagedEnemy = false;
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 1.5f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				damagedEnemy = true;
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				DamageEnemy (e);
			}
		}
		if (damagedEnemy)
			SoundManager.instance.PlaySingle (hitSounds [Random.Range (0, hitSounds.Length)]);
		
	}
}
