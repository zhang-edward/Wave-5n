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

	[HideInInspector]
	public Player player;
	[HideInInspector]
	public EntityPhysics body;
	[HideInInspector]
	public Animator anim;

	[Header("Ability Icons")]
	public Sprite[] icons;
	public Sprite specialAbilityIcon;
	public HeroType heroType;

	[Header("PlayerHero Properties")]
	public int maxHealth;
	public int damage;

	[HideInInspector]
	public HeroPowerUpHolder powerUpHolder;

	[Space]
	public Sprite[] deathProps;
	[Space]
	public RuntimeAnimatorController animatorController;

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
	public float[] cooldownTime;		// the regular cooldown time for each ability. Set in inspector
	[HideInInspector]
	public float[] cooldownMultipliers;

	public int NumAbilities{
		get {return cooldownTimers.Length;}
	}

	[Header("Hero Audio")]
	public AudioClip spawnSound;

	public delegate void InputAction();
	protected InputAction inputAction;
	public InputAction onSwipe;
	public InputAction onTapRelease;
	public InputAction onTapHoldDown;
	public InputAction onSpecialAbility;

	void OnDisable()
	{
		player.OnEnemyDamaged -= IncrementCombo;
		player.OnEnemyDamaged -= IncrementSpecialAbilityCharge;
	}

	/// <summary>
	/// Performs an ability on swipe
	/// </summary>
	public virtual void HandleTapRelease ()
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

	public virtual void Init(EntityPhysics body, Animator anim, Player player)
	{
		SoundManager.instance.PlaySingle (spawnSound);
		powerUpHolder = GetComponent<HeroPowerUpHolder> ();
		this.body = body;
		this.anim = anim;
		this.player = player;
		// init cooldownMultipliers
		cooldownMultipliers = new float[cooldownTime.Length];
		for(int i = 0; i < cooldownTime.Length; i ++)
		{
			cooldownMultipliers [i] = 1;
		}
		player.maxHealth = maxHealth;
		powerUpHolder.Init ();
		player.OnPlayerDamaged += ResetCombo;
		player.OnEnemyDamaged += IncrementCombo;
		player.OnEnemyDamaged += IncrementSpecialAbilityCharge;
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

	public bool IsCooledDown(int index, bool queueActionIfFailed = false, InputAction input = null)
	{
		// check cooldown timer
		if (CooldownTimers[index] > 0)
		{
			if (queueActionIfFailed && CooldownTimers [index]< 0.3f)
			{
				inputAction = input;
				QueueAction (CooldownTimers [index]);
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
		specialAbilityCharge += amt * chargeMultiplier;
		if (specialAbilityCharge >= specialAbilityChargeCapacity)
		{
			specialAbilityCharge = specialAbilityChargeCapacity;
		}
	}

	private void IncrementCombo(float amt)
	{
		combo++;
		if (combo > maxCombo)
			maxCombo = combo;
		comboTimer += 1.5f;
		if (comboTimer > maxComboTimer)
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
