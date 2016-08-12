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
	//private bool dragging = false;	// whether the user is dragging the scrollview
	public int selectedContentIndex { get; private set; }	// index of the content to snap to

	public delegate void EndedDrag ();
	public event EndedDrag OnEndDrag;

	void Start()
	{
		distances = new float[content.Length];
		EvaluateDistances ();
		InitContent ();

		/*contentDistance = Mathf.Abs (
			content [1].GetComponent<RectTransform> ().anchoredPosition.x -
			content [0].GetComponent<RectTransform> ().anchoredPosition.x);*/
	}

	private void InitContent()
	{
		for (int i = 0; i < content.Length; i ++)
		{
			GameObject contentItem = content [i];
			ScrollViewSnapContent scrollViewContent = contentItem.GetComponent<ScrollViewSnapContent> ();
			if (scrollViewContent != null)
			{
				scrollViewContent.index = i;
			}
		}
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
	}

	private void StartLerpToContent()
	{
		StopAllCoroutines ();
		StartCoroutine (LerpToContent ());
	}

	private IEnumerator LerpToContent()
	{
		float dest = selectedContentIndex * -contentDistance;
		while (Mathf.Abs(panel.anchoredPosition.x - dest) > 0.05f)
		{
			float newX = Mathf.Lerp (panel.anchoredPosition.x, dest, Time.deltaTime * 20f);
			Vector2 newPosition = new Vector2 (newX, panel.anchoredPosition.y);
			panel.anchoredPosition = newPosition;
			yield return null;
		}
	}

	private void EvaluateDistances()
	{
		for (int i = 0; i < content.Length; i ++)
		{
			distances [i] = Mathf.Abs (center.transform.position.x - content [i].transform.position.x);
//			Debug.Log (content [i].transform.position.x);
		}
	}

	public void EndDrag()
	{
		if (OnEndDrag != null)
			OnEndDrag();
		StartLerpToContent ();
	}

	public void SetSelectedContentIndex(int index)
	{
		selectedContentIndex = index;
		EndDrag ();
	}
}
