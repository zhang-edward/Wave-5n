using UnityEngine;

public class EnemyAnimationEventAudio : MonoBehaviour
{
	public AudioClip[] clips;

	public void PlayAudio(int index)
	{
		SoundManager.instance.PlaySingle(clips[index]);
	}
}