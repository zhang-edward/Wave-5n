namespace PlayerActions
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[System.Serializable]
	public class PA_Sequencer : PlayerAction
	{
		public PlayerAction[] actions;
		public bool inProgress { get; private set; }

		public void Init(Player player, params PlayerAction[] actions) {
			base.Init(player);
			this.actions = actions;

			for (int i = 0; i < actions.Length - 1; i ++) {
				// When one action finishes, it will trigger the execution of the next action
				actions[i].OnActionFinished += actions[i + 1].Execute;
				// Set this duration to be the sum of all its subactions' durations
				duration += actions[i].duration;
			}

			Debug.Log(actions.Length - 1);
			// Set the last action
			actions[actions.Length - 1].OnActionFinished += this.FinishAction;
			duration += actions[actions.Length - 1].duration;

			OnActionFinished += () => { inProgress = false; Debug.Log("Done"); };
		}

		protected override void DoAction() {
			inProgress = true;
			actions[0].Execute();
		}
	}
}
