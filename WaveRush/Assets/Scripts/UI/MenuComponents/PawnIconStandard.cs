using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PawnIconStandard : PawnIcon
{
	[Header("Card Decorative Elements")]
	public Sprite[] panelTierSprites;
	public Sprite[] starSprites;
	[Header("Card UI Elements")]
	public Text heroNameText;
	public Text heroLevelText;
	public bool levelInShortFormat;
	public Image heroPortrait;
	[Header("Optional UI Elements")]
	public Image heroPortraitBorder;
	public Image heroStars;
	public Image[] panels;

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
		if (levelInShortFormat)
			heroLevelText.text = pawnData.level.ToString();
		else
			heroLevelText.text = "lv." + pawnData.level.ToString();

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

	private void OnClick()
	{
		if (onClick != null)
			onClick(this);
	}

	private void InitOptionalElements()
	{
		if (heroStars != null)
		{
			if (pawnData.level == 0)
			{
				heroStars.color = Color.clear;
			}
			else
			{
				heroStars.color = Color.white;
				heroStars.sprite = starSprites[pawnData.level - 1];
			}
		}
		if (panels.Length > 0)
		{
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

	private void SetPanels(Sprite s)
	{
		foreach (Image img in panels)
		{
			img.sprite = s;
		}
	}
}
