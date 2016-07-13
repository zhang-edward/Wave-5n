using UnityEngine;
using System.Collections;

public class TerrainObject : MonoBehaviour {

	public bool flippable;

	// Use this for initialization
	void Awake () {
		// randomly flips this sprite horizontally on creation
		if (flippable)
			GetComponent<SpriteRenderer> ().flipX = Random.value < 0.5f;
	}
}
