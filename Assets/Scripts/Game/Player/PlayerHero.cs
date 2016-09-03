using UnityEngine;
using System.Collections;

public abstract class PlayerHero : MonoBehaviour {

	public const string KNIGHT = "knight";
	public const string MAGE = "mage";
	public const string NINJA = "ninja";

	[Header("Player Hero Properties")]
	public Sprite[] icons;
	public Sprite specialAbilityIcon;
	public string heroName;
	
	protected Player player;
	protected EntityPhysics body;
	protected Animator anim;

	public int maxHealth;
	public int damage;

	public Sprite[] deathProps;

	public RuntimeAnimatorController animatorController;

	public int combo { get; private set; }
	public int maxCombo { get; private set; }
	public float comboTimer { get; private set; }
	[HideInInspector]
	public float maxComboTimer = 3.0f;

	public float chargeMultiplier = 1;
	public float specialAbilityChargeCapacity;
	public float specialAbilityCharge { get; protected set; }

	protected float[] abilityCooldowns;
	public float[] AbilityCooldowns {
		get {return abilityCooldowns;}
	}

	public float[] cooldownTimeNormal;		// the regular cooldown time for each ability. Set in inspector
	public float[] cooldownTime { get; private set; }			// the cooldown time used for each ability

	public int NumAbilities{
		get {return abilityCooldowns.Length;}
	}

	[Header("Hero Audio")]
	public AudioClip spawnSound;

	public enum InputType {
		Tap,
		Swipe
	}
	//public InputType inputType;

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
		this.body = body;
		this.anim = anim;
		this.player = player;
		cooldownTime = new float[cooldownTimeNormal.Length];
		for(int i = 0; i < cooldownTime.Length; i ++)
		{
			cooldownTime [i] = cooldownTimeNormal [i];
		}
		player.maxHealth = maxHealth;
		player.OnPlayerDamaged += ResetCombo;
		player.OnEnemyDamaged += IncrementCombo;
		player.OnEnemyDamaged += IncrementSpecialAbilityCharge;
	}

	protected virtual void Update()
	{
		if (abilityCooldowns != null)
		{
			for (int i = 0; i < abilityCooldowns.Length; i++)
			{
				if (abilityCooldowns[i] > 0)
					abilityCooldowns [i] -= Time.deltaTime;
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

	protected void ResetCooldown(int index)
	{
		abilityCooldowns [index] = cooldownTime [index];
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
}
