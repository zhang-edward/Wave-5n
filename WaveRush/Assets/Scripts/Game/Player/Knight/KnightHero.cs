using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightHero : PlayerHero {

	public const int MAX_HIT = 5;	// maximum number of enemies that this player can damage per rush/area attack

	[Header("Class-Specific")]
	public GameObject rushEffect;
	public GameObject areaAttackEffect;
	public Sprite hitEffect;
	public float areaAttackRange = 2.0f;
	private bool activatedSpecialAbility = false;

	public float baseRushMoveSpeed = 9f;
	public float rushMoveSpeedMultiplier = 1;
	public float baseRushDuration = 0.5f;
	[HideInInspector]
	public bool rushHitBoxOn = false;

	[Header("Audio")]
	public AudioClip rushSound;
	public AudioClip[] hitSounds;
	public AudioClip areaAttackSound;
	public AudioClip powerUpSound;
	public AudioClip powerDownSound;

	private List<Enemy> hitEnemies = new List<Enemy>();		// list of the enemies that the player has hit during the rush ability

	public delegate void KnightAbilityActivated();
	public event KnightAbilityActivated OnKnightRush;

	public void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 1f);
	}

	public override void Init (EntityPhysics body, Animator anim, Player player)
	{
		cooldownTimers = new float[2];
		base.Init (body, anim, player);
		// handle input
		onSwipe = RushAbility;
		onTapRelease = AreaAttack;
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
		anim.SetBool ("Attacking", true);
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
		anim.SetBool ("Attacking", false);
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
		anim.SetTrigger ("AreaAttack");
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
				if (numEnemiesHit < MAX_HIT)
				{
					Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
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
	}

	private void ResetAreaAttackAbility()
	{
		hitEnemies.Clear ();
		anim.SetBool ("AreaAttack", false);
		player.input.isInputEnabled = true;
	}

	private void ResetInvincibility()
	{
		areaAttackEffect.GetComponent<IndicatorEffect> ().AnimateOut ();

		player.sr.color = Color.white;
		player.isInvincible = false;
	}

	private void ResetSpecialAbility()
	{
		// Sound
		SoundManager.instance.PlayImportantSound(powerDownSound);

		// Reset Stats
		cooldownMultipliers [0] /= 0.8f;
		player.isInvincible = false;
		activatedSpecialAbility = false;
		specialAbilityCharge = 0;
		baseRushMoveSpeed = 9f * rushMoveSpeedMultiplier;
		baseRushDuration = 0.5f;

		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (Color.clear, 0);
	}

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || activatedSpecialAbility)
			return;
		// Sound
		SoundManager.instance.PlayImportantSound(powerUpSound);
		// Properties
		activatedSpecialAbility = true;
		player.isInvincible = true;
		cooldownMultipliers [0] *= 0.8f;
		baseRushMoveSpeed = 13 * rushMoveSpeedMultiplier;
		baseRushDuration = 0.4f;
		CameraControl.instance.StartShake (0.3f, 0.05f);
		CameraControl.instance.StartFlashColor (Color.white);
		CameraControl.instance.SetOverlayColor (Color.red, 0.3f);
		Invoke ("ResetSpecialAbility", 10f);
	}

	private void PlayRushEffect()
	{
		TempObject effect = rushEffect.GetComponent<TempObject> ();
		SimpleAnimationPlayer animPlayer = rushEffect.GetComponent<SimpleAnimationPlayer> ();

		Vector2 dir = player.dir;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		TempObjectInfo info = new TempObjectInfo ();
		info.targetColor = new Color (1, 1, 1, 0.5f);
		info.lifeTime = baseRushDuration;
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
				if (!hitEnemies.Contains (e) && hitEnemies.Count < MAX_HIT)
				{
					SoundManager.instance.RandomizeSFX (hitSounds[Random.Range(0, hitSounds.Length)]);
					DamageEnemy (e);
				}
			}
		}
	}

	private void DamageEnemy(Enemy e)
	{
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
		}
	}
}
