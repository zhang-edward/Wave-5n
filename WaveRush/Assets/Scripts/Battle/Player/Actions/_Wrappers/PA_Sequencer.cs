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

		public void Init(Player player, params PlayerAction[] actions)
		{
			base.Init(player);
			this.actions = actions;
		}

		protected override void DoAction()
		{
			player.StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine()
		{
			inProgress = true;
			for (int i = 0; i < actions.Length; i ++) 
			{
				PlayerAction action = actions[i];
				action.Execute();
				if (action.duration > 0)
				{
					yield return new WaitForSeconds(action.duration);
				}
			}
			Debug.Log("Done");
			inProgress = false;
		}
	}
}
