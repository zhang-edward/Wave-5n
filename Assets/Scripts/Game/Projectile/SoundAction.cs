using UnityEngine;
namespace Projectiles
{
	public class SoundAction : ProjectileAction
	{
		public AudioClip[] clips;

		public override void Execute()
		{
			SoundManager.instance.RandomizeSFX(clips[Random.Range(0, clips.Length)]);
		}
	}
}
