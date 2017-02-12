
public class DelayAction : EnemyAction
{
	public float delay;
	public EnemyAction action;

	public override void Init(Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init(e, onActionFinished);
		action.Init(e, onActionFinished);
	}

	public override void Execute()
	{
		base.Execute();
		Invoke("ExecuteAll", delay);
	}

	private void ExecuteAll()
	{
		action.Execute();
	}

	public override void Interrupt()
	{
		if (interruptable)
			action.Interrupt();
	}
}
