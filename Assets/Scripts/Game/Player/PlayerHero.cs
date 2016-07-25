using UnityEngine;
using System.Collections;

public abstract class PlayerHero : MonoBehaviour {

	public Sprite primaryAbilityIcon;
	public string heroName;

	protected Player player;
	protected EntityPhysics body;
	protected Animator anim;

	public int maxHealth;
	public int damage;

	public Sprite[] deathProps;

	public RuntimeAnimatorController animatorController;

	protected float abilityCooldown;
	public float AbilityCooldown {
		get {return abilityCooldown;}
	}
	public float cooldownTime;

	public enum InputType {
		Tap,
		Swipe
	}
	public InputType inputType;

	/// <summary>
	/// Performs an ability on swipe
	/// </summary>
	public virtual void Ability ()
	{}
	/// <summary>
	/// Performs an action on button held down
	/// </summary>
	public virtual void AbilityHoldDown()
	{}
	/// <summary>
	/// Resets the ability.
	/// </summary>
	public abstract void ResetAbility ();

	public virtual void Init(Player player, EntityPhysics body, Animator anim)
	{
		this.player = player;
		this.body = body;
		this.anim = anim;
		player.maxHealth = maxHealth;
	}

	void Update()
	{
		if (abilityCooldown > 0)
			abilityCooldown -= Time.deltaTime;
	}
}
