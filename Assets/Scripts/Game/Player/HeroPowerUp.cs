using UnityEngine;
using System.Collections;

public abstract class HeroPowerUp : MonoBehaviour
{
	[HideInInspector]
	public PlayerHero playerHero;

	public string powerUpName;
	public Sprite icon;
	[HideInInspector]
	public int stacks;

	[Header("Shop Item Properties")]
	public HeroPowerUp[] unlockable;
	// for shop item
	public int maxStacks;
	public int cost;

	[Space]
	public string description;

	public abstract void Activate(PlayerHero hero);
	public abstract void Deactivate();
	public virtual void Stack() {stacks++;}
}

