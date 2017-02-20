using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyActions
{
	public abstract class EnemyCondition : MonoBehaviour
	{
		protected Enemy e;
		protected Transform player;
		protected EnemyAction action;

		public virtual void Init(EnemyAction action, Enemy e, Transform p)
		{
			this.action = action;
			this.e = e;
			this.player = p;
		}

		public abstract bool Check();
	}
}
