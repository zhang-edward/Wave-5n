using UnityEngine;

namespace EnemyActions 
{
	public abstract class EnemyAction : MonoBehaviour
	{
		public EnemyCondition[] conditions;
		public bool interruptable = true;

		protected Enemy e;

		public delegate void OnActionStateChanged();
		public OnActionStateChanged onActionFinished;
		public event OnActionStateChanged onExecute;

		public virtual void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			this.e = e;
			this.onActionFinished = onActionFinished;
			foreach (EnemyCondition condition in conditions)
			{
				condition.Init(this, e, e.player);
			}
		}

		public virtual bool CanExecute()
		{
			foreach (EnemyCondition condition in conditions)
			{
				if (!condition.Check())
					return false;
			}
			return true;
		}

		/// <summary>
		/// Used for when enemies are hit by an attack; if the enemy's action is interruptable, interrupt it
		/// </summary>
		public void TryInterrupt()
		{
			if (interruptable)
				Interrupt();
		}

		/// <summary>
		/// Interrupt this action. Can be called directly to force an interruption.
		/// </summary>
		public abstract void Interrupt();

		public virtual void Execute()
		{
			if (onExecute != null)
				onExecute();
		}
	}	
}