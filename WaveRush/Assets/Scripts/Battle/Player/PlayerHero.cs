using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerHero : MonoBehaviour {

	/*public static readonly Dictionary<string, string> HERO_TYPES = new Dictionary<string, string> 
	{
		{"KNIGHT", "knight"},
		{"MAGE", "mage"},
		{"NINJA", "ninja"}
	};*/

	public const float PARRY_TIME = 0.5f;
	public const float PARRY_SLOW_TIME = 0.0f;
	public const float PARRY_COOLDOWN_TIME = 0.5f;

	[HideInInspector]
	public Player player;
	[HideInInspector]
	public EntityPhysics body;
	protected SoundManager sound;

	[Header("Ability Icons")]
	public Sprite[] icons;
	public Sprite specialAbilityIcon;
	public HeroType heroType;

	[Header("PlayerHero Properties")]
	public int maxHealth;
	public int baseDamage;
	public int damage {
		get {
			return Mathf.RoundToInt(baseDamage * damageMultiplier);
		}
	}
	public float damageMultiplier { get; set; }
	[Header("Animations/Skins")]
	public AnimationSet anim;

	[HideInInspector]
	public HeroPowerUpManager powerUpManager;

	[Space]
	public Sprite[] deathProps;

	public int combo { get; private set; }
	public int maxCombo { get; private set; }
	public float comboTimer { get; private set; }
	[HideInInspector]
	public float maxComboTimer = 3.0f;

	[Header("Special Ability Properties")]
	public float chargeMultiplier = 1;
	public float specialAbilityChargeCapacity;
	public float specialAbilityCharge { get; protected set; }

	protected float[] cooldownTimers;
	public float[] CooldownTimers {
		get {return cooldownTimers;}
	}
	[Header("Ability Cooldown Times")]
	public float[] cooldownTime;		// The regular cooldown time for each ability. Set in inspector
	[HideInInspector]
	public float[] cooldownMultipliers;	// The cooldown time multipliers, modified by powerups or abilities

	public int NumAbilities{
		get {return cooldownTimers.Length;}
	}

	private Coroutine listenForParryRoutine;

	public delegate void InputAction();
	protected InputAction inputAction;
	public InputAction onSwipe;
	public InputAction onTap;
	public InputAction onTapRelease;
	public InputAction onTapHoldDown;
	public InputAction onSpecialAbility;
	public InputAction onParry;
	public event Player.PlayerLifecycleEvent OnSpecialAbilityCharged;

	public delegate void OnAbility(int i);
	public event OnAbility OnAbilityFailed;

	void OnDisable()
	{
		player.OnEnemyDamaged -= IncrementCombo;
		player.OnEnemyDamaged -= IncrementSpecialAbilityCharge;
	}

	/// <summary>
	/// Performs an ability on tap
	/// </summary>
	public virtual void HandleTap ()
	{
		if (onTap != null)
			onTap();
	}

	public virtual void HandleTapRelease()
	{
		if (onTapRelease != null)
			onTapRelease();
	}

	/// <summary>
	/// Performs an action on button held down
	/// </summary>
	public virtual void HandleHoldDown()
	{
		if (onTapHoldDown != null)
			onTapHoldDown();
	}

	/// <summary>
	/// Performs an ability on swipe
	/// </summary>
	public virtual void HandleSwipe()
	{
		if (onSwipe != null)
			onSwipe();
	}

	public virtual void HandleMultiTouch()
	{
		listenForParryRoutine = StartCoroutine(ListenForParry());
	}

	private IEnumerator ListenForParry()
	{
		EffectPooler.PlayEffect(player.parryEffect, transform.position, true, 0.1f);
		player.StrobeColor(Color.yellow, PARRY_TIME - 0.1f);
		body.Move(Vector2.zero);
		player.OnPlayerTryHit += Parry;
		player.input.enabled = false;
		sound.PlaySingle(player.parrySound);
		yield return new WaitForSeconds(PARRY_TIME);
		player.FlashColor(Color.gray, PARRY_COOLDOWN_TIME);
		player.OnPlayerTryHit -= Parry;
		yield return new WaitForSeconds(PARRY_COOLDOWN_TIME);
		player.sr.color = Color.white;
		player.input.enabled = true;
	}

	private void Parry()
	{
		if (onParry != null)
			onParry();
		sound.PlaySingle(player.parrySuccessSound);
		player.isInvincible = true;
		player.HitDisable(0.5f);
		ParryEffect();
		CameraControl.instance.StartFlashColor(Color.white, 0.5f, 0, 0f, 0.5f);
		StopCoroutine(listenForParryRoutine);
		player.OnPlayerTryHit -= Parry;
		player.input.enabled = true;
		player.sr.color = Color.white;
		Invoke("ResetInvincibility", 0.1f);
	}

	private void ResetInvincibility()
	{
//		print("player is not invincible");
		player.isInvincible = false;
	}

	protected abstract void ParryEffect();

	/// <summary>
	/// Performs a special ability.
	/// </summary>
	public abstract void SpecialAbility();

	protected void QueueAction(float t)
	{
		StopCoroutine("QueueActionCoroutine");
		StartCoroutine ("QueueActionCoroutine", t);
	}

	private IEnumerator QueueActionCoroutine(float t)
	{
		yield return new WaitForSeconds (t);
		inputAction ();
	}

	public virtual void Init(EntityPhysics body, Player player, Pawn heroData)
	{
		powerUpManager = GetComponent<HeroPowerUpManager> ();
		this.body = body;
		this.player = player;
		sound = SoundManager.instance;

		anim = heroData.GetAnimationSet();
		anim.Init(player.animPlayer);
		anim.player.Init();
		damageMultiplier = 1f;
		baseDamage = Mathf.RoundToInt(Pawn.DamageEquation(heroData.level));
		// init cooldownMultipliers
		cooldownMultipliers = new float[cooldownTime.Length];
		for(int i = 0; i < cooldownTime.Length; i ++)
		{
			cooldownMultipliers [i] = 1;
		}
		player.maxHealth = maxHealth;
		powerUpManager.Init (heroData);
		player.OnPlayerDamaged += ResetCombo;
		player.OnEnemyDamaged += IncrementCombo;
		player.OnEnemyDamaged += IncrementSpecialAbilityCharge;
	}

	private IEnumerator Spawn()
	{
		EffectPooler.PlayEffect(player.spawnEffect, transform.position);
		yield return new WaitForSeconds(player.spawnEffect.GetSecondsUntilFrame(10));

	}

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

	// Checks if an ability is cooled down and, if not, notifies event listeners (for the HUD icons to flash red)
	public bool CheckIfCooledDownNotify(int index, bool queueActionIfFailed = false, InputAction input = null)
	{
		// check cooldown timer
		if (CooldownTimers[index] > 0)
		{
			if (queueActionIfFailed && CooldownTimers [index]< 0.1f)
			{
				inputAction = input;
				QueueAction (CooldownTimers [index]);
			}
			else
			{
				if (OnAbilityFailed != null)
					OnAbilityFailed(index);
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
		specialAbilityCharge += 1 * chargeMultiplier;
		if (specialAbilityCharge >= specialAbilityChargeCapacity)
		{
			if (OnSpecialAbilityCharged != null)
				OnSpecialAbilityCharged();
			specialAbilityCharge = specialAbilityChargeCapacity;
		}
	}

	public void IncrementSpecialAbilityChargeByAmt(float amt)
	{
		specialAbilityCharge += amt;
		if (specialAbilityCharge >= specialAbilityChargeCapacity)
		{
			if (OnSpecialAbilityCharged != null)
				OnSpecialAbilityCharged();
			specialAbilityCharge = specialAbilityChargeCapacity;
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
		SetChargeMultiplier ();
	}

	private void ResetCombo(int amt)
	{
		combo = 0;
		comboTimer = 0;
		SetChargeMultiplier ();
	}

	private void SetChargeMultiplier()
	{
		chargeMultiplier = combo * 0.05f + 1;
		if (chargeMultiplier > 2)
			chargeMultiplier = 2;
	}

	public float GetCooldownTime(int index)
	{
		return cooldownTime [index] * cooldownMultipliers [index];
	}

	public override string ToString()
	{
		return heroType.ToString();
	}
}
