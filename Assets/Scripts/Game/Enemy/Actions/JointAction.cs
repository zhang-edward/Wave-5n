using UnityEngine;
using System.Collections;

public class JointAction : EnemyAction
{
	[Tooltip("Note: Make sure no more than one action controls animations!")]
	public EnemyAction[] actions;
	public bool checkAllConditions; // whether all conditions must be met for all of the actions to be executed

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
		foreach (EnemyAction action in actions)
		{
			if (checkAllConditions && !action.CanExecute ())
				return false;
			else if (!checkAllConditions && action.CanExecute ())
				return true;
		}
		return true;
	}

	public override void Execute ()
	{
		base.Execute ();
		foreach (EnemyAction action in actions)
			action.Execute ();
	}

	public override void Interrupt ()
	{
		base.Interrupt ();
		foreach (EnemyAction action in actions)
			action.Interrupt ();
	}
}

