using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroPowerUpInfoIcon : MonoBehaviour
{
	public Image icon;
	public NewFeatureIndicator newPowerUp;
	private HeroPowerUpData data;
	private ScrollingTextOption scrollingTextOption;
	private Toggle toggle;
	private string key;

	void Awake()
	{
		scrollingTextOption = GetComponent<ScrollingTextOption>();
		toggle = GetComponent<Toggle>();
	}

	public void Init(HeroPowerUpData data, bool locked, string key, int unlockLevel = 0)
	{
		// Init variables
		this.key = key;
		this.data = data;
		icon.sprite = data.icon;

		if (locked)
		{
			newPowerUp.gameObject.SetActive(false);
			icon.color = Color.gray;
			scrollingTextOption.text = string.Format("LOCKED: Unlocked at level {0}", unlockLevel);
		}
		else
		{
			newPowerUp.RegisterKey(key);	// Only indicate that the powerup is new if it has been unlocked
			icon.color = Color.white;
			scrollingTextOption.text = data.description;
			toggle.onValueChanged.AddListener(SetViewedPower);
		}
	}

	public void SetViewedPower(bool foo)
	{
		// GameManager.instance.save.SetHasPlayerViewedKey(key, true);
	}
}
