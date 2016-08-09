using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollViewSnapContent : MonoBehaviour {

	public ScrollViewSnap scrollView;
	public int index;

	public string heroName;
	private Button button;

	void Awake()
	{
		button = GetComponent<Button> ();
	}

	void Start()
	{
		button.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		scrollView.SetSelectedContentIndex (index);
	}
}
