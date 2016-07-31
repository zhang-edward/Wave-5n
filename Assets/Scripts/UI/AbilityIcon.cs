﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIcon : MonoBehaviour
{
	public Image image;
	public RectTransform cooldownMask;

	public void SetCoolDown(float percent)
	{
		cooldownMask.sizeDelta = new Vector2 (16, percent * 16);
	}
}

