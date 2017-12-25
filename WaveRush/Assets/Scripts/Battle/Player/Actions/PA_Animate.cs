namespace PlayerActions
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[System.Serializable]
	public class PA_Animate : PlayerAction
	{
		public string state;

		protected override void DoAction()
		{
			player.StartCoroutine(ExecuteRoutine());
		}

		private IEnumerator ExecuteRoutine()
		{
			hero.anim.Play(state);
			player.animPlayer.willResetToDefault = false;

			if (duration > 0)
			{
				yield return new WaitForSeconds(duration);
				hero.anim.Play("Default");
			}
		}
	}
}
