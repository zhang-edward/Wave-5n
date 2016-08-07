using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIcon : MonoBehaviour
{
	public Image image;
	public RectTransform cooldownMask;

	public void SetCooldown(float percent)
	{
		if (percent < 0)
			return;
		cooldownMask.sizeDelta = new Vector2 (16, percent * 16);
	}
}

