using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightHero : PlayerHero {

	public int maxHit = 5;	// maximum number of enemies that this player can damage per rush/area attack

	private ObjectPooler effectPool;

	[Header("Class-Specific")]
	public GameObject rushEffect;
	public GameObject specialRushEffect;
	public GameObject areaAttackEffect;
	public Sprite hitEffect;
	public float areaAttackRange = 2.0f;
	private bool specialActivated = false;
	private bool specialCharging = false;
	private bool areaAttackShieldOn = false;

	public float baseRushMoveSpeed = 9f;
	public float rushMoveSpeedMultiplier = 1;
	public float baseRushDuration = 0.5f;
	[HideInInspector]
	public bool rushHitBoxOn = false;
	[Header("Animation")]
	public SimpleAnimation specialHitAnim;
	public SimpleAnimationPlayer specialChargeAnim;
	[Header("Audio")]
	public AudioClip rushSound;
	public AudioClip[] hitSounds;
	public AudioClip[] specialHitSounds;
	public AudioClip areaAttackSound;
	public AudioClip specialRushSound;
	public AudioClip specialRushChargeInitial;

	private List<Enemy> hitEnemies = new List<Enemy>();     // list of the enemies that the player has hit during the rush ability

	public Coroutine specialAbilityChargeRoutine;

	public delegate void KnightAbilityActivated();
	public event KnightAbilityActivated OnKnightRush;
	public event KnightAbilityActivated OnKnightShield;

	public void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 1f);
	}

	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		effectPool = ObjectPooler.GetObjectPooler("Effect");
		// handle input
		onSwipe = RushAbility;
		onTap = AreaAttack;
	}

	public void RushAbility()
	{
		// check cooldown
		if (!IsCooledDown (0, true, HandleSwipe))
			return;
		ResetCooldownTimer (0);

		// Sound
		sound.RandomizeSFX (rushSound);
		// Animation
		//anim.SetBool ("Attacking", true);
		anim.Play("Rush");
		// Effects
		PlayRushEffect ();
		// Player properties
		rushHitBoxOn = true;
		body.moveSpeed = baseRushMoveSpeed;
		body.Move(player.dir.normalized);
		Debug.DrawRay (transform.position, player.dir, Color.red, 0.5f);
		// reset ability
		Invoke ("ResetRushAbility", baseRushDuration);

		if (OnKnightRush != null)
			OnKnightRush();
	}

	public void ResetRushAbility()
	{
		// Animation
		//anim.SetBool ("Attacking", false);
		anim.Play("Default");
		// Player Properties
		hitEnemies.Clear ();	// reset hit list
		rushHitBoxOn = false;
		body.moveSpeed = player.DEFAULT_SPEED;
		player.input.isInputEnabled = true;
	}

	public void AreaAttack()
	{
		// check cooldown
		if (!IsCooledDown (1))
			return;
		ResetCooldownTimer (1);
		// Sound
		sound.RandomizeSFX (areaAttackSound);
		// Animation
		//anim.SetTrigger ("AreaAttack");
		anim.Play("AreaAttack");
		// Effects
		areaAttackEffect.SetActive(true);
		// Properties
		areaAttackShieldOn = true;
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		player.sr.color = new Color (0.8f, 0.8f, 0.8f);
		body.Move (Vector2.zero);

		bool enemyHit = false;
		int numEnemiesHit = 0;
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, areaAttackRange);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				numEnemiesHit++;
				if (numEnemiesHit < maxHit)
				{
					Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
					hitEnemies.Clear();
					DamageEnemy (e);
					enemyHit = true;
				}
			}
		}
		if (enemyHit)
			sound.RandomizeSFX (hitSounds[Random.Range(0, hitSounds.Length)]);
		
		// Reset Ability
		Invoke ("ResetAreaAttackAbility", 0.5f);
		Invoke ("ResetInvincibility", 1.5f);

		if (OnKnightShield != null)
			OnKnightShield();
	}

	public void ResetAreaAttackAbility()
	{
		hitEnemies.Clear ();
		//anim.SetBool ("AreaAttack", false);
		//animationSet.Play("Default", true);
		player.input.isInputEnabled = true;
	}

	public void ResetInvincibility()
	{
		areaAttackShieldOn = false;
		areaAttackEffect.GetComponent<IndicatorEffect> ().AnimateOut ();

		player.sr.color = Color.white;
		if (!specialActivated)
			player.isInvincible = false;
	}

	/*private void ResetSpecialAbility()
	{
		// Sound
		sound.PlayImportantSound(powerDownSound);
		// Effects
		specialAbilityIndicator.AnimateOut();
		// Reset Stats
		onSwipe = RushAbility;
		onTap = AreaAttack;
		cooldownMultipliers [0] /= 0.8f;
		player.isInvincible = false;
		activatedSpecialAbility = false;
		specialAbilityCharge = 0;
		baseRushMoveSpeed = 9f * rushMoveSpeedMultiplier;
		baseRushDuration = 0.5f;

		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (Color.clear, 0);
	}*/

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || specialActivated)
			return;
		/*
		// Sound
		sound.PlayImportantSound(powerUpSound);
		// Effect
		PlaySpecialAbilityEffect();
		specialAbilityIndicator.gameObject.SetActive(true);
		onTap = RushAbility;
		// Properties
		activatedSpecialAbility = true;
		player.isInvincible = true;
		cooldownMultipliers [0] *= 0.8f;
		baseRushMoveSpeed = 13 * rushMoveSpeedMultiplier;
		baseRushDuration = 0.4f;
		// CameraControl.instance.StartShake (0.3f, 0.05f);
		CameraControl.instance.StartFlashColor (Color.white);
		// CameraControl.instance.SetOverlayColor (Color.red, 0.3f);
		Invoke ("ResetSpecialAbility", 10f);
		*/
		if (specialCharging)
		{
			if (specialAbilityChargeRoutine != null)
				StopCoroutine(specialAbilityChargeRoutine);
			ResetSpecialAbilityRoutine();
		}
		else
		{
			specialAbilityChargeRoutine = StartCoroutine(SpecialAbilityCharge());
		}
	}

	private IEnumerator SpecialAbilityCharge()
	{
		CancelInvoke();
		if (areaAttackShieldOn)
			ResetInvincibility();
		Time.timeScale = 0.2f;
		// Player Properties
		player.isInvincible = true;
		specialCharging = true;
		player.input.isInputEnabled = false;
		// Animation
		anim.Play("Special");
		specialChargeAnim.Play();
		// Sound
		sound.RandomizeSFX(specialRushChargeInitial);
		// Camera Control
		CameraControl.instance.SetOverlayColor(Color.black, 0.4f, 1.0f);
		CameraControl.instance.screenOverlay.sortingLayerName = "TerrainObjects";
		while (anim.player.isPlaying)
			yield return null;
		// Player Properties
		player.isInvincible = false;
		player.input.isInputEnabled = true;
		// Animation
		anim.Play("SpecialPersist");
		// Set onSwipe
		onSwipe = SpecialRush;
		yield return new WaitForSecondsRealtime(3.0f);
		// Animation
		anim.Play("Default");
		ResetSpecialAbilityRoutine();
	}

	private void ResetSpecialAbilityRoutine()
	{
		Time.timeScale = 1f;
		// Player Properties
		specialCharging = false;
		player.isInvincible = false;
		player.input.isInputEnabled = true;
		// Camera
		CameraControl.instance.screenOverlay.sortingLayerName = "Default";
		CameraControl.instance.DisableOverlay(1f);
		// Reset onSwipe
		onSwipe = RushAbility;
	}

	private void SpecialRush()
	{
		ResetSpecialAbilityRoutine();
		// Event call
		if (onSpecialAbility != null)
			onSpecialAbility();
		Time.timeScale = 1f;
		StopCoroutine(specialAbilityChargeRoutine);
		// Sound
		sound.RandomizeSFX(specialRushSound);
		// Player properties
		maxHit = 15;
		rushHitBoxOn = true;
		specialActivated = true;
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		GetComponent<CircleCollider2D>().radius = 3f;       // Make hitbox huge
		damageMultiplier *= 1.5f;
		body.moveSpeed = baseRushMoveSpeed * 1.5f;
		// Body Movement
		body.Move(player.dir.normalized);
		// Animation
		anim.Play("Rush");
		PlayRushEffect();
		// Debug
		Debug.DrawRay(transform.position, player.dir, Color.red, 0.5f);
		// Reset ability
		Invoke("ResetSpecialAbility", baseRushDuration * 2.5f);
	}

	public void ResetSpecialAbility()
	{
		// Animation
		anim.Play("Default");
		// Player Properties
		maxHit = 5;
		rushHitBoxOn = false;
		specialActivated = false;
		player.isInvincible = false;
		player.input.isInputEnabled = true;
		GetComponent<CircleCollider2D>().radius = 0.5f;
		damageMultiplier /= 1.5f;
		body.moveSpeed = player.DEFAULT_SPEED;
		hitEnemies.Clear(); // Reset hit list
		specialAbilityCharge = 0;
	}

	private void PlayRushEffect()
	{
		GameObject effectObject = specialActivated ? specialRushEffect : rushEffect;
		float duration = specialActivated ? baseRushDuration * 2 : baseRushDuration;

		TempObject effect = effectObject.GetComponent<TempObject>();
		SimpleAnimationPlayer animPlayer = effectObject.GetComponent<SimpleAnimationPlayer> ();

		Vector2 dir = player.dir;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		TempObjectInfo info = new TempObjectInfo ();
		info.targetColor = new Color (1, 1, 1, 0.5f);
		info.lifeTime = duration;
		info.fadeOutTime = 0.1f;
		effect.Init (Quaternion.Euler (new Vector3 (0, 0, angle)), transform.position, animPlayer.anim.frames [0], info);
		animPlayer.Play ();
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (rushHitBoxOn)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				if (!hitEnemies.Contains (e) && hitEnemies.Count < maxHit)
				{
					if (DamageEnemy(e))
					{
						if (specialActivated)
							sound.RandomizeSFX(specialHitSounds[Random.Range(0, specialHitSounds.Length)]);
						else
							sound.RandomizeSFX(hitSounds[Random.Range(0, hitSounds.Length)]);

						if (specialActivated)
							player.StartTempSlowDown(0.3f);
					}
				}
			}
		}
	}

	public bool DamageEnemy(Enemy e)
	{
		//e.AddStatus(Instantiate(StatusEffectContainer.instance.GetStatus("Weakness")));
		if (!e.invincible && e.health > 0 && !hitEnemies.Contains(e))
		{
			e.Damage (damage);
			hitEnemies.Add (e);
			if (specialActivated)
				PlayEffect(e.transform.position, specialHitAnim);
			else
				HitEffect(e.transform.position);
			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent (e);
			return true;
		}
		return false;
	}

	private void HitEffect(Vector3 position)
	{
		player.effectPool.GetPooledObject().GetComponent<TempObject>().Init(
				Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360f))),
				Vector3.Lerp(transform.position, position, 0.5f),
				hitEffect,
				true,
				0);
	}

	private void PlayEffect(Vector3 position, SimpleAnimation simpleAnim)
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer animPlayer = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, simpleAnim.TimeLength, 0);
		animPlayer.anim = simpleAnim;
		tempObj.Init(Quaternion.Euler(0, 0, Random.Range(0, 360)),
					 position,
				 specialHitAnim.frames[0]);
		animPlayer.Play();
	}
}
