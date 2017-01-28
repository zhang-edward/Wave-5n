using UnityEngine;
using System.Collections;

public class JointAction : EnemyAction
{
	[Tooltip("Note: Make sure no more than one action controls animations!")]
	public EnemyAction[] actions;
	public bool checkAllConditions; // whether all conditions must be met for all of the actions to be executed

	public override bool CanExecute ()
	{
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

