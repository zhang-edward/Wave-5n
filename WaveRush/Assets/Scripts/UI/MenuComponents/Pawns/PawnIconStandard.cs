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
	[SerializeField] private TMP_Text boostsText;
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
		SetLevel(pawnData.level);
		heroNameText.text = pawnData.type.ToString();
		heroPortrait.sprite = DataManager.GetHeroData(pawnData.type).icons[(int)pawnData.tier];
		int numBoosts = pawnData.GetNumBoosts();
		if (numBoosts > 0) {
			boostsText.gameObject.SetActive(true);
			boostsText.text = string.Format("+{0}", numBoosts);
		}
		else {
			boostsText.gameObject.SetActive(false);
		}
		InitOptionalElements();
	}

	protected void SetLevel(int level) {
		if (levelInShortFormat)
			heroLevelText.text = level.ToString();
		else
			heroLevelText.text = "lv." + level.ToString();
	}

	void Update() {
	}

	public void SetHighlight(bool active, Color color) {
		highlight.SetActive(active);
		highlight.GetComponent<Image>().color = color;
	}

	private void OnClick()
	{
		if (onClick != null)
			onClick(this);
	}

	private void InitOptionalElements()
	{
		if (experienceSlider != null) {
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
