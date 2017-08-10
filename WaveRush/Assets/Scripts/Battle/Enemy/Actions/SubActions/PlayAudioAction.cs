using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class PlayAudioAction : EnemyAction
	{
		public AudioClip[] clips;
		public bool randomizePitch;
		private SoundManager sound;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			sound = SoundManager.instance;
		}

		public override void Execute()
		{
			base.Execute();
			if (randomizePitch)
				sound.RandomizeSFX(clips[Random.Range(0, clips.Length)]);
			else
				sound.PlaySingle(clips[Random.Range(0, clips.Length)]);
		}

		public override void Interrupt()
		{
		}
	}
}

