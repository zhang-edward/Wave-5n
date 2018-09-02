using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroTypesMenu : MonoBehaviour {

	public const string NFI_KEY_PREFIX = "UNLOCKED_HERO_";

	public ScrollViewSnap scrollView;
	public GameObject heroTypeIconPrefab;
	public GameObject scrollLeftButton, scrollRightButton;
	public NewFeatureIndicator[] tierButtonIndicators;
	public TMPro.TMP_Text heroTypeText;
	public GameObject lockedPanel;
	public TMPro.TMP_Text questText;

	private HeroTypeIcon[] icons;
	private bool initialized;

	void Start() {
		icons = new HeroTypeIcon[System.Enum.GetValues(typeof(HeroType)).Length];
		scrollView.content = new List<GameObject>();
		for (int i = 0; i < icons.Length; i ++ ) {
			GameObject o = Instantiate(heroTypeIconPrefab);
			scrollView.content.Add(o);
			o.transform.SetParent(scrollView.panel, false);
			HeroTypeIcon heroTypeIcon = o.GetComponent<HeroTypeIcon>();
			icons[i] = heroTypeIcon;
			bool unlocked = GameManager.instance.save.UnlockedHeroes[i * 3];	// i * 3 because we want the t1 hero of each type
			heroTypeIcon.Init((HeroType)i, HeroTier.tier1, unlocked);
		}
		// Debug.Break();
		scrollView.Init();
		scrollView.OnSelectedContentChanged += OnSelectedContentChanged;
		OnSelectedContentChanged();
	}

	void OnEnable() {
		if (!initialized)
			return;
		OnSelectedContentChanged();
	}

	void OnDisable() {
		scrollView.SetSelectedContentIndex(0);
		scrollView.ForcePosition();
		UpdateTier(0);
	}

	public void UpdateTier(int tier) {
		int type = scrollView.selectedContentIndex;
		bool unlocked = GameManager.instance.save.UnlockedHeroes[SaveModifier.HeroTypeTier2Index(type, tier)];
		icons[scrollView.selectedContentIndex].Init((HeroType)type, (HeroTier)tier, unlocked);
		OnSelectedContentChanged();
	}

	private void OnSelectedContentChanged() {
		HeroTypeIcon selectedIcon = icons[scrollView.selectedContentIndex];
		HeroType selectedType = selectedIcon.type;
		heroTypeText.text = selectedType.ToString();
		// Update tier buttons
		for (int tier = 1; tier < tierButtonIndicators.Length; tier ++) {
			NewFeatureIndicator nfi = tierButtonIndicators[tier];
			if (GameManager.instance.save.UnlockedHeroes[SaveModifier.HeroTypeTier2Index((int)selectedType, tier)]) {
				nfi.RegisterKey(GetKey((int)selectedType, tier));
			}
			else {
				nfi.gameObject.SetActive(false);
			}
		}
		
		// Update locked-ness of this hero
		HeroTier selectedTier = selectedIcon.tier;
		if (!GameManager.instance.save.UnlockedHeroes[SaveModifier.HeroTypeTier2Index((int)selectedType, (int)selectedTier)]) {
			lockedPanel.SetActive(false);
			lockedPanel.SetActive(true);
			Quests.Quest quest = DataManager.GetPlayerHero(selectedType).GetComponent<PlayerHero>().GetUnlockQuest(selectedTier);
			if (quest != null)
				questText.text = quest.QuestDescription();
			else
				questText.text = "Currently Unobtainable";
		}
		else {
			lockedPanel.SetActive(false);
		}

		scrollLeftButton.SetActive(scrollView.selectedContentIndex != 0);
		scrollRightButton.SetActive(scrollView.selectedContentIndex != scrollView.content.Count - 1);
	}

	public static string GetKey(int type, int tier) {
		return NFI_KEY_PREFIX + ((HeroType)type).ToString() + "_" + tier;
	}
}