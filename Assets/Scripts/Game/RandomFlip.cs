using UnityEngine;
using System.Collections;

public class RandomFlip : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		// randomly flips this sprite horizontally on creation
		GetComponent<SpriteRenderer> ().flipX = Random.value < 0.5f;
	}
}
