using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SimpleAnimation {

	public Sprite[] frames;
	public int fps = 10;
	public float SecondsPerFrame {
		get {
			return 1.0f / (float)fps;
		}
	}
	public float TimeLength {
		get {
			return frames.Length * (float)SecondsPerFrame;
		}
	}
}