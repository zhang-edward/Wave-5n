using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public abstract class HeroPowerUp : MonoBehaviour
{
	[HideInInspector]
	public PlayerHero playerHero;

	public HeroPowerUpInfo info;

	// public string powerUpName;
	// public Sprite icon;
	public int stacks;
	public float percentActivated;

	//[Header("Shop Item Properties")]
	// public HeroPowerUp[] unlockable;
	// if this is not a sub-powerup of another powerup (if another powerup has this in its "unlockable" field
	// public bool isRoot = true;
	// for shop item
	// public int maxStacks;
	// public int cost;

	/*[Space]
	[TextArea]
	// public string description;
	[TextArea]
	// public string stackDescription;*/

	public virtual void Activate(PlayerHero hero) 
	{
		playerHero = hero;
		percentActivated = 1f;
	}
	public virtual void Deactivate() {percentActivated = 0f;}
	public virtual void Stack() 
	{
		stacks++;
		Assert.IsFalse (stacks > info.maxStacks);
	}
}

