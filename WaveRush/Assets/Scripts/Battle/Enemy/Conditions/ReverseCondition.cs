using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class ReverseCondition : EnemyCondition
	{
		public EnemyCondition condition;

		public override void Init(EnemyAction action, Enemy e, Transform p)
		{
			base.Init(action, e, p);
			condition.Init(action, e, p);
		}

		public override bool Check()
		{
			return !condition.Check();
		}
	}
}

