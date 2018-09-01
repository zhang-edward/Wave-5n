using UnityEngine;
using System.Collections;

public class MusicTracksFader : MonoBehaviour
{
	public AudioSource[] musicSrc;

	void Awake() {
		foreach (AudioSource src in musicSrc) {
			SoundManager.instance.RegisterMusicSrc(src);
		}
	}

	public void StartFadeMusic(int index)
	{
//		print("switching tracks to " + index);
		StopAllCoroutines();
		for (int i = 0; i < musicSrc.Length; i++)
		{
			if (i == index)
			{
				StartCoroutine(FadeMusicRoutine(i));
			}
			else
			{
				StartCoroutine(FadeMusicRoutine(0f, i));
			}
		}
	}

	private IEnumerator FadeMusicRoutine(int audioSrcIndex)
	{
		SoundManager sound = SoundManager.instance;
		AudioSource src = musicSrc[audioSrcIndex];
//		print("setting " + src + " to volume " + targetVolume);
		float initialVolume = src.volume;
		float t = 0;
		for (;;)
		{
			while (Mathf.Abs(src.volume - sound.musicVolume) > 0.05f)
			{
				src.volume = Mathf.Lerp(initialVolume, sound.musicVolume, t);
				t += Time.deltaTime;
				yield return null;
			}
			src.volume = sound.musicVolume;
			yield return null;
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
