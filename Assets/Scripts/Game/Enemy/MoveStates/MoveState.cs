using UnityEngine;
using System.Collections;

public abstract class MoveState : MonoBehaviour
{
	protected Enemy enemy;
	protected EntityPhysics body;
	protected Transform player;

	public virtual void Init(Enemy e, Transform player)
	{
		this.enemy = e;
		this.body = e.body;
		this.player = player;
	}

	public abstract void UpdateState ();
	public virtual void Reset()
	{}
}

