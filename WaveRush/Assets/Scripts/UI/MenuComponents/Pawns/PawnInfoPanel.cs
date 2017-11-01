using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PawnInfoPanel : MonoBehaviour
{
	[Header("Set from prefab")]
	public PawnIconStandard pawnIcon;
	public TMP_Text damageText, healthText, livesText;
	public ScrollingText infoText;
	public Button bottomButton;
	[Header("Mid Panel Menus")]
	public Transform powersPanel;
	public GameObject abilityIcon1, abilityIcon2, specialAbilityIcon;
	public ToggleGroup infoIconsToggleGroup;
	public ToggleGroup midPanelTabToggleGroup;
	public ScrollViewSnap midPanelScrollView;
	public NewFeatureIndicator newTextAbilities, newTextPowers;		// Flashing "new" symbol if the player has not
																	// viewed the powers or abilities for the specified hero
	[Header("Prefabs")]
	public GameObject heroPowerUpInfoPrefab;
	[Header("Properties")]
	public bool hasButton;
	private GameObject[] heroPowerUpInfoIcons;

	private string HasPlayerViewedAbilitiesKey {
		get {
			return pawnIcon.pawnData.type.ToString() + "_Ablities";
		}
	}
	private string HasPlayerViewedPowersKey {
		get {
			return pawnIcon.pawnData.type.ToString() + "_Powers";
		}
	}

	void Awake()
	{
		heroPowerUpInfoIcons = new GameObject[HeroPowerUpListData.powerUpUnlockLevels.Length];
		for (int i = 0; i < HeroPowerUpListData.powerUpUnlockLevels.Length; i++)
		{
			GameObject o = Instantiate(heroPowerUpInfoPrefab);
			o.transform.SetParent(powersPanel, false);
			o.GetComponent<ScrollingTextOption>().scrollingText = infoText;
			o.GetComponent<Toggle>().group = infoIconsToggleGroup;
			heroPowerUpInfoIcons[i] = o;
		}
	}

	void OnEnable()
	{
		bottomButton.gameObject.SetActive(hasButton);
		foreach(GameObject o in heroPowerUpInfoIcons)
		{
			o.GetComponent<Toggle>().isOn = false;
		}
	}

	public void Init(Pawn pawn)
	{
		pawnIcon.Init(pawn);
		// Get Hero data
		HeroData heroData = DataManager.GetHeroData(pawn.type);
		// Initialize the hero's power up info
		HeroPowerUpListData powerUpListData = DataManager.GetPowerUpListData(pawn.type);
		int numPowerUpsUnlocked = HeroPowerUpListData.GetNumPowerUpsUnlocked(pawn.level);
		for (int i = 0; i < HeroPowerUpListData.powerUpUnlockLevels.Length; i++)
		{
			bool locked = i >= numPowerUpsUnlocked;
			int unlockedLevel = HeroPowerUpListData.powerUpUnlockLevels[i];
			HeroPowerUpData data = powerUpListData.GetPowerUpFromIndex(i).data;
			heroPowerUpInfoIcons[i].GetComponent<HeroPowerUpInfoIcon>().Init(data, locked, unlockedLevel);
			heroPowerUpInfoIcons[i].GetComponent<ScrollingTextOption>().scrollingText = infoText;
		}
		// Reset MidPanel Menu
		midPanelTabToggleGroup.SetAllTogglesOff();
		// Due to some stupid bug we have to wait one frame before initializing the scroll view
		StartCoroutine(ForcePosAfter1Frame());
		// Initialize the scrolling text to display info about the hero
		infoText.defaultText = heroData.heroDescription;
		infoText.SetToDefaultText();
		// Initialize NewFeatureIndicator
		newTextPowers.RegisterKey(HasPlayerViewedPowersKey);
		newTextAbilities.RegisterKey(HasPlayerViewedAbilitiesKey);
		// Initialize Abilities Panel
		abilityIcon1.GetComponent<Image>().sprite = heroData.abilityIcons[0];
		abilityIcon2.GetComponent<Image>().sprite = heroData.abilityIcons[1];
		specialAbilityIcon.GetComponent<Image>().sprite = heroData.specialAbilityIcon;
		abilityIcon1.GetComponent<ScrollingTextOption>().text = heroData.ability1Description;
		abilityIcon2.GetComponent<ScrollingTextOption>().text = heroData.ability2Description;
		specialAbilityIcon.GetComponent<ScrollingTextOption>().text = heroData.specialDescription;
	}

	private IEnumerator ForcePosAfter1Frame()
	{
		yield return new WaitForEndOfFrame();
		midPanelScrollView.SetSelectedContentIndex(1);
		midPanelScrollView.ForcePosition();
	}

	// If a mid panel toggle was turned off, return to middle tab
	public void ToggledMidPanelTab(bool toggle)
	{
		if (!toggle)
			midPanelScrollView.SetSelectedContentIndex(1);
	}

	public void SetViewedAbilities()
	{
		//print("Viewing abilities for " + pawnIcon.pawnData.type.ToString());
		GameManager.instance.SetHasPlayerViewedKey(HasPlayerViewedAbilitiesKey, true);
	}

	public void SetViewedPowers()
	{
		//print("Viewing powers for " + pawnIcon.pawnData.type.ToString());
		GameManager.instance.SetHasPlayerViewedKey(HasPlayerViewedPowersKey, true);
	}
}
