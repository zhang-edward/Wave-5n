using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	AudioSource ui;
	AudioSource music;
	AudioSource sfx;

	private float lowPitchRange = 0.95f;
	private float highPitchRange = 1.05f;

	public float musicVolume;

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

	public void PlayMusicLoop(AudioClip clip, AudioClip intro = null)
	{
//		Debug.Log ("Playing new music loop: " + clip);
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

	private IEnumerator ImportantSound()
	{
		StartCoroutine(MusicFadeOut (0.5f));
		sfx.Play ();
		yield return new WaitForSeconds (sfx.clip.length + 1);
		StartCoroutine (MusicFadeIn (musicVolume));
	}

	private IEnumerator MusicFadeIn(float targetVolume)
	{
		while (music.volume < targetVolume)
		{
			music.volume += 0.05f;
			yield return null;
		}
	}

	private IEnumerator MusicFadeOut(float targetVolume)
	{
		while (music.volume > targetVolume)
		{
			music.volume -= 0.05f;
			yield return null;
		}
	}
}
