using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	AudioSource ui;
	AudioSource music;
	AudioSource sfx;

	private float lowPitchRange = 0.95f;
	private float highPitchRange = 1.05f;

	void Awake()
	{
		// make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this);

		ui = GetComponent<AudioSource> ();
		music = GameObject.Find ("Music").GetComponent<AudioSource> ();
		sfx = GameObject.Find ("SFX").GetComponent<AudioSource> ();
	}

	public void RandomizeSFX(AudioClip clip)
	{
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);
		sfx.pitch = randomPitch;
		sfx.PlayOneShot(clip);
	}

	public void PlaySingle(AudioClip clip)
	{
		sfx.clip = clip;
		sfx.Play ();
	}

	public void PlayUISound(AudioClip clip)
	{
		ui.clip = clip;
		ui.Play ();
	}
}
