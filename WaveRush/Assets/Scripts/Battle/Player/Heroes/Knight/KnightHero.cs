using UnityEngine;
using PlayerActions;
using System.Collections;
using System.Collections.Generic;

public class KnightHero : PlayerHero {

	[System.Serializable]
	public class PA_SpecialRush : PA_Joint {

		public PA_Animate chargeAnim;
		public PA_EffectAttached chargeEffect;
		public PA_SpecialAbilityEffect specialEffect;
		public PA_Rush specialRush;

		// private PA_Joint chargeAction = new PA_Joint();
		private PA_InputListener inputListener = new PA_InputListener();

		public void Init(Player player, PA_Rush.HitEnemy onHitEnemyCallback)
		{
			actions = new PlayerAction[2];

			chargeAnim.Init(player);
			chargeEffect.Init(player);
			specialEffect.Init(player, specialRush);
			inputListener.input = PA_InputListener.InputType.Drag;
			inputListener.duration = 2.0f;
			inputListener.Init(player, ExecuteSpecialRush);
			specialRush.Init(player, onHitEnemyCallback);

			base.Init(player, chargeAnim, 
			                  chargeEffect,
			                  specialEffect,
							  inputListener);
		}

		private void ExecuteSpecialRush(Vector3 dir) {
			specialRush.SetDirection(dir);
			specialRush.Execute();
			FinishAction();
		}
	}

	public const float SHIELD_TIME = 15f;
	public const int   SHIELD_MAXHEALTH = 4;

	[Header("Abilities")]
	public PA_Rush rushAbility;
	public PA_AreaEffect areaAttackAbility;
	public PA_SpecialRush specialRushAbility;
	[Header("Effects")]
	public GameObject rushEffect;
	public GameObject specialRushEffect;
	public GameObject shieldIndicator;
	[Header("Animation")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation specialHitAnim;
	public SimpleAnimation shieldHitAnim;
	[Header("Audio")]
	public AudioClip[] hitSounds;
	public AudioClip[] specialHitSounds;
	public AudioClip   specialChargeSound;
	public AudioClip   shieldHitSound;
	public AudioClip   shieldSound;
	[Header("Misc")]
	public GameObject shieldMeterPrefab;

	// Properties
	public int   shieldHealth { get; private set; }
	public float shieldTimer  { get; private set; }

	// Private fields
	private float timeSinceLastShieldHit;
	private bool specialActivated;
	private int shieldTimerId;

	// Coroutines
	public Coroutine specialAbilityChargeRoutine;

	// Events
	public DirectionalInputAction storedOnSwipe;
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

		// Instantiate meter
		GameObject o = Instantiate(shieldMeterPrefab);
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Tutorial1") {
			o.transform.SetParent(TutorialScene1Manager.instance.gui.customUI, false);
		}
		else {
			o.transform.SetParent(BattleSceneManager.instance.gui.customUI, false);
		}
		o.GetComponent<KnightShieldMeter>().Init(this);
	}

	private void InitAbilities()
	{
		rushAbility.Init	   (player, onHitEnemyCallback: HandleRushHitEnemy);
		areaAttackAbility.Init (player, onHitEnemyCallback: (enemy) => { PushEnemyBack(enemy, 10f, 0.25f); });
		specialRushAbility.Init(player, onHitEnemyCallback: HandleSpecialOnDamageEnemy);
			specialRushAbility.specialRush.OnExecutedAction += () => { 
			specialAbilityCharge = 0; 
			player.invincibility.Add(specialRushAbility.specialRush.duration);
		};
		specialRushAbility.specialRush.OnActionFinished += ResetSpecialAbility;		// Special resets if special rush is executed
		specialRushAbility.OnActionFinished += ResetSpecialAbility;					// Special also resets if special rush fails to execute (player waited too long)
	}
#endregion

	protected override void Update() 
	{
		base.Update();
		if (shieldTimer > 0)
		{
			shieldIndicator.SetActive(true);
			shieldTimer -= Time.deltaTime;
			if (shieldTimer <= 0) {
				DestroyShield();
			}
		}
		timeSinceLastShieldHit += Time.deltaTime;
	}

	public override void HandleMultiTouch()
	{
		// The knight is only able to parry if his shield is not active
		if (shieldTimer <= 0)
			base.HandleMultiTouch();
	}

	public void Rush(Vector3 dir)
	{
		// check cooldown
		if (!CheckIfCooledDownNotify (0, HandleDragRelease, dir))
			return;
		ResetCooldownTimer (0);
		rushAbility.SetDirection(dir);
		rushAbility.Execute();
		if (OnKnightRush != null)
			OnKnightRush();
	}

	public void AreaAttack(Vector3 dir)
	{
		// check cooldown
		if (!CheckIfCooledDownNotify (1))
			return;
		ResetCooldownTimer (1);
		sound.RandomizeSFX(shieldSound);
		// Play the indicator effect
		shieldIndicator.SetActive(false);
		shieldIndicator.SetActive(true);
		// Properties
		AddShieldTimer(SHIELD_TIME);
		// areaAttackAbility.SetPosition(transform.position);
		// areaAttackAbility.Execute();

		if (OnKnightShield != null)
			OnKnightShield();
	}

	public override void SpecialAbility () {
		if (specialAbilityCharge < SPECIAL_ABILITY_CHARGE_CAPACITY || specialActivated)
			return;
		sound.RandomizeSFX(specialChargeSound);
		onDragRelease -= Rush;
		specialRushAbility.Execute();
		specialActivated = true;
	}

	private void ResetSpecialAbility() {
		if (!specialActivated)
			return;
		onDragRelease += Rush;
		specialActivated = false;
	}

	protected override void ParryEffect(IDamageable src)
	{
		cooldownTimers[0] = 0f;
		
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
			player.TriggerOnEnemyDamagedEvent(dmg);
			player.TriggerOnEnemyLastHitEvent (e);

			if (effect != null)
				EffectPooler.PlayEffect(effect, e.transform.position, true, 0.1f);
			if (sfx != null)
				sound.RandomizeSFX(sfx[Random.Range(0, sfx.Length)]);
			if (tempSlowDown)
				player.StartTempSlowDown(0.3f);
		}
	}

	public void AddShieldTimer(float amt)
	{
		shieldTimer += amt;
		shieldHealth = SHIELD_MAXHEALTH;
		shieldTimerId = player.invincibility.Add(shieldTimer);
	}

	private void CheckKnightShieldHit(IDamageable source)
	{
		if (shieldTimer <= 0 || timeSinceLastShieldHit < 0.2f)
			return;
		EffectPooler.PlayEffect(shieldHitAnim, transform.position, false, 0.2f);
		sound.RandomizeSFX(shieldHitSound);

		Enemy e = source as Enemy;
		if (e != null && Vector2.Distance(transform.position, e.transform.position) < 2f) {
			PushEnemyBack(e, 4f, 0.5f);
		}
		timeSinceLastShieldHit = 0;
		shieldHealth --;
		if (shieldHealth <= 0)
			DestroyShield();
		if (OnKnightShieldHit != null)
			OnKnightShieldHit(source);
	}

	private void DestroyShield () {
		shieldTimer = 0;
		shieldIndicator.GetComponent<IndicatorEffect>().AnimateOut();
		player.invincibility.RemoveTimer(shieldTimerId);
		player.invincibility.Add(0.5f);		// Add a little invincibility after shield is broken to prevent any cheap hits from active hitboxes
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

	protected override Quests.Quest UnlockQuest(HeroTier tier) {
		switch (tier) {
			case HeroTier.tier1:
				return null;
			case HeroTier.tier2:
				return null;
			case HeroTier.tier3:
				return null;	// TODO: Do this
			default:
				return null;
		}
	}
}
