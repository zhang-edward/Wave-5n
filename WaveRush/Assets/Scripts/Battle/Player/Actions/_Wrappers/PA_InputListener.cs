namespace PlayerActions
{
	using System.Collections;
	using UnityEngine;

	[System.Serializable]
	public class PA_InputListener : PlayerAction
	{
		private Coroutine executeRoutine;

		public enum InputType 
		{
			Drag,
			Tap,
			TapHold,
		}
		public PlayerAction action;
		public InputType input;			// What type of input to listen for

		public void Init(Player player, PlayerAction action)
		{
			base.Init(player);
			this.action = action;
		}

		protected override void DoAction()
		{
			if (executeRoutine != null)
				player.StopCoroutine(executeRoutine);
			executeRoutine = player.StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine()
		{
			switch (input)
			{
				case InputType.Drag:
					hero.onDragRelease += TryExecuteSubAction;
					break;
				case InputType.Tap:
					hero.onTap += TryExecuteSubAction;
					break;
				case InputType.TapHold:
					hero.onTapHoldDown += TryExecuteSubAction;
					break;
			}
			yield return new WaitForSecondsRealtime(duration);
			switch (input)
			{
				case InputType.Drag:
					hero.onDragRelease -= TryExecuteSubAction;
					break;
				case InputType.Tap:
					hero.onTap -= TryExecuteSubAction;
					break;
				case InputType.TapHold:
					hero.onTapHoldDown -= TryExecuteSubAction;
					break;
			}
		}

		private void TryExecuteSubAction()
		{
			//Debug.Log("Input Listener executing sub action: " + action);
			action.Execute();
		}
	}
}
