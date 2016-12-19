using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltPrefab : MonoBehaviour {

	public SimpleAnimation[] animations;
	public SimpleAnimationPlayer anim;

	void OnEnable()
	{
		transform.localScale = Vector3.one * Random.Range (1f, 1.2f);
		anim.anim = animations [Random.Range (0, animations.Length)];
		anim.anim.fps = Random.Range (15, 30);
		anim.destroyOnFinish = true;
		anim.Play ();
	}
}
