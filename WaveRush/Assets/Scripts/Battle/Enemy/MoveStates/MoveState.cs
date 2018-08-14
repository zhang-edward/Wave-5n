using UnityEngine;
using System.Collections;

// Abstract class for all states that cause the player to move
public abstract class MoveState : MonoBehaviour
{
	protected Enemy enemy;
	protected EntityPhysics body;
	protected Transform player;
	protected AnimationSet anim;
	public string moveState = "Moving";

	public virtual void Init(Enemy e, Transform player)
	{
		this.enemy = e;
		this.body = e.body;
		this.anim = e.anim;
		this.player = player;
	}

	public abstract void UpdateState ();
	public virtual void Reset()
	{}
}

