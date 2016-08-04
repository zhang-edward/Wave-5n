using UnityEngine;
using System.Collections;

public abstract class PlayerHero : MonoBehaviour {

	public const string KNIGHT = "knight";
	public const string MAGE = "mage";
	public const string NINJA = "ninja";

	[Header("Player Hero Properties")]
	public Sprite[] icons;
	public string heroName;

	protected Player player;
	protected EntityPhysics body;
	protected Animator anim;

	public int maxHealth;
	public int damage;

	public Sprite[] deathProps;

	public RuntimeAnimatorController animatorController;

	protected float[] abilityCooldowns;
	public float[] AbilityCooldowns {
		get {return abilityCooldowns;}
	}
	public float[] cooldownTime;
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

	public virtual void HandleSwipe()
	{}

	public virtual void Init(Player player, EntityPhysics body, Animator anim)
	{
		SoundManager.instance.PlaySingle (spawnSound);
		this.player = player;
		this.body = body;
		this.anim = anim;
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
}
