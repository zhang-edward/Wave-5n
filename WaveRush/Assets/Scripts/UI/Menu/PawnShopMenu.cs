using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PawnShopMenu : MonoBehaviour
{
	GameManager gm;

	public PawnSelectionView pawnSelectionView;
	public Button recruitButton;
	public RectTransform scrollContent;
	public ScrollingText infoText;
	public float contentDistance;

	private PawnShop pawnShop;
	private PawnIconStandard selectedIcon;
	private Coroutine LerpToContentRoutine;

	void Awake() {
		pawnShop = new PawnShop();
	}

	void Start() {
		gm = GameManager.instance;
		// Initialize pawn selection view
		pawnShop.OnSaveGameLoaded(gm.save);
		pawnSelectionView.Init(pawnShop.AvailablePawns.ToArray(), pawnShop.OnPawnListUpdated);
		pawnSelectionView.SortByLevel();
		// Set onClick for each pawnIcon
		foreach (PawnIcon p in pawnSelectionView.pawnIcons) {
			PawnIconStandard icon = (PawnIconStandard)p;
			icon.onClick += (pawnIcon) => {
				if (pawnIcon != selectedIcon)
					SelectIcon(pawnIcon);
				else
					DeselectIcon();
			};
		}
		// Set recruitButton onClick
		recruitButton.onClick.AddListener(RecruitHero);
	}

	private void SelectIcon(PawnIcon icon) {
		if (selectedIcon != null)
			DeselectIcon();
		PawnIconStandard standardIcon = (PawnIconStandard)icon;
		standardIcon.SetHighlight(true);
		selectedIcon = standardIcon;
		StartLerpToContent();
		infoText.UpdateText(DataManager.GetHeroData(icon.pawnData.type).heroDescription);
	}

	private void DeselectIcon() {
		selectedIcon.SetHighlight(false);
		selectedIcon = null;
	}

	private void RecruitHero() {
		if (gm.save.AddPawn(selectedIcon.pawnData)) {
			selectedIcon.button.interactable = false;
			DeselectIcon();
		}
		else {
			gm.DisplayAlert("You can't recruit any more pawns!");
		}
	}

	void Init() {
	}

	private void StartLerpToContent() {
		if (LerpToContentRoutine != null)
			StopCoroutine(LerpToContentRoutine);
		LerpToContentRoutine = StartCoroutine(LerpToContent());
	}

	private IEnumerator LerpToContent() {
		float dest = pawnSelectionView.IndexOfPawnIcon(selectedIcon) * -contentDistance;
		while (Mathf.Abs(scrollContent.anchoredPosition.x - dest) > 0.05f) {
			// set new position
			float newX = Mathf.Lerp(scrollContent.anchoredPosition.x, dest, Time.deltaTime * 20);
			scrollContent.anchoredPosition = new Vector2(newX, scrollContent.anchoredPosition.y);
			yield return null;
		}
	}

	public void OnDrag() {
		StopCoroutine(LerpToContentRoutine);
	}
}
