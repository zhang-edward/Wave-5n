using UnityEngine;
using System.Collections;

public class SortingLayerControl : MonoBehaviour {

	public bool updating = false;
	public bool isPivotInMiddle = true;
	SpriteRenderer sr;

	// Use this for initialization
	void Awake () {
		sr = GetComponent<SpriteRenderer> ();
	}

	void Start() 
	{
		if (updating)
			StartCoroutine (SetSortingOrderRepeating());
		else
			SetSortingOrder ();
	}

	private IEnumerator SetSortingOrderRepeating()
	{
		for (;;)
		{
			SetSortingOrder ();
			yield return null;
		}
	}

	private void SetSortingOrder()
	{
		float offset = 0;
		if (isPivotInMiddle)
			offset = sr.bounds.size.y / 2;
		sr.sortingOrder = (int)((transform.position.y - offset) * -100);
	}
}
