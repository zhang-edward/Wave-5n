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

	public int combo = 0;

	public float chargeMultiplier;
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

	void OnEnable()
	{
		player = GetComponentInParent<Player> ();
		player.OnEnemyDamaged += IncrementSpecialAbilityCharge;
	}

	void OnDisable()
	{
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

	public virtual void Init(EntityPhysics body, Animator anim)
	{
		SoundManager.instance.PlaySingle (spawnSound);
		this.body = body;
		this.anim = anim;
		cooldownTime = new float[cooldownTimeNormal.Length];
		for(int i = 0; i < cooldownTime.Length; i ++)
		{
			cooldownTime [i] = cooldownTimeNormal [i];
		}
		player.maxHealth = maxHealth;
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
	}

	protected void ResetCooldown(int index)
	{
		abilityCooldowns [index] = cooldownTime [index];
	}

	public void IncrementSpecialAbilityCharge(float amt)
	{
		specialAbilityCharge+= amt;
		if (specialAbilityCharge >= specialAbilityChargeCapacity)
		{
			specialAbilityCharge = specialAbilityChargeCapacity;
		}
	}
}
