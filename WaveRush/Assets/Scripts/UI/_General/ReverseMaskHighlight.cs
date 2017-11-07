using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReverseMaskHighlight : MonoBehaviour
{
	public RectTransform mask;

	public void Highlight(Vector3 position, Vector2 sizeDelta)
	{
		gameObject.SetActive(true);
		mask.position = position;
		mask.sizeDelta = sizeDelta;
	}
}
