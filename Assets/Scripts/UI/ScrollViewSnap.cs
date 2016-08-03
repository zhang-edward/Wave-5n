using UnityEngine;
using System.Collections;

public class ScrollViewSnap : MonoBehaviour {

	public RectTransform panel;		// the panel that holds the content
	public GameObject[] content;
	public RectTransform center;	// the center of the panel
	public float contentDistance;	// holds the distance between the content

	public GameObject SelectedContent {
		get {return content [selectedContentIndex];}
	}

	private float[] distances;		// the distance of each content item to the center
	private bool dragging = false;	// whether the user is dragging the scrollview
	public int selectedContentIndex { get; private set; }	// index of the content to snap to

	public delegate void EndedDrag ();
	public event EndedDrag OnEndDrag;

	void Start()
	{
		distances = new float[content.Length];
		EvaluateDistances ();
		/*contentDistance = Mathf.Abs (
			content [1].GetComponent<RectTransform> ().anchoredPosition.x -
			content [0].GetComponent<RectTransform> ().anchoredPosition.x);*/
	}

	void Update()
	{
		EvaluateDistances ();
		float minDistance = int.MaxValue;
		for (int i = 0; i < content.Length; i ++)
		{
			if (distances[i] < minDistance)
			{
				minDistance = distances [i];
				selectedContentIndex = i;
			}
		}
//		Debug.Log("SELECTED CONTENT INDEX: " + selectedContentIndex);

		if (!dragging)
		{
			LerpToContent (selectedContentIndex * -contentDistance);
		}
	}

	private void LerpToContent(float position)
	{
		float newX = Mathf.Lerp (panel.anchoredPosition.x, position, Time.deltaTime * 20f);
		Vector2 newPosition = new Vector2 (newX, panel.anchoredPosition.y);
		panel.anchoredPosition = newPosition;
	}

	private void EvaluateDistances()
	{
		for (int i = 0; i < content.Length; i ++)
		{
			distances [i] = Mathf.Abs (center.transform.position.x - content [i].transform.position.x);
//			Debug.Log (content [i].transform.position.x);
		}
	}

	public void StartDrag()
	{
		dragging = true;
	}

	public void EndDrag()
	{
		dragging = false;
		if (OnEndDrag != null)
			OnEndDrag();
	}
}
