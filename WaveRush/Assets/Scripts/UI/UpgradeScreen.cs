using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class UpgradeScreen : MonoBehaviour
{
	private HeroType selectedHero;				// the selected hero to disply info for
	public HeroPowerUpListData[] powerUpList;	// power up info for every hero
	public GameObject upgradeIconPrefab;        // the icons for each power up

	[Header("In Prefab")]
	public Transform contentPanel;              // the folder in which upgradeIcons are placed
	public ToggleGroup toggleGroup;
	private List<UpgradeIcon> upgradeIcons = new List<UpgradeIcon>();

	public Text descriptionText;
	public Button purchaseButton;

	[Header("Audio")]
	public AudioClip purchaseSound;
	public AudioClip notEnoughMoneySound;

	void Start()
	{
		Init(HeroType.Knight);
	}

	public void Init(HeroType selectedHero)
	{
		this.selectedHero = selectedHero;
		RefreshScrollView(GetSelectedHeroData());
	}

	private HeroPowerUpListData GetSelectedHeroData()
	{
		foreach(HeroPowerUpListData data in powerUpList)
		{
			if (data.type == selectedHero)
				return data;
		}
		throw new UnityEngine.Assertions.AssertionException
		                     (this.name,
		                      "Could not find data for hero with name " + selectedHero.ToString() + "!");
	}

	private void RefreshScrollView(HeroPowerUpListData data)
	{
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

		// init each icon
		for (int i = 0; i < data.powerUps.Length; i ++)
		{
			// if 1 powerup unlocked, i = 0 should be unlocked
			// if 2 powerups unlocked, i = 0, 1 should be unlocked, etc.
			int powerUpsUnlocked = GameManager.instance.saveGame.GetHeroData(selectedHero).powerUpsUnlocked;
			bool powerUpUnlocked = i < powerUpsUnlocked;
			HeroPowerUp powerUp = data.powerUps[i];

			upgradeIcons[i].Init(this, powerUp.data, powerUpUnlocked);
			upgradeIcons[i].gameObject.SetActive(true);
			if (i > powerUpsUnlocked)
				upgradeIcons[i].gameObject.SetActive(false);
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
		descriptionText.GetComponent<ScrollingText>().UpdateText(selected.data.description);
		purchaseButton.interactable = !selected.unlocked;
	}

	public void OnPurchaseSelected()
	{
		if (GameManager.instance.wallet.TrySpend(GetSelected().data.cost))
		{
			GameManager.instance.saveGame.GetHeroData(selectedHero).powerUpsUnlocked++;
			RefreshScrollView(GetSelectedHeroData());
			OnTogglesValueChanged();
			SoundManager.instance.PlaySingle(purchaseSound);
			SaveLoad.Save();
		}
		else
		{
			SoundManager.instance.PlaySingle(notEnoughMoneySound);
		}
	}
}