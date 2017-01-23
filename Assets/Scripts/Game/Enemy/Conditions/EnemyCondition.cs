using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCondition : MonoBehaviour 
{
	protected Enemy e;
	protected Transform player;

	public virtual void Init(Enemy e, Transform p)
	{
		this.e = e;
		this.player = p;
	}

	public abstract bool Check ();
}
