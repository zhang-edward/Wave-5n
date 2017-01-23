using UnityEngine;
using System.Collections;

public abstract class MoveState : MonoBehaviour
{
	protected Enemy enemy;
	protected EntityPhysics body;
	protected Transform player;

	public virtual void Init(Enemy e, EntityPhysics body, Transform player)
	{
		this.enemy = e;
		this.body = body;
		this.player = player;
	}

	public abstract void UpdateState ();
	public virtual void Reset()
	{}
}

