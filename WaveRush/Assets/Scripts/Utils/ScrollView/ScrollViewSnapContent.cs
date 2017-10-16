using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ScrollViewSnapContent : MonoBehaviour {

	private ScrollViewSnap scrollView;
	public int index { get; set; }

	private Button button;

	protected virtual void Awake()
	{
		button = GetComponent<Button> ();
	}

	public void SetScrollView(ScrollViewSnap scrollView)
	{
		this.scrollView = scrollView;
		button.onClick.AddListener(() =>
		{
			OnClick();
		});
	}

	protected virtual void OnClick()
	{
		scrollView.SetSelectedContentIndex (index);
	}
}
