using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemyActions
{
	public class GenerateRandomPositionNearPlayerAction : EnemyAction
	{
		private Vector3 generatedPosition;
		private Transform playerTransform;
		public float randomOffset;

		public override void Init(Enemy e, EnemyAction.OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			playerTransform = e.playerTransform;
		}

		public override void Interrupt()
		{
			CancelInvoke();
		}

		public override void Execute()
		{
			generatedPosition = playerTransform.position +
				new Vector3(Random.Range(-randomOffset, randomOffset), Random.Range(-randomOffset, randomOffset));
			if (onActionFinished != null)
				onActionFinished();
		}

		public Vector3 GetGeneratedPosition()
		{
			return generatedPosition;
		}
	}
}
