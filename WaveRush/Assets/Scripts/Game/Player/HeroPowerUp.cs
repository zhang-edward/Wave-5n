using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public abstract class HeroPowerUp : MonoBehaviour
{
	[HideInInspector]
	public PlayerHero playerHero;

	public int stacks { get; protected set; }
	public float percentActivated { get; protected set; }

	public HeroPowerUpData data;

	public virtual void Activate(PlayerHero hero) 
	{
		playerHero = hero;
		percentActivated = 1f;
	}
	public virtual void Deactivate() {percentActivated = 0f;}
	public virtual void Stack() 
	{
		stacks++;
		Assert.IsFalse (stacks > data.maxStacks);
	}
}

