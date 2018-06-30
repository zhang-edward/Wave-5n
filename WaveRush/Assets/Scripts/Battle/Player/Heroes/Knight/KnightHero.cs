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

	private const float SHIELD_TIME = 3f;

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
	public AudioClip   specialChargeSound;
	public AudioClip   shieldHitSound;

	public Coroutine specialAbilityChargeRoutine;

	public InputAction storedOnSwipe;
	public delegate void KnightEvent();
	public event KnightEvent OnKnightRush;
	public event KnightEvent OnKnightShield;
	public event Player.PlayerTargetedEnemyEvent OnKnightShieldHit;
	public event Player.EnemySelected OnKnightRushHitEnemy;

#region Initialization
	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		InitAbilities();
		// Handle input
		onDragRelease = Rush;
		onTap = AreaAttack;
		player.OnPlayerTryHit += CheckKnightShieldHit;
	}

	private void InitAbilities()
	{
		rushAbility.Init	   (player, onHitEnemyCallback: HandleRushHitEnemy);
		areaAttackAbility.Init (player, onHitEnemyCallback: (enemy) => { PushEnemyBack(enemy, 10f, 0.25f); });
		specialRushAbility.Init(player, onHitEnemyCallback: HandleSpecialOnDamageEnemy);
	}
#endregion

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

	public void Rush()
	{
		// check cooldown
		if (!CheckIfCooledDownNotify (0, true, HandleDragRelease))
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
		AddShieldTimer(SHIELD_TIME);
		areaAttackAbility.SetPosition(transform.position);
		areaAttackAbility.Execute();

		if (OnKnightShield != null)
			OnKnightShield();
	}

	public override void SpecialAbility ()
	{
		if (specialAbilityCharge < SPECIAL_ABILITY_CHARGE_CAPACITY || 
		    specialRushAbility.inProgress)
			return;
		StartCoroutine(SpecialAbilityRoutine());
	}

	private IEnumerator SpecialAbilityRoutine()
	{
		sound.RandomizeSFX(specialChargeSound);
		onDragRelease -= Rush;
		specialRushAbility.Execute();
		specialRushAbility.specialRush.OnExecutedAction += () => { specialAbilityCharge = 0; };
		while (specialRushAbility.inProgress)
			yield return null;
		onDragRelease += Rush;
	}

	protected override void ParryEffect(IDamageable src)
	{
		cooldownTimers[0] = 0f;
		AddShieldTimer(1f);
		
		Enemy e = src as Enemy;
		if (e != null && Vector2.Distance(transform.position, e.transform.position) < 2f) {
			PushEnemyBack(e, 8f, 0.1f);
			StartCoroutine(StunEnemyDelayed(e, 0.1f, 5.0f));
		}
	}

	public void ResetInvincibility()
	{
		shieldIndicator.GetComponent<IndicatorEffect>().AnimateOut();
	}

	private void HandleRushHitEnemy(Enemy e)
	{
		// Add impulse to enemy (knock enemy away)
		e.Disable(0.2f);
		e.body.AddImpulse(e.transform.position - transform.position);
		int damageDealt = damage;

		// Damage enemy
		if (TryCriticalDamage(ref damageDealt))
			DamageEnemy(e, damageDealt, specialHitAnim, false, specialHitSounds);
		else
			DamageEnemy(e, damageDealt, hitEffect, false, hitSounds);
		
		// Event call
		if (OnKnightRushHitEnemy != null)
			OnKnightRushHitEnemy(e);
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
			e.Damage (dmg, player);
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

	private void CheckKnightShieldHit(IDamageable source)
	{
		if (shieldTimer <= 0)
			return;
		if (OnKnightShieldHit != null)
			OnKnightShieldHit(source);
		EffectPooler.PlayEffect(shieldHitAnim, transform.position, false, 0.2f);
		sound.RandomizeSFX(shieldHitSound);

		Enemy e = source as Enemy;
		if (e != null && Vector2.Distance(transform.position, e.transform.position) < 2f) {
			PushEnemyBack(e, 4f, 0.5f);
			StartCoroutine(StunEnemyDelayed(e, 0.5f, 2.0f));
		}
	}

	private void PushEnemyBack(Enemy e, float strength, float time) {
		Vector2 awayFromPlayerDir = (e.transform.position - transform.position).normalized;
		e.Disable(0.5f);
		e.body.AddImpulse(awayFromPlayerDir, strength);
	}

	private void StunEnemy(Enemy e, float stunTime) {
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = stunTime;
		e.AddStatus(stun.gameObject);
	}

	private IEnumerator StunEnemyDelayed(Enemy e, float delay, float stunTime) {
		yield return new WaitForSeconds(0.5f);
		StunEnemy(e, stunTime);
	}
}
