using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedLineSegment : MonoBehaviour {

	public SimpleAnimation[] animations;
	public SimpleAnimationPlayer anim;

	void OnEnable()
	{
		transform.localScale = Vector3.one * Random.Range (1f, 1.2f);
		anim.anim = animations [Random.Range (0, animations.Length)];
		anim.anim.fps = anim.anim.fps + Random.Range(-anim.anim.fps / 2f, anim.anim.fps / 2f);
		anim.destroyOnFinish = true;
		anim.Play ();
	}
}
