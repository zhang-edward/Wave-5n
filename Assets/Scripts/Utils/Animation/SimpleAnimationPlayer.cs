using UnityEngine;
using System.Collections;

public class SimpleAnimationPlayer : MonoBehaviour {

	public SpriteRenderer sr;
	public SimpleAnimation anim;

	private int frameIndex;

	public bool isPlaying;
	public bool looping;

	public void Start()
	{
		//sr = GetComponent<SpriteRenderer>();
		//secondsPerFrame = 1.0f / anim.fps;
		frameIndex = 0;
		//sr.sprite = anim.frames [0];
	}

	public void Play()
	{
		UnityEngine.Assertions.Assert.IsNotNull (anim);
		Reset ();
		StartCoroutine("PlayAnim");
	}

	public void Reset()
	{
		frameIndex = 0;
		sr.sprite = anim.frames[0];
	}

	private IEnumerator PlayAnim()
	{
		isPlaying = true;
		while (frameIndex < anim.frames.Length)
		{
			sr.sprite = anim.frames[frameIndex];
			frameIndex++;
			// if looping, set animation to beginning and do not stop playing
			if (looping)
			{
				if (frameIndex >= anim.frames.Length)
					frameIndex = 0;
			}
			yield return new WaitForSeconds(anim.SecondsPerFrame);
		}
		isPlaying = false;
		yield return null;
	}
}