using UnityEngine;
using PlayerActions;
using System.Collections;
using System.Collections.Generic;

public class KnightHero : PlayerHero {

	[System.Serializable]
	public class PA_SpecialRush : PA_Sequencer
	{
		public PA_Animate chargeAnim;
		public PA_EffectAttached chargeEffect;
		public PA_SpecialAbilityEffect specialEffect;
		private PA_Joint chargeAction = new PA_Joint();
		[Space]
		public PA_InputListener inputListener;
		public PA_Rush specialRush;

		public void Init(Player player, PA_Rush.HitEnemy onHitEnemyCallback)
		{
			base.Init(player);
			actions = new PlayerAction[2];

			chargeAnim.Init(player);
			chargeEffect.Init(player);
			specialEffect.Init(player, specialRush);
			chargeAction.Init(player, 
			                  chargeAnim, 
			                  chargeEffect,
			                  specialEffect);

			inputListener.Init(player, specialRush);
			specialRush.Init(player, onHitEnemyCallback);

			actions[0] = chargeAction;
			actions[1] = inputListener;
		}
	}

	[Header("Abilities")]
	public PA_Rush rushAbility;
	public PA_AreaEffect areaAttackAbility;
	public PA_SpecialRush specialRushAbility;
	[Header("Effects")]
	public GameObject rushEffect;
	public GameObject specialRushEffect;
	public GameObject shieldIndicator;
	public bool specialActivated { get; private set; }
	private float shieldTimer;
	private float timeSinceLastShieldHit;

	[Header("Animation")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation specialHitAnim;
	public SimpleAnimation shieldHitAnim;
	[Header("Audio")]
	public AudioClip[] hitSounds;
	public AudioClip[] specialHitSounds;
	public AudioClip specialChargeSound;

	public Coroutine specialAbilityChargeRoutine;

	public InputAction storedOnSwipe;
	public delegate void KnightEvent();
	public event KnightEvent OnKnightRush;
	public event KnightEvent OnKnightShield;
	public event KnightEvent OnKnightShieldHit;


	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		InitAbilities();
		// Handle input
		onDrag = RushAbility;
		onTap = AreaAttack;
		player.OnPlayerTryHit += CheckKnightShieldHit;
	}

	private void InitAbilities()
	{
		rushAbility.Init	   (player, onHitEnemyCallback: HandleOnDamageEnemy);
		areaAttackAbility.Init (player, onHitEnemyCallback: HandleOnDamageEnemy);
		specialRushAbility.Init(player, onHitEnemyCallback: HandleSpecialOnDamageEnemy);
	}

	protected override void Update() 
	{
		base.Update();
		if (shieldTimer > 0)
		{
			shieldIndicator.SetActive(true);
			shieldTimer -= Time.deltaTime;
			if (shieldTimer <= 0)
				shieldIndicator.GetComponent<IndicatorEffect>().AnimateOut();
		}
		else
		{
			shieldTimer = 0;
		}
		timeSinceLastShieldHit += Time.deltaTime;
	}

	public override void HandleMultiTouch()
	{
		// The knight is only able to parry if his shield is not active
		if (shieldTimer <= 0)
			base.HandleMultiTouch();
	}

	public void RushAbility()
	{
		// check cooldown
		if (!CheckIfCooledDownNotify (0, true, HandleDrag))
			return;
		ResetCooldownTimer (0);
		rushAbility.Execute();
		if (OnKnightRush != null)
			OnKnightRush();
	}

	public void AreaAttack()
	{
		// check cooldown
		if (!CheckIfCooledDownNotify (1))
			return;
		ResetCooldownTimer (1);
		// Play the indicator effect
		shieldIndicator.SetActive(false);
		shieldIndicator.SetActive(true);
		// Properties
		AddShieldTimer(300f);
		areaAttackAbility.SetPosition(transform.position);
		areaAttackAbility.Execute();

		if (OnKnightShield != null)
			OnKnightShield();
	}

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < specialAbilityChargeCapacity || 
		    specialRushAbility.inProgress)
			return;
		StartCoroutine(SpecialAbilityRoutine());
	}

	private IEnumerator SpecialAbilityRoutine()
	{
		sound.RandomizeSFX(specialChargeSound);
		onDrag -= RushAbility;
		specialRushAbility.Execute();
		specialRushAbility.specialRush.OnExecutedAction += () => { specialAbilityCharge = 0; };
		while (specialRushAbility.inProgress)
			yield return null;
		onDrag += RushAbility;
	}

	protected override void ParryEffect()
	{
		cooldownTimers[0] = 0f;
		AddShieldTimer(1f);
		areaAttackAbility.SetPosition(transform.position);
		areaAttackAbility.Execute();
	}

	public void ResetInvincibility()
	{
		shieldIndicator.GetComponent<IndicatorEffect>().AnimateOut();
	}

	private void HandleOnDamageEnemy(Enemy e)
	{
		DamageEnemy(e, damage, hitEffect, false, hitSounds);
	}

	private void HandleSpecialOnDamageEnemy(Enemy e)
	{
		int specialDamage = Mathf.RoundToInt(damage * 1.5f);
		DamageEnemy(e, specialDamage, specialHitAnim, true, specialHitSounds);
	}

	public void DamageEnemy(Enemy e, int dmg, SimpleAnimation effect, bool tempSlowDown, AudioClip[] sfx)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (dmg);
			EffectPooler.PlayEffect(effect, e.transform.position, true, 0.1f);
			player.TriggerOnEnemyDamagedEvent(dmg);
			player.TriggerOnEnemyLastHitEvent (e);

			sound.RandomizeSFX(sfx[Random.Range(0, sfx.Length)]);
			if (tempSlowDown)
				player.StartTempSlowDown(0.3f);
		}
	}

	public void AddShieldTimer(float amt)
	{
		shieldTimer += amt;
		player.invincibility.Add(shieldTimer);
	}

	private void CheckKnightShieldHit()
	{
		if (shieldTimer <= 0 || timeSinceLastShieldHit < 0.5f)
			return;
		timeSinceLastShieldHit = 0;
		if (OnKnightShieldHit != null)
			OnKnightShieldHit();
		EffectPooler.PlayEffect(shieldHitAnim, transform.position);
	}
}
