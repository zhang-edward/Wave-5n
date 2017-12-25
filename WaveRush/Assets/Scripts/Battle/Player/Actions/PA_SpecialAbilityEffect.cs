namespace PlayerActions
{
	using UnityEngine;
	using System.Collections;

	[System.Serializable]
	public class PA_SpecialAbilityEffect : PlayerAction
	{
		public PlayerAction disableAction;		// Action which turns this effect off 
												// (usually the execution of the next step)

		public void Init(Player player, PlayerAction disableAction)
		{
			base.Init(player);
			this.disableAction = disableAction;
			disableAction.OnExecutedAction += DisableEffect;
		}

		protected override void DoAction()
		{
			player.StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine()
		{
			Time.timeScale = 0.2f;
			CameraControl.instance.SetOverlayColor(Color.black, 0.4f, 1.0f);
			CameraControl.instance.screenOverlay.sortingLayerName = "TerrainObjects";
			if (duration > 0) {
				yield return new WaitForSecondsRealtime(duration);
				DisableEffect();
			}
		}

		void DisableEffect()
		{
			Time.timeScale = 1f;
			CameraControl.instance.DisableOverlay(1f);
			CameraControl.instance.screenOverlay.sortingLayerName = "Default";
		}
	}
}
