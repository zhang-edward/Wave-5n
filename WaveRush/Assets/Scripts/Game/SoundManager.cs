using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	AudioSource ui;
	AudioSource music;
	AudioSource sfx;
	private List<AudioSource> sfxSources = new List<AudioSource>();
	private List<AudioSource> musicSources = new List<AudioSource>();

	private float lowPitchRange = 0.95f;
	private float highPitchRange = 1.05f;

	[Range(0, 1)]
	public float musicVolume = 1;
	[Range(0, 1)]
	public float sfxVolume = 1;

	public bool playingMusic { get; private set; }

	void Awake()
	{
		// make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
		
		ui = GetComponent<AudioSource> ();
		music = GameObject.Find ("Music").GetComponent<AudioSource> ();
		sfx = GameObject.Find ("SFX").GetComponent<AudioSource> ();

		music.volume = musicVolume;
	}

	public void RegisterSfxSrc(AudioSource src)
	{
		sfxSources.Add(src);
	}

	public void UnregisterSfxSrc(AudioSource src)
	{
		sfxSources.Remove(src);
	}

	public void RegisterMusicSrc(AudioSource src)
	{
		musicSources.Add(src);
	}

	public void UnregisterMusicSrc(AudioSource src)
	{
		musicSources.Remove(src);
	}

	/// <summary>
	/// Plays the AudioClip with some pitch variance.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="stacking">whether or not this clip can be stacked with others (bad for many sounds played at the same time)</param>
	public void RandomizeSFX(AudioClip clip)
	{
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);
		sfx.pitch = randomPitch;
		sfx.PlayOneShot (clip);
	}

	public void PlayInterrupt(AudioClip clip)
	{
		sfx.clip = clip;
		sfx.Play ();
	}

	public void PlaySingle(AudioClip clip)
	{
		sfx.pitch = 1.0f;
		sfx.PlayOneShot(clip);
	}

	/// <summary>
	/// Plays a sound and also lowers the volume of the background music while the clip is playing
	/// </summary>
	/// <param name="clip">Clip.</param>
	public void PlayImportantSound(AudioClip clip)
	{
		sfx.clip = clip;
		StartCoroutine (ImportantSound ());
	}

	public void PlayUISound(AudioClip clip)
	{
		ui.clip = clip;
		ui.Play ();
	}

	public void PlayMusicLoop(AudioClip clip, bool fadeIn = false, AudioClip intro = null)
	{
//		Debug.Log ("Playing new music loop: " + clip);
		if (fadeIn) {
			StopAllCoroutines();
			music.volume = 0;
			StartCoroutine(FadeMusicRoutine(1));
		}
		playingMusic = true;
		StartCoroutine (MusicLoop (clip, intro));
	}

	private IEnumerator MusicLoop(AudioClip clip, AudioClip intro = null)
	{
		// resets any previous music clip
		music.Stop ();
		// play the intro
		if (intro != null)
		{
			music.loop = false;
			music.clip = intro;
			music.Play ();
		}
		while (music.isPlaying)
			yield return null;
		music.loop = true;
		music.clip = clip;
		music.Play ();
	}

	public void PauseMusic()
	{
		music.Pause();
	}

	public void UnPauseMusic()
	{
		music.UnPause();
	}

	public void FadeMusic(float targetVolume) {
		StopAllCoroutines();
		StartCoroutine(FadeMusicRoutine(targetVolume));
	}

	private IEnumerator ImportantSound()
	{
		StartCoroutine(FadeMusicRoutine (0.5f));
		sfx.Play ();
		yield return new WaitForSeconds (sfx.clip.length + 1);
		StartCoroutine (FadeMusicRoutine (1));
	}

	private IEnumerator FadeMusicRoutine(float targetVolume)
	{
		float initialVolume = music.volume;
		float finalVolume = targetVolume * musicVolume;
		float t = 0;
		while (Mathf.Abs(music.volume - targetVolume) > 0.05f)
		{
			music.volume = Mathf.Lerp(initialVolume, finalVolume, t);
			t += Time.deltaTime;
			yield return null;
		}
		music.volume = targetVolume;
	}

	void Update()
	{
		music.volume = musicVolume;
		sfx.volume = sfxVolume;
	}

	public void SetMusicVolume(float volume)
	{
		musicVolume = volume;
	}

	public void SetSfxVolume(float volume)
	{
		sfxVolume = volume;
	}
}
