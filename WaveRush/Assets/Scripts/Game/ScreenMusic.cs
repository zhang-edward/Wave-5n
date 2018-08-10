﻿using UnityEngine;
using System.Collections;

public class ScreenMusic : MonoBehaviour {

	public AudioClip intro;
	public AudioClip musicLoop;

	void Start()
	{
		SoundManager.instance.PlayMusicLoop (musicLoop, false, intro);
	}

	private void OnDisable()
	{
		SoundManager.instance.PauseMusic();
	}
}
