using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class JointAction : EnemyAction
	{
		[Tooltip("Note: Make sure no more than one action controls animations!")]
		public EnemyAction[] actions;
		public float spacing;
		public bool checkAllConditions; // whether all conditions must be met for all of the actions to be executed
		private int finishedActions = 0;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			foreach (EnemyAction action in actions)
				action.Init(e, IncrementFinishedActionsCounter);
		}

		public override bool CanExecute() {
			if (!base.CanExecute())
				return false;
			foreach (EnemyAction action in actions) {
				if (checkAllConditions && !action.CanExecute())
					return false;
				else if (!checkAllConditions && action.CanExecute())
					return true;
			}
			return true;
		}

		public override void Execute() {
			print ("executing " + this);
			base.Execute();
			StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine() {
			foreach (EnemyAction action in actions) {
				if (action.CanExecute()) {
					print ("Joint action executing: " + action);
					action.Execute();
				}
				yield return new WaitForSeconds(spacing);
			}
		}

		private void IncrementFinishedActionsCounter() {
			print ("finishedActionsBefore: " + finishedActions);
			finishedActions ++;
			print ("finishedActions: " + finishedActions);
			if (finishedActions >= actions.Length && onActionFinished != null) {
				print (this + " finished successfully");
				finishedActions = 0;
				onActionFinished();
			}
		}

		public override void Interrupt() {
			StopAllCoroutines();
			foreach (EnemyAction action in actions)
				action.Interrupt();
		}
	}
}


