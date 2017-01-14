using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerHero : MonoBehaviour {

	public static readonly Dictionary<string, string> HERO_TYPES = new Dictionary<string, string> 
	{
		{"KNIGHT", "knight"},
		{"MAGE", "mage"},
		{"NINJA", "ninja"}
	};

	public Player player;
	protected EntityPhysics body;
	protected Animator anim;

	[Header("Ability Icons")]
	public Sprite[] icons;
	public Sprite specialAbilityIcon;
	public string heroName { get; protected set; }

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
	public float[] cooldownTimeNormal;		// the regular cooldown time for each ability. Set in inspector
	private float[] cooldownTime;			// the cooldown time used for each ability
	public float[] cooldownMultipliers;

	public int NumAbilities{
		get {return cooldownTimers.Length;}
	}

	[Header("Hero Audio")]
	public AudioClip spawnSound;

	protected delegate void InputAction();
	protected InputAction inputAction;

	void OnDisable()
	{
		player.OnEnemyDamaged -= IncrementCombo;
		player.OnEnemyDamaged -= IncrementSpecialAbilityCharge;
	}

	/// <summary>
	/// Performs an ability on swipe
	/// </summary>
	public virtual void HandleTapRelease ()
	{}

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

	/// <summary>
	/// Performs an action on button held down
	/// </summary>
	public virtual void HandleHoldDown()
	{}

	/// <summary>
	/// Performs an ability on swipe
	/// </summary>
	public virtual void HandleSwipe()
	{}

	/// <summary>
	/// Specials the ability.
	/// </summary>
	public abstract void SpecialAbility();

	public virtual void Init(EntityPhysics body, Animator anim, Player player)
	{
		SoundManager.instance.PlaySingle (spawnSound);
		powerUpHolder = GetComponent<HeroPowerUpHolder> ();
		this.body = body;
		this.anim = anim;
		this.player = player;
		cooldownTime = new float[cooldownTimeNormal.Length];
		cooldownMultipliers = new float[cooldownTimeNormal.Length];
		for(int i = 0; i < cooldownTime.Length; i ++)
		{
			cooldownTime [i] = cooldownTimeNormal [i];
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

	protected void ResetCooldownTimer(int index)
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

	public void ResetToDefaultCooldowns()
	{
		for (int i = 0; i < cooldownTime.Length; i ++)
		{
			cooldownTime [i] = cooldownTimeNormal [i];
		}
	}
}
