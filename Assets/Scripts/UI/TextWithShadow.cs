using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextWithShadow : MonoBehaviour {

	public GameObject textShadowPrefab;
	private Text text;
	private Text shadow;

	public Color textColor;
	public Color shadowColor;

	void Awake()
	{
		GameObject o = Instantiate (textShadowPrefab);
		o.transform.SetParent (this.transform, false);
		o.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 2, 0);
		// text is the child object because children are rendered in front of parents
		text = o.GetComponent<Text> ();
		shadow = this.GetComponent<Text> ();
		// copy components
		text.font = shadow.font;
		text.fontSize = shadow.fontSize;
		text.fontStyle = shadow.fontStyle;
		text.alignment = shadow.alignment;
		// set Colors
		text.color = textColor;
		shadow.color = shadowColor;
	}

	void LateUpdate()
	{
		text.text = shadow.text;	// remember that 'text' is the child component and "shadow" is this component
		text.color = new Color(text.color.r, text.color.g, text.color.b, shadow.color.a);
	}
}
