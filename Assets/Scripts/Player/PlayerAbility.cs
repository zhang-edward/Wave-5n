using UnityEngine;
using System.Collections;

public abstract class PlayerAbility : MonoBehaviour {

	public Sprite icon;

	protected Player player;
	protected EntityPhysics body;
	protected Animator anim;

	protected float abilityCooldown;

	public float AbilityCooldown {
		get {return abilityCooldown;}
	}
	public float cooldownTime;

	/// <summary>
	/// Performs an ability on click
	/// </summary>
	public abstract void Ability ();
	/// <summary>
	/// Performs an action on button held down
	/// </summary>
	public abstract void AbilityHoldDown();
	/// <summary>
	/// Resets the ability.
	/// </summary>
	public abstract void ResetAbility ();

	public void Init(Player player, EntityPhysics body, Animator anim)
	{
		this.player = player;
		this.body = body;
		this.anim = anim;
	}

	void Update()
	{
		if (abilityCooldown > 0)
			abilityCooldown -= Time.deltaTime;
	}
}
