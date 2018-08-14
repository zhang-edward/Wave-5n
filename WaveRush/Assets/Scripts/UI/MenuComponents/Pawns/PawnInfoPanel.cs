using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PawnInfoPanel : MonoBehaviour
{
	[Header("Set from prefab")]
	public PawnIconStandard pawnIcon;
	public ScrollingText infoText;
	public Sprite heroParrySprite;
	[Header("Mid Panel Menus")]
	public Transform powersPanel;
	public GameObject abilityIcon1, abilityIcon2, sAbilityIcon, parryIcon;
	public ToggleGroup infoIconsToggleGroup;
	public ToggleGroup midPanelTabToggleGroup;
	public ScrollViewSnap midPanelScrollView;
	public TMP_Text[] boostInfoTexts;
	public TMP_Text[] statInfoTexts;
	[Header("NewIndicators")]
	public NewFeatureIndicator newSpecial;
	public NewFeatureIndicator newParry;
	public NewFeatureIndicator[] newAbility;
	public NewFeatureIndicatorGroup newAbilitiesTab, newPowersTab;
	private NewFeatureIndicator[] newPowers;
	[Header("Prefabs")]
	public GameObject heroPowerUpInfoPrefab;
	private GameObject[] heroPowerUpInfoIcons;

	void Awake()
	{
		heroPowerUpInfoIcons = new GameObject[HeroPowerUpListData.powerUpUnlockLevels.Length];
		newPowers = new NewFeatureIndicator[heroPowerUpInfoIcons.Length];
		for (int i = 0; i < HeroPowerUpListData.powerUpUnlockLevels.Length; i++) {
			GameObject o = Instantiate(heroPowerUpInfoPrefab);
			o.transform.SetParent(powersPanel, false);
			o.GetComponent<ScrollingTextOption>().scrollingText = infoText;
			o.GetComponent<Toggle>().group = infoIconsToggleGroup;
			newPowers[i] = o.GetComponent<HeroPowerUpInfoIcon>().newPowerUp;
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
		print(pawn);
		pawnIcon.Init(pawn);
		HeroData heroData = DataManager.GetHeroData(pawn.type);
		PlayerHero playerHero = DataManager.GetPlayerHero(pawn.type).GetComponent<PlayerHero>();
		SetStatInfoTexts(pawn);
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
		// Initialize ScrollView
		StartCoroutine(ForcePosAfter1Frame());
		// Initialize the scrolling text to display info about the hero
		infoText.defaultText = heroData.heroDescription;
		infoText.SetToDefaultText();
		// Initialize NewFeatureIndicator
		newAbility[0].RegisterKey(GetViewedAbilityKey('0'));
		newAbility[1].RegisterKey(GetViewedAbilityKey('1'));
		newSpecial.RegisterKey(GetViewedAbilityKey('S'));
		newParry.RegisterKey(GetViewedAbilityKey('P'));
		foreach (NewFeatureIndicator nfi in newPowers)
			newPowersTab.newFeatureIndicators.Add(nfi);
		foreach (NewFeatureIndicator nfi in newAbility)
			newAbilitiesTab.newFeatureIndicators.Add(nfi);
		newAbilitiesTab.newFeatureIndicators.Add(newSpecial);
		newAbilitiesTab.newFeatureIndicators.Add(newParry);

		/** Initialize Abilities Panel */
		// Set image
		abilityIcon1.GetComponent<Image>().sprite = heroData.abilityIcons[0];
		abilityIcon2.GetComponent<Image>().sprite = heroData.abilityIcons[1];
		sAbilityIcon.GetComponent<Image>().sprite = heroData.specialAbilityIcon;
		parryIcon	.GetComponent<Image>().sprite = heroParrySprite;

		// Set text option
		abilityIcon1.GetComponent<ScrollingTextOption>().text = heroData.ability1Name.ToUpper() + ": " + heroData.ability1Description + " (" + playerHero.cooldownTime[0] + " sec cooldown)";
		abilityIcon2.GetComponent<ScrollingTextOption>().text = heroData.ability2Name.ToUpper() + ": " + heroData.ability2Description + " (" + playerHero.cooldownTime[1] + " sec cooldown)";
		sAbilityIcon.GetComponent<ScrollingTextOption>().text = heroData.specialName.ToUpper()  + ": " + heroData.specialDescription;
		parryIcon	.GetComponent<ScrollingTextOption>().text = "PARRY: " + heroData.parryDescription;

		// Set toggle to false
		abilityIcon1.GetComponent<Toggle>().isOn = false;
		abilityIcon2.GetComponent<Toggle>().isOn = false;
		sAbilityIcon.GetComponent<Toggle>().isOn = false;
		parryIcon	.GetComponent<Toggle>().isOn = false;
	}

	// Have to do this due to some shitty ass bug
	private IEnumerator ForcePosAfter1Frame() {
		yield return new WaitForEndOfFrame();
		midPanelScrollView.Init();
		midPanelScrollView.SetSelectedContentIndex(1);
		midPanelScrollView.ForcePosition();
	}

	// If a mid panel toggle was turned off, return to middle tab
	public void ToggledMidPanelTab(bool toggle) {
		if (!toggle)
			midPanelScrollView.SetSelectedContentIndex(1);
	}

	/// <summary>
	/// Gets the key for the NewFeatureIndicator.
	/// </summary>
	/// <returns>The viewed ability key.</returns>
	/// <param name="ability">Either '0', '1', or 'S'</param>
	public string GetViewedAbilityKey(char ability) {
		return pawnIcon.pawnData.type.ToString() + "_Abilities_" + ability;
	}

	public string GetViewedPowerKey(int powerIndex) {
		return pawnIcon.pawnData.type.ToString() + "_Powers_" + powerIndex;
	}

	private void SetStatInfoTexts(Pawn pawn) {
		for (int i = 0; i < StatData.NUM_STATS; i ++) {
			boostInfoTexts[i].text = string.Format("+{0}", pawn.boosts[i]);
		}
		float[] stats = pawn.GetStatsArray();
		statInfoTexts[StatData.STR].text  = string.Format("{0} dmg", 	  Mathf.RoundToInt(Formulas.PlayerDamage(pawn.level, (int)pawn.tier) * stats[StatData.STR]));
		statInfoTexts[StatData.VIT].text  = string.Format("{0} hp/heart", (int)stats[StatData.VIT]);
		statInfoTexts[StatData.CHG].text  = string.Format("{0}%", 		  stats[StatData.CHG]);
		statInfoTexts[StatData.DEX].text  = string.Format("{0}%",		  stats[StatData.DEX] * 100);
		statInfoTexts[StatData.CRIT].text = string.Format("{0}x",		  stats[StatData.CRIT]);
		statInfoTexts[StatData.LUCK].text = string.Format("{0}%",		  stats[StatData.LUCK] * 100);
	}

	public void ShowStatHelp(int statIndex) {
		infoText.UpdateText(DataManager.instance.statData.statDescriptions[statIndex]);
	}
}
