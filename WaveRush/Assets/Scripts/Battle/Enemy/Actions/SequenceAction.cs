namespace EnemyActions
{
	public class SequenceAction : EnemyAction
	{
		public EnemyAction[] actions;

		private EnemyAction currentAction
		{
			get
			{
				return actions[index];
			}
		}
		private int index;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			for (int i = 0; i < actions.Length - 1; i ++)
				actions[i].Init(e, TryExecuteNextAction);
			actions[actions.Length - 1].Init(e, onActionFinished);
		}

		public override bool CanExecute()
		{
			if (!base.CanExecute())
				return false;
			return currentAction.CanExecute();
		}

		private void TryExecuteNextAction() {
			print (currentAction);
			if (CanExecute()) {
				Execute();
			}
		}

		public override void Execute() {
			base.Execute();
			print(currentAction);
			currentAction.Execute();
			index++;
			if (index >= actions.Length)
				index = 0;
		}

		public override void Interrupt()
		{
			print ("Interrupted");
			// Interrupt ALL actions
			foreach (EnemyAction action in actions)
				action.Interrupt();
			index = 0;
		}
	}
}

