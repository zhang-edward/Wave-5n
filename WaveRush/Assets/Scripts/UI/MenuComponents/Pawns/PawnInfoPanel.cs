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
	[Header("Mid Panel Menus")]
	public Transform powersPanel;
	public GameObject abilityIcon1, abilityIcon2, sAbilityIcon, parryIcon;
	public ToggleGroup infoIconsToggleGroup;
	public ToggleGroup midPanelTabToggleGroup;
	public ScrollViewSnap midPanelScrollView;
	//[Header("NewIndicators")]
	//public NewFeatureIndicator newSpecial;
	//public NewFeatureIndicator[] newAbility;
	//public NewFeatureIndicator newTextAbilities, newTextPowers;
	[Header("Prefabs")]
	public GameObject heroPowerUpInfoPrefab;
	private GameObject[] heroPowerUpInfoIcons;

	private string HasPlayerViewedAbilitiesTabKey {
		get {
			return pawnIcon.pawnData.type.ToString() + "_Ablities_Tab";
		}
	}
	private string HasPlayerViewedPowersTabKey {
		get {
			return pawnIcon.pawnData.type.ToString() + "_Powers_Tab";
		}
	}

	void Awake()
	{
		heroPowerUpInfoIcons = new GameObject[HeroPowerUpListData.powerUpUnlockLevels.Length];
		for (int i = 0; i < HeroPowerUpListData.powerUpUnlockLevels.Length; i++) {
			GameObject o = Instantiate(heroPowerUpInfoPrefab);
			o.transform.SetParent(powersPanel, false);
			o.GetComponent<ScrollingTextOption>().scrollingText = infoText;
			o.GetComponent<Toggle>().group = infoIconsToggleGroup;
			heroPowerUpInfoIcons[i] = o;
		}
	}

	void OnEnable()
	{
		foreach(GameObject o in heroPowerUpInfoIcons)
		{
			o.GetComponent<Toggle>().isOn = false;
		}
	}

	public void Init(Pawn pawn)
	{
		pawnIcon.Init(pawn);
		HeroData heroData = DataManager.GetHeroData(pawn.type);
		/** Initialize the hero's power up info */
		HeroPowerUpListData powerUpListData = DataManager.GetPowerUpListData(pawn.type);
		int numPowerUpsUnlocked = HeroPowerUpListData.GetNumPowerUpsUnlocked(pawn.level);
		for (int i = 0; i < HeroPowerUpListData.powerUpUnlockLevels.Length; i++)
		{
			// If the hero has unlocked this powerUp
			bool locked = i >= numPowerUpsUnlocked;
			int unlockedLevel = HeroPowerUpListData.powerUpUnlockLevels[i];
			HeroPowerUpData data = powerUpListData.powerUps[i].GetComponent<HeroPowerUp>().data;
			// Get the key for NewFeatureIndicator
			string newKey = GetViewedPowerKey(i);
			heroPowerUpInfoIcons[i].GetComponent<HeroPowerUpInfoIcon>().Init(data, locked, newKey, unlockedLevel);
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
		//newTextPowers.RegisterKey(HasPlayerViewedPowersTabKey);
		//newTextAbilities.RegisterKey(HasPlayerViewedAbilitiesTabKey);
		//newAbility[0].RegisterKey(GetViewedAbilityKey('0'));
		//newAbility[1].RegisterKey(GetViewedAbilityKey('1'));
		//newSpecial.RegisterKey(GetViewedAbilityKey('S'));

		/** Initialize Abilities Panel */
		// Set image
		abilityIcon1.GetComponent<Image>().sprite = heroData.abilityIcons[0];
		abilityIcon2.GetComponent<Image>().sprite = heroData.abilityIcons[1];
		sAbilityIcon.GetComponent<Image>().sprite = heroData.specialAbilityIcon;

		// Set text option
		abilityIcon1.GetComponent<ScrollingTextOption>().text = heroData.ability1Name.ToUpper() + ": " + heroData.ability1Description;
		abilityIcon2.GetComponent<ScrollingTextOption>().text = heroData.ability2Name.ToUpper() + ": " + heroData.ability2Description;
		sAbilityIcon.GetComponent<ScrollingTextOption>().text = heroData.specialName.ToUpper()  + ": " + heroData.specialDescription;
		parryIcon	.GetComponent<ScrollingTextOption>().text = "PARRY: " + heroData.parryDescription;

		// Set toggle to false
		abilityIcon1.GetComponent<Toggle>().isOn = false;
		abilityIcon2.GetComponent<Toggle>().isOn = false;
		sAbilityIcon.GetComponent<Toggle>().isOn = false;
		parryIcon	.GetComponent<Toggle>().isOn = false;
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

	/// <summary>
	/// Gets the key for the NewFeatureIndicator.
	/// </summary>
	/// <returns>The viewed ability key.</returns>
	/// <param name="ability">Either '0', '1', or 'S'</param>
	public string GetViewedAbilityKey(char ability)
	{
		return pawnIcon.pawnData.type.ToString() + "_Abilities_" + ability;
	}

	public string GetViewedPowerKey(int powerIndex)
	{
		return pawnIcon.pawnData.type.ToString() + "_Powers_" + powerIndex;
	}
}
