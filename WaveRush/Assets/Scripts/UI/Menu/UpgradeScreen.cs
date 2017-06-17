﻿/*using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class UpgradeScreen : MonoBehaviour
{
	private HeroType selectedHero;				// the selected hero to disply info for
	public GameObject upgradeIconPrefab;        // the icons for each power up

	[Header("In Prefab")]
	public Transform contentPanel;              // the folder in which upgradeIcons are placed
	public ToggleGroup toggleGroup;
	private List<UpgradeIcon> upgradeIcons = new List<UpgradeIcon>();

	public ScrollingText descriptionText;
	public Button purchaseButton;

	[Header("Audio")]
	public AudioClip purchaseSound;
	public AudioClip notEnoughMoneySound;

	public void Init(HeroType heroName)
	{
		this.selectedHero = heroName;
		// reset scroll view to original position
		contentPanel.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		// reset scrolling text
		descriptionText.SetToDefaultText();
		// turn off selected item
		UpgradeIcon selected = GetSelected();
		if (selected != null)
			selected.GetComponent<Toggle>().isOn = false;
		
		RefreshScrollView(DataManager.GetPowerUpListData(heroName));
	}

	private void RefreshScrollView(HeroPowerUpListData data)
	{
		print("RefreshScrollView:" + selectedHero);
		// if there are not enough icons for each powerup, instantiate more
		if (data.powerUps.Length > upgradeIcons.Count)
		{
			int numToInstantiate = data.powerUps.Length - upgradeIcons.Count;
			for (int i = 0; i < numToInstantiate; i ++)
			{
				GameObject o = Instantiate(upgradeIconPrefab);
				o.transform.SetParent(contentPanel, false);
				upgradeIcons.Add(o.GetComponent<UpgradeIcon>());
				o.GetComponent<Toggle>().group = toggleGroup;
			}
		}

		// reset each icon
		foreach (UpgradeIcon icon in upgradeIcons)
			icon.gameObject.SetActive(false);

		// init each icon
		for (int i = 0; i < data.powerUps.Length; i ++)
		{
			// if 1 powerup unlocked, i = 0 should be unlocked
			// if 2 powerups unlocked, i = 0, 1 should be unlocked, etc.
			int powerUpsUnlocked = GameManager.instance.saveGame.GetHeroData(selectedHero).numPowerUpsUnlocked;
			bool powerUpUnlocked = i < powerUpsUnlocked;
			HeroPowerUp powerUp = data.powerUps[i];

			upgradeIcons[i].Init(this, powerUp.data, powerUpUnlocked);
			if (i <= powerUpsUnlocked)
				upgradeIcons[i].gameObject.SetActive(true);
		}
	}

	private UpgradeIcon GetSelected()
	{
		foreach (Toggle toggle in toggleGroup.ActiveToggles())
		{
			if (toggle.isOn)
				return toggle.GetComponent<UpgradeIcon>();
		}
		return null;
	}

	public void OnTogglesValueChanged()
	{
		UpgradeIcon selected = GetSelected();
		purchaseButton.gameObject.SetActive(selected != null);	// if selected == null, hide button; else, show it
		if (selected != null)
		{
			descriptionText.UpdateText(selected.data.description);
			purchaseButton.interactable = !selected.unlocked;
		}
		else
		{
			descriptionText.SetToDefaultText();
		}
	}

	public void OnPurchaseSelected()
	{
		if (GameManager.instance.wallet.TrySpend(GetSelected().data.cost))
		{
			GameManager.instance.saveGame.GetHeroData(selectedHero).numPowerUpsUnlocked++;
			SaveLoad.Save();
			RefreshScrollView(DataManager.GetPowerUpListData(selectedHero));
			OnTogglesValueChanged();
			SoundManager.instance.PlaySingle(purchaseSound);
		}
		else
		{
			SoundManager.instance.PlaySingle(notEnoughMoneySound);
		}
	}
}*/