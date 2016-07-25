using UnityEngine;
using System.Collections;

public class SortingLayerControl : MonoBehaviour {

	public bool updating = false;
	SpriteRenderer sr;

	// Use this for initialization
	void Awake () {
		sr = GetComponent<SpriteRenderer> ();
	}

	void Start() 
	{
		if (updating)
			StartCoroutine ("SetSortingOrder");
		else
			sr.sortingOrder = (int)(transform.position.y * -100);
	}

	private IEnumerator SetSortingOrder()
	{
		while (true)
		{
			sr.sortingOrder = (int)(transform.position.y * -100);
			yield return null;
		}
	}
}
