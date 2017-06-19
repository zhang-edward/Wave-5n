/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpgradeIcon : MonoBehaviour {

	private UpgradeScreen upgradeScreen;

	// color of the price text
	public static Color32 priceTextColor = new Color32(239, 239, 123, 255);
	public static Color32 unlockedTextColor = new Color32(163, 206, 39, 255);
	public Text priceText;
	public Image icon;

	public HeroPowerUpData data;
	public bool unlocked;

	private Toggle toggle;

	void Awake()
	{
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(NotifyUpgradeScreenValueChange);
	}

	private void NotifyUpgradeScreenValueChange(bool value)
	{
		upgradeScreen.OnTogglesValueChanged();
	}

	public void Init(UpgradeScreen upgradeScreen, HeroPowerUpData data, bool unlocked)
	{
		this.upgradeScreen = upgradeScreen;
		this.data = data;
		this.unlocked = unlocked;

		icon.sprite = data.icon;
		if (!unlocked)
		{
			priceText.text = data.cost.ToString();
			priceText.color = priceTextColor;
		}
		else
		{
			priceText.text = "UNLOCKED!";
			priceText.color = unlockedTextColor;
		}
	}
}
*/