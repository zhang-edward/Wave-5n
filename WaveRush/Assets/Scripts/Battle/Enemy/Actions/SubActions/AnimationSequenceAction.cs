using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class AnimationSequenceAction : EnemyAction
	{
		private AnimationSet anim;
		private int curState;

		[System.Serializable]
		public class AnimationState
		{
			public string name;
			public float duration;
			public AudioClip clip;
		}

		public AnimationState[] states;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			anim = e.anim;
		}

		public override void Execute()
		{
			base.Execute();
			StartCoroutine(UpdateState());
		}

		private IEnumerator UpdateState()
		{
			while (curState < states.Length)
			{
				AnimationState state = states[curState];            // get the current state
				anim.Play(state.name);                    			// transition to the animation state
				SoundManager.instance.RandomizeSFX(state.clip);
				yield return new WaitForSeconds(state.duration);    // hold the state for a specific duration
				curState++;                                         // move to the next state
			}
			if (onActionFinished != null)
				onActionFinished();
			curState = 0;
		}

		public override void Interrupt()
		{
			StopAllCoroutines();
		}
	}
}