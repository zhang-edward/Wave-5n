namespace PlayerActions
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	[System.Serializable]
	public class PA_EffectCallback : PA_Effect
	{
		public delegate void OnFrameReached(int frame);
		public event OnFrameReached onFrameReached;

		private List<int> listeners = new List<int>();					// List of the frame numbers to listen for

		public void Init(Player player, params int[] listeners)
		{
			base.Init(player);
			this.listeners.AddRange(listeners);
		}

		public void AddListener(int frame)
		{
			listeners.Add(frame);
		}

		protected override void DoAction()
		{
			base.DoAction();
			// For each frame to listen for, start a coroutine which will invoke the callback when that frame is played
			foreach (int frame in listeners)
				player.StartCoroutine(FrameListenerRoutine(frame));
		}

		private IEnumerator FrameListenerRoutine(int frame)
		{
			yield return new WaitForSeconds(effect.SecondsPerFrame * frame);
			if (onFrameReached != null)
				onFrameReached(frame);
		}
	}
}
