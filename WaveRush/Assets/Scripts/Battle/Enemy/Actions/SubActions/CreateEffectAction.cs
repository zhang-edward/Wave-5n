using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class CreateEffectAction : EnemyAction
	{
		public Transform location;
		public TempObjectInfo info;
		public SimpleAnimation anim;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, null);
			
		}

		public override void Execute()
		{
			base.Execute();
			EffectPooler.PlayEffect(anim, location.position, info);
		}

		public override void Interrupt()
		{
		}
	}
}

