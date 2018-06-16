using UnityEngine;

public class AnimationEventAudio : MonoBehaviour
{
	public AudioClip[] clips;

	public void PlayAudio(int index)
	{
		SoundManager.instance.PlaySingle(clips[index]);
	}
}