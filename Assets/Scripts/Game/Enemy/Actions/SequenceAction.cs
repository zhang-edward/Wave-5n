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

	public override void Init (Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init (e, onActionFinished);
		foreach (EnemyAction action in actions)
			action.Init (e, onActionFinished);
	}

	public override bool CanExecute ()
	{
		if (!base.CanExecute ())
			return false;
		return currentAction.CanExecute ();
	}

	public override void Execute ()
	{
		base.Execute ();
		print (currentAction);
		currentAction.Execute ();
		index++;
		if (index >= actions.Length)
			index = 0;
	}

	public override void Interrupt ()
	{
		if (!interruptable)
			return;
		currentAction.Interrupt ();
		index = 0;
	}
}

