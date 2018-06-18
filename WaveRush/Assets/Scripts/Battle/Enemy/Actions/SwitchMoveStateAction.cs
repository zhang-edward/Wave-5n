using UnityEngine;
using System.Collections;


namespace EnemyActions
{
	public class SwitchMoveStateAction : EnemyAction
	{
		public MoveState conditionalMoveState;
		[Header("Effect (Optional)")]
		public SimpleAnimation switchEffect;

		private bool isConditionalState;

		public override void Init(Enemy e, EnemyAction.OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			conditionalMoveState.Init(e, e.playerTransform);
		}

		public override void Execute()
		{
			base.Execute();
			e.movementMethod = conditionalMoveState;
			if (switchEffect.frames.Length > 0)
				EffectPooler.PlayEffect(switchEffect, e.transform.position + e.healthBarOffset * 1.1f, false, 0.2f);
			if (onActionFinished != null)
				onActionFinished();
		}

		public override bool CanExecute()
		{
			isConditionalState = e.movementMethod == conditionalMoveState;
			return base.CanExecute() && !isConditionalState;
		}

		public override void Interrupt()
		{
		}
	}
}
