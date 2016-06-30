using UnityEngine;
using System.Collections;

public class SortingLayerControl : MonoBehaviour {

	SpriteRenderer sr;

	// Use this for initialization
	void Awake () {
		sr = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		sr.sortingOrder = (int)(transform.position.y * -100);
	}
}
