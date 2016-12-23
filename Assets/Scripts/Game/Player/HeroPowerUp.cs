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
	public bool activated;

	[Header("Shop Item Properties")]
	public HeroPowerUp[] unlockable;
	// for shop item
	public int maxStacks;
	public int cost;

	[Space]
	public string description;

	public virtual void Activate(PlayerHero hero) {activated = true;}
	public virtual void Deactivate() {activated = false;}
	public virtual void Stack() {stacks++;}
}

