using UnityEngine;
using PlayerAbilities;
using System.Collections;
using System.Collections.Generic;

public class KnightHero : PlayerHero {

	private ObjectPooler effectPool;

	[Header("Abilities")]
	public RushAbility rushAbility;
	public RushAbility specialRushAbility;
	public AreaAttackAbility areaAttackAbility;
	[Header("Effects")]
	public GameObject rushEffect;
	public GameObject specialRushEffect;
	public GameObject areaAttackEffect;
	public bool areaAttackShieldOn = false;
	public bool specialActivated { get; private set; }
	private bool specialCharging = false;

	[Header("Animation")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation specialHitAnim;
	public SimpleAnimationPlayer specialChargeAnim;
	[Header("Audio")]
	public AudioClip rushSound;
	public AudioClip[] hitSounds;
	public AudioClip[] specialHitSounds;
	public AudioClip areaAttackSound;
	public AudioClip specialRushSound;
	public AudioClip specialRushChargeInitial;

	public Coroutine specialAbilityChargeRoutine;

	public InputAction storedOnSwipe;
	public delegate void KnightAbilityActivated();
	public event KnightAbilityActivated OnKnightRush;
	public event KnightAbilityActivated OnKnightShield;


	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		effectPool = ObjectPooler.GetObjectPooler("Effect");
		base.Init (body, player, heroData);
		InitAbilities();
		// handle input
		onSwipe = RushAbility;
		onTap = AreaAttack;
	}

	private void InitAbilities()
	{
		rushAbility.Init(player, DamageEnemy);
		specialRushAbility.Init(player, DamageEnemy);
		areaAttackAbility.Init(player, DamageEnemy);
	}

	public void RushAbility()
	{
		// check cooldown
		if (!IsCooledDown (0, true, HandleSwipe))
			return;
		ResetCooldownTimer (0);
		rushAbility.Execute();
		if (OnKnightRush != null)
			OnKnightRush();
	}

	public void AreaAttack()
	{
		// check cooldown
		if (!IsCooledDown (1))
			return;
		ResetCooldownTimer (1);
		areaAttackEffect.SetActive(true);
		// Properties
		areaAttackShieldOn = true;
		player.isInvincible = true;
		areaAttackAbility.Execute();

		// Reset Ability
		Invoke ("ResetInvincibility", 1.5f);

		if (OnKnightShield != null)
			OnKnightShield();
	}

	public void ResetInvincibility()
	{
		areaAttackShieldOn = false;
		areaAttackEffect.GetComponent<IndicatorEffect> ().AnimateOut ();

		player.sr.color = Color.white;
		if (!specialActivated)
			player.isInvincible = false;
	}

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || specialActivated)
			return;
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
		storedOnSwipe = onSwipe;
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
		onSwipe = storedOnSwipe;
	}

	private void SpecialRush()
	{
		ResetSpecialAbilityRoutine();
		// Event call
		if (onSpecialAbility != null)
			onSpecialAbility();
		Time.timeScale = 1f;
		specialRushAbility.Execute();
		player.input.isInputEnabled = false;
		damageMultiplier *= 1.5f;
		specialActivated = true;
		StopCoroutine(specialAbilityChargeRoutine);
		Invoke("ResetSpecialAbility", specialRushAbility.duration);
	}

	public void ResetSpecialAbility()
	{
		Debug.Log("ResetSpecial");
		player.input.isInputEnabled = true;
		damageMultiplier /= 1.5f;
		specialAbilityCharge = 0;
		specialActivated = false;
	}

	public void DamageEnemy(Enemy e)
	{
		//e.AddStatus(Instantiate(StatusEffectContainer.instance.GetStatus("Weakness")));
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			if (specialActivated)
				PlayEffect(e.transform.position, specialHitAnim);
			else
				PlayEffect(e.transform.position, hitEffect);
			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent (e);

			if (specialActivated)
				sound.RandomizeSFX(specialHitSounds[Random.Range(0, specialHitSounds.Length)]);
			else
				sound.RandomizeSFX(hitSounds[Random.Range(0, hitSounds.Length)]);

			if (specialActivated)
				player.StartTempSlowDown(0.3f);
		}
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
		             simpleAnim.frames[0]);
		animPlayer.Play();
	}
}
