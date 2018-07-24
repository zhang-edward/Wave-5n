using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Quests;

public abstract class PlayerHero : MonoBehaviour {

	/** Constants */
	public const float PARRY_TIME = 0.7f;			// The parry time
	public const float PARRY_SLOW_TIME = 0.0f;		// How long the slow-down effect should be on successful parry
	public const float PARRY_COOLDOWN_TIME = 0.3f;	// How long the player should be inactive upon failed parry
	public const float SPECIAL_ABILITY_CHARGE_CAPACITY = 100.0f;

	/** Hidden properties */
	[HideInInspector]public Player player;
	[HideInInspector]public EntityPhysics body;
	[HideInInspector]public HeroPowerUpManager powerUpManager;
	[HideInInspector]public AnimationSet anim;
	[HideInInspector]public float[] cooldownMultipliers;	// The cooldown time multipliers, modified by powerups or abilities
	[HideInInspector]public int level;						// This hero's level
	[HideInInspector]public int hardHealth;					// The health of this hero, so when this hero is enabled the Player class will set the proper health
	[HideInInspector]public int healthPerHeart;				// The amount of health each heart contains for this hero
	[HideInInspector]public float[] stats;
	private int baseDamage;
	protected SoundManager sound;


	/** Hero properties */
	[Header("PlayerHero Properties")]
	public HeroType heroType;
	public int numHearts;
	public float hitDisableTime = 0.2f;
	public float damageMultiplier { get; set; }
	public int noiselessDamage { get { return Mathf.RoundToInt(baseDamage * damageMultiplier); } }
	public int damage {
		get {
			float noiseRange = 0.1f * baseDamage;
			float noise = Random.Range(-noiseRange, noiseRange);
			return Mathf.RoundToInt(baseDamage * damageMultiplier + noise);
		}
	}
	// Combo
	// TODO: Remove scores from the game
	public int   maxCombo   { get; private set; }	// The highest combo for this run
	public int   combo      { get; private set; }	// Combo score
	public float comboTimer { get; private set; }	// Combo timer - if this reaches 0, reset combo to 0
	public float maxComboTimer = 3.0f;

	// Special Ability
	public float specialAbilityCharge { get; protected set; }

	[Header("Ability Cooldown Times")]
	public float[] cooldownTime;		// The regular cooldown time for each ability. Set in inspector

	public float[] 	cooldownTimers	{ get; protected set; }

	// Misc
	private Coroutine listenForParryRoutine;
	private Coroutine queuedActionRoutine;
	private bool canParry = true;
	private int parryDisableTimerId;
	protected bool dragIndicatorEnabled = true;

	/** Delegates and events */
	// Input Actions
	public delegate void DirectionalInputAction(Vector3 dir);
	protected DirectionalInputAction inputAction;
	public	  DirectionalInputAction onTouchBegan;
	public	  DirectionalInputAction onTouchEnded;
	public    DirectionalInputAction onDragBegan;
	public 	  DirectionalInputAction onDragRelease;
	public    DirectionalInputAction onDragHold;
	public    DirectionalInputAction onTap;
	public    DirectionalInputAction onTapHoldRelease;
	public    DirectionalInputAction onTapHoldDown;
	public delegate void InputAction();
	public    InputAction onSpecialAbility;
	public    InputAction onParrySuccess;
	// Miscellaneous
	public event Player.PlayerLifecycleEvent OnSpecialAbilityCharged;
	public delegate void OnAbility(int i);
	public event OnAbility OnAbilityFailed;
	private bool initialized;

#region Initialization
	/// <summary>
	/// This is called once at the beginning of the game
	/// and initializes any essential fields
	/// </summary>
	public virtual void Init(EntityPhysics body, Player player, Pawn heroData) {
		this.body = body;
		this.player = player;

		powerUpManager = GetComponent<HeroPowerUpManager> ();
		sound = SoundManager.instance;

		// AnimationSet
		anim = heroData.GetAnimationSet();
		anim.Init(player.animPlayer);
		anim.player.Init();

		stats = heroData.GetStatsArray();
		level = heroData.level;
		damageMultiplier = 1f;
		baseDamage = Mathf.RoundToInt(Formulas.PlayerDamageFormula(heroData.level, (int)heroData.tier) * stats[StatData.STR]);
		healthPerHeart = (int)stats[StatData.VIT];

		// Init cooldownMultipliers to x1
		cooldownMultipliers = new float[cooldownTime.Length];
		for(int i = 0; i < cooldownTime.Length; i ++)
			cooldownMultipliers [i] = 1;

		hardHealth = numHearts * healthPerHeart;
		powerUpManager.Init (heroData, this);
		print ("Initializing powerUpManager " + powerUpManager.gameObject + "...");
		player.OnPlayerDamaged += ResetCombo;
		player.OnEnemyDamaged += IncrementCombo;
		player.OnEnemyDamaged += IncrementSpecialAbilityCharge;
		initialized = true;
	}
#endregion

	void OnEnable() {
		if (!initialized)
			return;
		anim.Init(player.animPlayer);
		anim.Play("Default");
		player.OnPlayerDamaged += ResetCombo;
		player.OnEnemyDamaged += IncrementCombo;
		player.OnEnemyDamaged += IncrementSpecialAbilityCharge;

	}

	void OnDisable() {
		player.OnPlayerDamaged -= ResetCombo;
		player.OnEnemyDamaged -= IncrementCombo;
		player.OnEnemyDamaged -= IncrementSpecialAbilityCharge;
	}

#region Input Handlers
	public void HandleTouchBegan(Vector3 dir) {
		if (onTouchBegan != null) 
			onTouchBegan(dir);
	}

	public void HandleTouchEnded(Vector3 dir) {
		if (onTouchEnded != null)
			onTouchEnded(dir);
	}

	/** Player Inputs */
	/// <summary>
	/// Performs an action on tap
	/// </summary>
	public void HandleTap (Vector3 dir) {
		if (onTap != null)
			onTap(dir);
	}

	/// <summary>
	/// Performs an action on tap release
	/// </summary>
	public void HandleTapHoldRelease(Vector3 dir)
	{
		if (onTapHoldRelease != null)
			onTapHoldRelease(dir);
	}

	/// <summary>
	/// Performs an action on screen held down
	/// </summary>
	public void HandleHoldDown(Vector3 dir)
	{
		if (onTapHoldDown != null)
			onTapHoldDown(dir);
	}

	/// <summary>
	/// Performs an action on drag
	/// </summary>
	public void HandleDragRelease(Vector3 dir)
	{
		if (onDragRelease != null)
			onDragRelease(dir);
		player.dirIndicator.gameObject.SetActive(false);
	}

	/// <summary>
	/// Performs an action on dragHoldDown
	/// </summary>
	public void HandleDragHold(Vector3 dir)
	{
		if (onDragHold != null)
			onDragHold(dir);
		
		if (dragIndicatorEnabled) {
			player.dirIndicator.gameObject.SetActive(true);
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			player.dirIndicator.rotation = Quaternion.Euler(0, 0, angle);
			float dirIndicatorLength = Mathf.Min(dir.magnitude, 2);			// Max length is 2
			player.dirIndicator.localScale = new Vector3(dirIndicatorLength, 0.5f, 1);
		}
	}

	/// <summary>
	/// Performs an action on started drag
	/// </summary>
	public void HandleDragBegan(Vector3 dir) {
		if (onDragBegan != null)
			onDragBegan(dir);
	}

	public void HandleDragCancel() {
		player.dirIndicator.gameObject.SetActive(false);
	}

	/// <summary>
	/// Performs a special ability.
	/// </summary>
	public abstract void SpecialAbility();

	protected void QueueAction(float t, Vector3 dir)
	{
		if (queuedActionRoutine != null)
			StopCoroutine(queuedActionRoutine);
		queuedActionRoutine = StartCoroutine(QueueActionRoutine(t, dir));
	}

	/// <summary>
	/// Queues the action
	/// </summary>
	/// <param name="t">Time</param>
	private IEnumerator QueueActionRoutine(float t, Vector3 dir)
	{
		yield return new WaitForSeconds(t);
		inputAction(dir);
	}

	public virtual void HandleMultiTouch()
	{
		// If the player was dragging
		player.dirIndicator.gameObject.SetActive(false);

		if (!canParry)
			return;
		player.OnPlayerTryHit += Parry;		// This is not inside the coroutine because of frame lag
		listenForParryRoutine = StartCoroutine(ListenForParry());
	}

	/** Parry */
	private IEnumerator ListenForParry()
	{
		// Effect
		EffectPooler.PlayEffect(player.parryEffect, transform.position, true, 0.1f);
		player.StrobeColor(Color.yellow, PARRY_TIME - 0.1f);
		// Lose Momentum
		float bodySpeed = body.rb2d.velocity.magnitude;
		if (bodySpeed > 0)
			body.Move(Vector2.zero, 0);
		// Player Properties
		parryDisableTimerId = player.inputDisabled.Add(PARRY_TIME);
		canParry = false;
		// Sound
		sound.PlaySingle(player.parrySound);

		yield return new WaitForSeconds(PARRY_TIME);

		// Effect
		player.FlashColor(Color.gray, PARRY_COOLDOWN_TIME);
		// Player Properties
		player.OnPlayerTryHit -= Parry;
		player.inputDisabled.Add(PARRY_COOLDOWN_TIME);

		yield return new WaitForSeconds(PARRY_COOLDOWN_TIME);

		// Effect
		player.sr.color = Color.white;
		// Player Properties
		canParry = true;
	}

	public void Parry(IDamageable src)
	{
		if (onParrySuccess != null)
			onParrySuccess();
		// Effect
		CameraControl.instance.StartFlashColor(Color.white, 0.5f, 0, 0f, 0.5f);
		// Player properties
		player.invincibility.Add(1.0f);
		player.sr.color = Color.white;
		player.OnPlayerTryHit -= Parry;
		StopCoroutine(listenForParryRoutine);
		// Sound
		sound.PlaySingle(player.parrySuccessSound);
		// Effect
		ParryEffect(src);
		StartCoroutine(ParryCooldown());
	}

	private IEnumerator ParryCooldown() {
		yield return new WaitForSeconds(0.1f);
		player.inputDisabled.RemoveTimer(parryDisableTimerId);
		yield return new WaitForSeconds(PARRY_COOLDOWN_TIME);
		canParry = true;
	}

	protected abstract void ParryEffect(IDamageable src);

	private IEnumerator Spawn()
	{
		EffectPooler.PlayEffect(player.spawnEffect, transform.position);
		yield return new WaitForSeconds(player.spawnEffect.GetSecondsUntilFrame(10));

	}
#endregion

	protected virtual void Update()
	{
		if (cooldownTimers != null)
		{
			for (int i = 0; i < cooldownTimers.Length; i++)
			{
				if (cooldownTimers[i] > 0)
					cooldownTimers [i] -= Time.deltaTime;
			}
		}
		if (comboTimer > 0)
		{
			comboTimer -= Time.deltaTime;
			if (comboTimer <= 0)
			{
				ResetCombo (0);
			}
		}
	}

	/// <summary>
	/// Checks if an ability is cooled down and, if not, notifies event listeners (for the HUD icons to flash red)
	/// </summary>
	/// <returns><c>true</c>if the ability with index <paramref name="index"/> is cooled down,<c>false</c> otherwise.</returns>
	/// <param name="index">Index of the ability.</param>
	public bool CheckIfCooledDownNotify(int index)
	{
		// If the ability is not cooled down
		if (cooldownTimers[index] > 0)
		{
			if (OnAbilityFailed != null)
				OnAbilityFailed(index);
			return false;
		}
		else
			return true;
	}

	/// <summary>
	/// Checks if an ability is cooled down and, if not, notifies event listeners (for the HUD icons to flash red)
	/// </summary>
	/// <returns><c>true</c>if the ability with index <paramref name="index"/> is cooled down,<c>false</c> otherwise.</returns>
	/// <param name="index">Index of the ability.</param>
	/// <param name="input">The input.</param>
	/// <param name="dir">The direction to pass to the input.</param>
	public bool CheckIfCooledDownNotify(int index, DirectionalInputAction input, Vector3 dir)
	{
		// If the ability is not cooled down
		if (cooldownTimers[index] > 0) {
			// If we can buffer the action, buffer it
			if (cooldownTimers [index]< 0.3f) {
				inputAction = input;
				QueueAction (cooldownTimers [index], dir);
			}
			return false;
		}
		else
			return true;
	}

	public void ResetCooldownTimer(int index)
	{
		cooldownTimers [index] = GetCooldownTime(index);
	}

	public void IncrementSpecialAbilityCharge(float amt)
	{
		specialAbilityCharge += stats[StatData.CHG];
		if (specialAbilityCharge >= SPECIAL_ABILITY_CHARGE_CAPACITY)
		{
			if (OnSpecialAbilityCharged != null)
				OnSpecialAbilityCharged();
			specialAbilityCharge = SPECIAL_ABILITY_CHARGE_CAPACITY;
		}
	}

	public void IncrementSpecialAbilityChargeByAmt(float amt)
	{
		specialAbilityCharge += amt;
		if (specialAbilityCharge >= SPECIAL_ABILITY_CHARGE_CAPACITY)
		{
			if (OnSpecialAbilityCharged != null)
				OnSpecialAbilityCharged();
			specialAbilityCharge = SPECIAL_ABILITY_CHARGE_CAPACITY;
		}	
	}

	private void IncrementCombo(float amt)
	{
		combo++;
		if (combo > maxCombo)
			maxCombo = combo;
		comboTimer += 1.5f;
		if (comboTimer > maxComboTimer || combo == 1)		// if just started combo, set to max timer
			comboTimer = maxComboTimer;
	}

	private void ResetCombo(int amt)
	{
		combo = 0;
		comboTimer = 0;
	}

	public float GetCooldownTime(int index)
	{
		return cooldownTime [index] * cooldownMultipliers [index];
	}

	public override string ToString()
	{
		return heroType.ToString();
	}

	public bool TryCriticalDamage(ref int damage) {
		if (Random.value > stats[StatData.DEX])
			return false;
		else {
			damage = (int)(stats[StatData.CRIT] * damage);
			return true;
		}
	}

	public Quest GetUnlockQuest(HeroTier tier) {
		Quest quest = UnlockQuest(tier);
		if (quest == null) {
			Debug.LogError(string.Format("No unlock quest for {0} (tier {1})", heroType, tier));
		}
		return quest;
	}

	protected abstract Quest UnlockQuest(HeroTier tier);
}