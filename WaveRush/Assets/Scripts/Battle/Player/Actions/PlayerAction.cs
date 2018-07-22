using UnityEngine;
using System.Collections;

namespace PlayerActions
{
	public abstract class PlayerAction
	{
		protected Player player;
		protected PlayerHero hero;

		public float duration = -1;
		public delegate void ActionEvent();
		public event ActionEvent OnExecutedAction;
		public event ActionEvent OnActionFinished;

		private Coroutine finishActionRoutine;

		public virtual void Init(Player player)
		{
			this.player = player;
			this.hero = player.hero;
		}

		public void Execute() {
			DoAction();
			if (OnExecutedAction != null)
				OnExecutedAction();
			finishActionRoutine = player.StartCoroutine(FinishActionRoutine());
			// Debug.Log("Executing " + ToString());
		}

		protected abstract void DoAction();
		private IEnumerator FinishActionRoutine() {
			yield return new WaitForSeconds(duration);
			FinishAction();
			// Debug.Log("Finished " + ToString());
		}

		protected void FinishAction() {
			if (OnActionFinished != null)
				OnActionFinished();
		}
	}
}
