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
	private bool activatedSpecialAbility = false;

	public float baseRushMoveSpeed = 9f;
	public float rushMoveSpeedMultiplier = 1;
	public float baseRushDuration = 0.5f;
	[HideInInspector]
	public bool rushHitBoxOn = false;
	[Header("Animation")]
	public SimpleAnimation specialAbilityActivationAnim;
	public IndicatorEffect specialAbilityIndicator;
	[Header("Audio")]
	public AudioClip rushSound;
	public AudioClip[] hitSounds;
	public AudioClip areaAttackSound;
	public AudioClip powerUpSound;
	public AudioClip powerDownSound;

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
		SoundManager.instance.RandomizeSFX (rushSound);
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
		SoundManager.instance.RandomizeSFX (areaAttackSound);
		// Animation
		//anim.SetTrigger ("AreaAttack");
		anim.Play("AreaAttack");
		// Effects
		areaAttackEffect.SetActive(true);
		// Properties
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
			SoundManager.instance.RandomizeSFX (hitSounds[Random.Range(0, hitSounds.Length)]);
		
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
		areaAttackEffect.GetComponent<IndicatorEffect> ().AnimateOut ();

		player.sr.color = Color.white;
		if (!activatedSpecialAbility)
			player.isInvincible = false;
	}

	/*private void ResetSpecialAbility()
	{
		// Sound
		SoundManager.instance.PlayImportantSound(powerDownSound);
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
		if (specialAbilityCharge < specialAbilityChargeCapacity || activatedSpecialAbility)
			return;
		/*
		// Sound
		SoundManager.instance.PlayImportantSound(powerUpSound);
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
		specialAbilityChargeRoutine = StartCoroutine(SpecialAbilityCharge());
		if (onSpecialAbility != null)
			onSpecialAbility();
	}

	private IEnumerator SpecialAbilityCharge()
	{
		Time.timeScale = 0.2f;
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		body.Move(Vector2.zero);
		anim.Play("Special");
		CameraControl.instance.SetOverlayColor(Color.black, 0.4f, 1.0f);
		CameraControl.instance.screenOverlay.sortingLayerName = "TerrainObjects";
		while (anim.player.isPlaying)
			yield return null;
		player.isInvincible = false;
		player.input.isInputEnabled = true;
		onSwipe = SpecialRush;
		anim.Play("SpecialPersist");
		yield return new WaitForSecondsRealtime(3.0f);
		anim.Play("Default");
		ResetSpecialAbilityRoutine();
	}

	private void ResetSpecialAbilityRoutine()
	{
		CameraControl.instance.screenOverlay.sortingLayerName = "Default";
		Time.timeScale = 1f;
		CameraControl.instance.DisableOverlay(0f);
		onSwipe = RushAbility;
	}

	private void SpecialRush()
	{
		activatedSpecialAbility = true;
		ResetSpecialAbilityRoutine();
		Time.timeScale = 1f;
		StopCoroutine(specialAbilityChargeRoutine);
		// Sound
		SoundManager.instance.RandomizeSFX(rushSound);
		// Animation
		//anim.SetBool ("Attacking", true);
		anim.Play("Rush");
		// Effects
		PlayRushEffect();
		// Player properties
		maxHit = 15;
		rushHitBoxOn = true;
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		GetComponent<CircleCollider2D>().radius = 3f;
		body.moveSpeed = baseRushMoveSpeed * 1.5f;
		body.Move(player.dir.normalized);
		Debug.DrawRay(transform.position, player.dir, Color.red, 0.5f);
		// reset ability
		Invoke("ResetSpecialAbility", baseRushDuration * 2.5f);
	}

	public void ResetSpecialAbility()
	{
		// Animation
		//anim.SetBool ("Attacking", false);
		anim.Play("Default");
		// Player Properties
		hitEnemies.Clear(); // reset hit list
		rushHitBoxOn = false;
		GetComponent<CircleCollider2D>().radius = 0.5f;
		body.moveSpeed = player.DEFAULT_SPEED;
		player.input.isInputEnabled = true;
		player.isInvincible = false;
		activatedSpecialAbility = false;
		specialAbilityCharge = 0;
	}

	private void PlayRushEffect()
	{
		GameObject effectObject = activatedSpecialAbility ? specialRushEffect : rushEffect;
		float duration = activatedSpecialAbility ? baseRushDuration * 2 : baseRushDuration;

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
						SoundManager.instance.RandomizeSFX(hitSounds[Random.Range(0, hitSounds.Length)]);
						if (activatedSpecialAbility)
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
			player.effectPool.GetPooledObject().GetComponent<TempObject>().Init(
				Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360f))),
				Vector3.Lerp (transform.position, e.transform.position, 0.5f), 
				hitEffect,
				true,
				0);
			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent (e);
			return true;
		}
		return false;
	}

	public void PlaySpecialAbilityEffect()
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, specialAbilityActivationAnim.TimeLength, 0, new Color(1, 1, 1, 0.8f));
		anim.anim = specialAbilityActivationAnim;
		tempObj.Init(Quaternion.identity,
					 transform.position,
		             specialAbilityActivationAnim.frames[0]);
		anim.Play();
	}
}
