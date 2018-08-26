using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SkinsMenu : MonoBehaviour {

	GameManager gm;

	public PawnIconStandard bigIconDisplay;
	public PawnSelectionView pawnSelectionView;
	public GameObject scrollLeftButton, scrollRightButton;
	public TMPro.TMP_Text skinNameText;
	public TMPro.TMP_Text lockedText;
	public Button confirmButton, cancelButton;

	private PawnIconStandard selectedIcon;
	private AnimationSet[] skins;
	private bool initialized;
	private int skinIndex;
	private Pawn modifiedPawn;

	void Awake() {
		Init();
	}

	void Init() {
		gm = GameManager.instance;
		pawnSelectionView.Init(gm.save.pawns, PawnSelectionView.PawnSortMode.Sorted);
		initialized = true;
		Refresh();
	}

	private void GetSkins(HeroType type, HeroTier tier) {
		// Get all possible skins for this hero type and tier
		HeroData heroData = DataManager.GetHeroData(type);
		switch (tier) {
			case HeroTier.tier1:
				skins = heroData.t1Skins;
				break;
			case HeroTier.tier2:
				skins = heroData.t2Skins;
				break;
			case HeroTier.tier3:
				skins = heroData.t3Skins;
				break;
			default:
				skins = new AnimationSet[0];
				Debug.LogError("Unexpected tier");
				break;
		}
	}

	public void NextSkin() {
		skinIndex ++;
		OnSelectedContentChanged();
	}

	public void PreviousSkin() {
		skinIndex --;
		OnSelectedContentChanged();
	}

	private void OnSelectedContentChanged() {
		if (selectedIcon == null) {
			DeselectIcon();
			skinNameText.text = "Select a Hero";
			lockedText.gameObject.SetActive(false);
			scrollLeftButton.SetActive(false);
			scrollRightButton.SetActive(false);
			return;
		}
		// Set big icon display
		modifiedPawn.skin = skinIndex;
		bigIconDisplay.Init(modifiedPawn);
		bigIconDisplay.gameObject.SetActive(false);
		bigIconDisplay.gameObject.SetActive(true);
		// Unlocked or not
		if (gm.save.IsSkinUnlocked(modifiedPawn.type, modifiedPawn.tier, skinIndex)) {
			confirmButton.interactable = true;
			lockedText.gameObject.SetActive(false);
		}
		else {
			confirmButton.interactable = false;
			lockedText.gameObject.SetActive(true);
		}
		// Update skin name
		skinNameText.text = skins[skinIndex].animationSetName;
		// Update scrolling buttons
		scrollLeftButton.SetActive(skinIndex != 0);
		scrollRightButton.SetActive(skinIndex != skins.Length - 1);
	}

	public void Refresh() {
		pawnSelectionView.Refresh();
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons) {
			PawnIconStandard icon = (PawnIconStandard)pawnIcon;
			icon.button.interactable = true;
			icon.onClick = (clickedIcon) => {
				if (selectedIcon != null) {
					DeselectIcon();
				}
				selectedIcon = clickedIcon;
				selectedIcon.button.interactable = false;
				bigIconDisplay.Init(clickedIcon.pawnData);
				bigIconDisplay.gameObject.SetActive(true);

				GetSkins(clickedIcon.pawnData.type, clickedIcon.pawnData.tier);
				modifiedPawn = new Pawn(clickedIcon.pawnData);
				OnSelectedContentChanged();
			};
		}
		OnSelectedContentChanged();
	}

	public void DeselectIcon() {
		if (selectedIcon != null)
			selectedIcon.button.interactable = true;
		bigIconDisplay.gameObject.SetActive(false);
		selectedIcon = null;
	}

	public void Confirm() {
		int id = selectedIcon.pawnData.Id;
		gm.save.GetPawn(id).skin = skinIndex;
		pawnSelectionView.Refresh();
		DeselectIcon();
	}
}