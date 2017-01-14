using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public abstract class HeroPowerUp : MonoBehaviour
{
	[HideInInspector]
	public PlayerHero playerHero;

	public string powerUpName;
	public Sprite icon;
	public int stacks;
	public float percentActivated;

	[Header("Shop Item Properties")]
	public HeroPowerUp[] unlockable;
	public HeroPowerUp parent;			// the power up that needs to be unlocked for this one to be available;
	// for shop item
	public int maxStacks;
	public int cost;

	[Space]
	public string description;

	public virtual void Activate(PlayerHero hero) 
	{
		playerHero = hero;
		percentActivated = 1f;
	}
	public virtual void Deactivate() {percentActivated = 0f;}
	public virtual void Stack() 
	{
		stacks++;
		Assert.IsFalse (stacks > maxStacks);
	}
}

