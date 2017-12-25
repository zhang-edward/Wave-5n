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

	[Header("Animation")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation specialHitAnim;
	[Header("Audio")]
	public AudioClip[] hitSounds;
	public AudioClip[] specialHitSounds;
	public AudioClip specialChargeSound;

	public Coroutine specialAbilityChargeRoutine;

	public InputAction storedOnSwipe;
	public delegate void KnightAbilityActivated();
	public event KnightAbilityActivated OnKnightRush;
	public event KnightAbilityActivated OnKnightShield;

	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		InitAbilities();
		// handle input
		onDrag = RushAbility;
		onTap = AreaAttack;
	}

	private void InitAbilities()
	{
		rushAbility.Init(player, DamageEnemy);
		specialRushAbility.Init(player, SpecialDamageEnemy);
		areaAttackAbility.Init(player, DamageEnemy);
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
		// Properties
		AddShieldTimer(3f);
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
		AddShieldTimer(3f);
		areaAttackAbility.SetPosition(transform.position);
		areaAttackAbility.Execute();
	}

	public void ResetInvincibility()
	{
		shieldIndicator.GetComponent<IndicatorEffect>().AnimateOut();
	}

	public void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			EffectPooler.PlayEffect(hitEffect, e.transform.position);
			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent (e);

			sound.RandomizeSFX(hitSounds[Random.Range(0, hitSounds.Length)]);
		}
	}

	public void SpecialDamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage(damage);
			EffectPooler.PlayEffect(specialHitAnim, e.transform.position);
			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent(e);

			sound.RandomizeSFX(specialHitSounds[Random.Range(0, specialHitSounds.Length)]);

			player.StartTempSlowDown(0.3f);
		}
	}

	public void AddShieldTimer(float amt)
	{
		shieldTimer += amt;
		player.invincibility.Add(shieldTimer);
	}
}
