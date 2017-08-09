using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemyActions
{
	public class CreateDynamicEffectAction : EnemyAction
	{
		public SimpleAnimation effect;
		public float fadeOutTime;
		public GenerateRandomPositionNearPlayerAction posGenerator;

		public override void Interrupt()
		{
			CancelInvoke();
		}

		public override void Execute()
		{
			base.Execute();
			Vector3 pos = posGenerator.GetGeneratedPosition();
			EffectPooler.PlayEffect(effect, pos, false, fadeOutTime);
		}
	}
}
