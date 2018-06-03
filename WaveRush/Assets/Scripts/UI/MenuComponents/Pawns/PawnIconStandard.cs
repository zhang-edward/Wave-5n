using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PawnIconStandard : PawnIcon {
#pragma warning disable 0649
	[Header("Card Decorative Elements")]
	[SerializeField] private Sprite[] panelTierSprites;
	[SerializeField] private Sprite[] starSprites;
	[Header("Card UI Elements")]
	[SerializeField] private TMP_Text heroNameText;
	[SerializeField] private TMP_Text heroLevelText;
	[SerializeField] private bool levelInShortFormat;
	[SerializeField] private Image heroPortrait;
	[Header("Optional UI Elements")]
	[SerializeField] protected Slider experienceSlider;
	[SerializeField] private GameObject highlight;
	[SerializeField] private Image heroPortraitBorder;
	[SerializeField] private Image heroStars;
	[SerializeField] private Image[] panels;
#pragma warning restore 0649

	public Button button;       // if it is interactable
	public delegate void Click(PawnIconStandard iconData);
	public Click onClick;

	void Awake()
	{
		// initialize button interactivity
		if (button != null)
			button.onClick.AddListener(() => OnClick());
	}

	public override void Init(Pawn pawnData)
	{
		base.Init(pawnData);
		// initialize display items
		heroNameText.text = pawnData.type.ToString();
		SetLevel(pawnData.level);

		switch (pawnData.tier)
		{
			case HeroTier.tier1:
				heroPortrait.sprite = DataManager.GetHeroData(pawnData.type).icons[0];
				break;
			case HeroTier.tier2:
				heroPortrait.sprite = DataManager.GetHeroData(pawnData.type).icons[1];
				break;
			case HeroTier.tier3:
				heroPortrait.sprite = DataManager.GetHeroData(pawnData.type).icons[2];
				break;
		}
		InitOptionalElements();
	}

	protected void SetLevel(int level) {
		if (levelInShortFormat)
			heroLevelText.text = level.ToString();
		else
			heroLevelText.text = "lv." + level.ToString();
	}

	void Update()
	{
	}

	public void SetHighlight(bool active) {
		highlight.SetActive(active);
	}

	private void OnClick()
	{
		if (onClick != null)
			onClick(this);
	}

	private void InitOptionalElements()
	{
		if (experienceSlider != null) {
			print("Pawn: " + pawnData);
			print("Max value: " + pawnData.MaxExperience + ", " + pawnData.Experience);
			experienceSlider.maxValue = pawnData.MaxExperience;
			experienceSlider.value = pawnData.Experience;
		}
		if (heroStars != null) {
			int index = (int)pawnData.tier;
			heroStars.color = Color.white;
			heroStars.enabled = true;
			heroStars.sprite = starSprites[index];
		}
		if (panels.Length > 0) {
			switch(pawnData.tier)
			{
				case HeroTier.tier1:
					SetPanels(panelTierSprites[0]);
					break;
				case HeroTier.tier2:
					SetPanels(panelTierSprites[1]);
					break;
				case HeroTier.tier3:
					SetPanels(panelTierSprites[2]);
					break;
			}
		}
	}

	private void SetPanels(Sprite s) {
		foreach (Image img in panels) {
			img.sprite = s;
		}
	}
}
