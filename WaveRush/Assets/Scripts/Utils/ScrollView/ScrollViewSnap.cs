using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollViewSnap : MonoBehaviour {

	public RectTransform panel;		// the panel that holds the content
	public List<GameObject> content;
	public RectTransform center;	// the center of the panel
	public float contentDistance;   // holds the distance between the content
	public float scrollSpeed = 20f;	// how fast the snapping occurs
	public bool initOnAwake = true;	// set to true if the content is set in the scene, set to false if content is generated via code
									// WARNING: if set to false, the Init function must be called manually!!

	public GameObject SelectedContent {
		get {return content [selectedContentIndex];}
	}

	private float[] distances;		// the distance of each content item to the center
	//private bool dragging = false;	// whether the user is dragging the scrollview
	public int selectedContentIndex { get; private set; }	// index of the content to snap to

	public delegate void EndedDrag ();
	public event EndedDrag OnEndDrag;

	void Awake()
	{
		if (initOnAwake)
			Init();	
	}

	public void Init()
	{
		distances = new float[content.Count];
		InitContent ();
		EvaluateDistances ();
		if (content.Count > 1)
		{
			contentDistance = Mathf.Abs(
				content[1].GetComponent<RectTransform>().anchoredPosition.x -
				content[0].GetComponent<RectTransform>().anchoredPosition.x);
		}
		StartCoroutine(UpdateScrollView());
	}

	/// <summary>
	/// Initialize the content in the scrollView
	/// </summary>
	protected virtual void InitContent()
	{
		for (int i = 0; i < content.Count; i ++)
		{
			GameObject contentItem = content [i];
			ScrollViewSnapContent scrollViewContent = contentItem.GetComponent<ScrollViewSnapContent> ();
			// make sure that scrollViewContent is a component on each content item
			// UnityEngine.Assertions.Assert.IsNotNull (scrollViewContent);
			if (scrollViewContent != null)
			{
				scrollViewContent.SetScrollView(this);
				scrollViewContent.index = i;
			}
		}
	}

	IEnumerator UpdateScrollView()
	{
		for (;;)
		{
			EvaluateDistances();
			float minDistance = int.MaxValue;
			for (int i = 0; i < content.Count; i++)
			{
				if (distances[i] < minDistance)
				{
					minDistance = distances[i];
					selectedContentIndex = i;
				}
			}
			yield return null;
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
			// set new position
			float newX = Mathf.Lerp (panel.anchoredPosition.x, dest, Time.deltaTime * scrollSpeed);
			panel.anchoredPosition = new Vector2 (newX, panel.anchoredPosition.y);
			yield return null;
		}
	}

	private void EvaluateDistances()
	{
		for (int i = 0; i < content.Count; i ++)
		{
			distances [i] = Mathf.Abs (center.transform.position.x - content[i].transform.position.x);
			// Debug.Log (i + ":" + distances[i]);
		}
	}

	public virtual void EndDrag()
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

	public void ScrollRight()
	{
		selectedContentIndex++;
		if (selectedContentIndex >= content.Count)
			selectedContentIndex = content.Count - 1;
		EndDrag();
	}

	public void ScrollLeft()
	{
		selectedContentIndex--;
		if (selectedContentIndex <= 0)
			selectedContentIndex = 0;
		EndDrag();
	}
}
