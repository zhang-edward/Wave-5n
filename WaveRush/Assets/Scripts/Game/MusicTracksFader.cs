using UnityEngine;
using System.Collections;

public class MusicTracksFader : MonoBehaviour
{
	public AudioSource[] musicSrc;

	public void StartFadeMusic(int index)
	{
//		print("switching tracks to " + index);
		StopAllCoroutines();
		for (int i = 0; i < musicSrc.Length; i++)
		{
			if (i == index)
			{
				StartCoroutine(FadeMusicRoutine(SoundManager.instance.musicVolume, i));
			}
			else
			{
				StartCoroutine(FadeMusicRoutine(0f, i));
			}
		}
	}

	private IEnumerator FadeMusicRoutine(float targetVolume, int audioSrcIndex)
	{
		AudioSource src = musicSrc[audioSrcIndex];
//		print("setting " + src + " to volume " + targetVolume);
		float initialVolume = src.volume;
		float finalVolume = targetVolume;
		float t = 0;
		while (Mathf.Abs(src.volume - targetVolume) > 0.05f)
		{
			src.volume = Mathf.Lerp(initialVolume, finalVolume, t);
			t += Time.deltaTime;
			yield return null;
		}
		src.volume = targetVolume;
	}
}
