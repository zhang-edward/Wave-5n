namespace PlayerActions
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[System.Serializable]
	public class PA_Joint : PlayerAction
	{
		public PlayerAction[] actions;

		public void Init(Player player, params PlayerAction[] actions)
		{
			base.Init(player);
			this.actions = actions;
			// Set this duration to be the longest subaction duration
			foreach (PlayerAction action in actions) {
				if (duration < action.duration)
					duration = action.duration;
			}
		}

		protected override void DoAction()
		{
			foreach (PlayerAction action in actions)
			{
				action.Execute();
			}		
		}
	}
}
