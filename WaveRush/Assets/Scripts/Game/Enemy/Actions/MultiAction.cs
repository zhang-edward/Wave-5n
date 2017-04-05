namespace EnemyActions
{
	public class MultiAction : EnemyAction
	{
		public EnemyAction[] actions;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			foreach (EnemyAction action in actions)
				action.Init(e, onActionFinished);
		}

		public override bool CanExecute()
		{
			if (!base.CanExecute())
				return false;
			foreach (EnemyAction action in actions)
			{
				if (action.CanExecute())
				{
					return true;
				}
			}
			return false;
		}

		public override void Execute()
		{
			base.Execute();
			foreach (EnemyAction action in actions)
			{
				if (action.CanExecute())
				{
					action.Execute();
					return;
				}
			}
		}

		public override void Interrupt()
		{
			if (!interruptable)
				return;
			foreach (EnemyAction action in actions)
				action.Interrupt();
		}
	}
}