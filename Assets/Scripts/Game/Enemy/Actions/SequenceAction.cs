using UnityEngine;
using System.Collections;

public class SequenceAction : EnemyAction
{
	public EnemyAction[] actions;

	private EnemyAction currentAction {
		get {
			return actions [index];
		}
	}
	private int index;

	public override bool CanExecute ()
	{
		return currentAction.CanExecute ();
	}

	public override void Execute ()
	{
		currentAction.Execute ();
		index++;
		if (index >= actions.Length)
			index = 0;
	}

	public override void Interrupt ()
	{
		base.Interrupt ();
		currentAction.Interrupt ();
	}
}

