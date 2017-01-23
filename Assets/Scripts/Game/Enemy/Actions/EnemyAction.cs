using UnityEngine;
using System.Collections;

public abstract class EnemyAction : MonoBehaviour
{
	public EnemyCondition[] conditions;

	protected Enemy e;

	public delegate void OnActionStateChanged();
	public OnActionStateChanged onActionFinished;

	public virtual void Init(Enemy e, OnActionStateChanged onActionFinished)
	{
		this.e = e;
		this.onActionFinished = onActionFinished;
		foreach (EnemyCondition condition in conditions)
			condition.Init (e, e.player);
	}

	public virtual bool CanExecute()
	{
		foreach (EnemyCondition condition in conditions)
		{
			if (!condition.Check ())
				return false;
		}
		return true;
	}

	public abstract void Interrupt();
	public abstract void Execute ();
}

