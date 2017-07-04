using UnityEngine;
using System.Collections;


namespace EnemyActions
{
	public class SwitchMoveStateAction : EnemyAction
	{
		public MoveState conditionalMoveState;
		private bool isConditionalState;

		public override void Init(Enemy e, EnemyAction.OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			conditionalMoveState.Init(e, e.player);
		}

		public override void Execute()
		{
			base.Execute();
			e.movementMethod = conditionalMoveState;
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
