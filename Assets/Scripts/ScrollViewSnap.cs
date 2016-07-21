using UnityEngine;
using System.Collections;

public class ScrollViewSnap : MonoBehaviour {

	public RectTransform panel;		// the panel that holds the content
	public GameObject[] content;
	public RectTransform center;	// the center of the panel
	public float contentDistance;	// holds the distance between the content


	private float[] distances;		// the distance of each content item to the center
	private bool dragging = false;	// whether the user is dragging the scrollview
	public int selectedContentIndex { get; private set; }	// index of the content to snap to

	void Start()
	{
		distances = new float[content.Length];
		//contentDistance = Mathf.Abs (
		//	content [1].GetComponent<RectTransform> ().anchoredPosition.x -
		//	content [0].GetComponent<RectTransform> ().anchoredPosition.x);
	}

	void Update()
	{
		for (int i = 0; i < content.Length; i ++)
		{
			distances [i] = Mathf.Abs (center.transform.position.x - content [i].transform.position.x);
		}

		float minDistance = Mathf.Min (distances);
		for (int i = 0; i < content.Length; i ++)
		{
			if (minDistance == distances[i])
			{
				selectedContentIndex = i;
			}
		}

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

	public void StartDrag()
	{
		dragging = true;
	}

	public void EndDrag()
	{
		dragging = false;
	}
}
