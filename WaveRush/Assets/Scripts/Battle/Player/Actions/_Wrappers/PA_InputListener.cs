namespace PlayerActions
{
	using System.Collections;
	using UnityEngine;

	[System.Serializable]
	public class PA_InputListener : PlayerAction
	{
		private Coroutine executeRoutine;

		public enum InputType {
			Drag,
			Tap,
			TapHold,
		}
		public PlayerHero.DirectionalInputAction Callback;
		public InputType input;			// What type of input to listen for

		public void Init(Player player, PlayerHero.DirectionalInputAction Callback) {
			base.Init(player);
			this.Callback = Callback;
		}

		protected override void DoAction()
		{
			if (executeRoutine != null)
				player.StopCoroutine(executeRoutine);
			executeRoutine = player.StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine()
		{
			EnableListener();
			yield return new WaitForSecondsRealtime(duration);
			DisableListener();
		}

		private void OnInputDetected(Vector3 dir) {
			Callback(dir);
			DisableListener();
			player.StopCoroutine(executeRoutine);
		}

		private void EnableListener() {
			switch (input) {
				case InputType.Drag:
					hero.onDragRelease += OnInputDetected;
					break;
				case InputType.Tap:
					hero.onTap += OnInputDetected;
					break;
				case InputType.TapHold:
					hero.onTapHoldDown += OnInputDetected;
					break;
			}
		}

		private void DisableListener() {
			switch (input) {
				case InputType.Drag:
					hero.onDragRelease -= OnInputDetected;
					break;
				case InputType.Tap:
					hero.onTap -= OnInputDetected;
					break;
				case InputType.TapHold:
					hero.onTapHoldDown -= OnInputDetected;
					break;
			}
		}
	}
}
