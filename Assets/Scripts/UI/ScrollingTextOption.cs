using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollingTextOption : MonoBehaviour {

	public ScrollingText scrollingText;
	public string text;

	public void UpdateScrollingText(bool isOn)
	{
		if (isOn)
			scrollingText.UpdateText (text);
		else
			scrollingText.SetToDefaultText ();
	}
}
