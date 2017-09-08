using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroPowerUpInfoIcon : MonoBehaviour
{
	public Image icon;
	private HeroPowerUpData data;
	private ScrollingTextOption scrollingTextOption;

	void Awake()
	{
		scrollingTextOption = GetComponent<ScrollingTextOption>();
	}

	public void Init(HeroPowerUpData data, bool locked, int unlockLevel = 0)
	{
		this.data = data;
		icon.sprite = data.icon;
		if (locked)
		{
			icon.color = Color.gray;
			scrollingTextOption.text = string.Format("LOCKED: Unlocked at level {0}", unlockLevel);
		}
		else
		{
			icon.color = Color.white;
			scrollingTextOption.text = data.description;
		}
	}
}
