using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {

	public SimpleAnimationPlayer animPlayer;

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GetComponent<SpriteRenderer> ().sprite = animPlayer.anim.frames [animPlayer.anim.frames.Length - 1];
			animPlayer.StopAllCoroutines ();
		}
	}
}
