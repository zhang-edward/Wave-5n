using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ScrollViewSnapContent : MonoBehaviour {

	public ScrollViewSnap scrollView;
	public int index { get; set; }

	private Button button;

	protected virtual void Awake()
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
