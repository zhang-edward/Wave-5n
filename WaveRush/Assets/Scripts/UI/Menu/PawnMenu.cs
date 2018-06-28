using UnityEngine;
using UnityEngine.UI;

public class PawnMenu : MonoBehaviour {

	public const int MODAL_YES = 0;
	public const int MODAL_NO  = 1;

	public PawnIconStandard bigIconDisplay;
	//public Button infoButton, trainingButton, retireButton;
	public TMPro.TMP_Text retireMoneyText;
	public PawnInfoPanel infoPanel;
	public PawnSelectionView pawnSelectionView;
	public CanvasGroup optionsGroup;

	private GameManager gm;
	private PawnIconStandard selectedIcon;

	private bool initialized = false;

	void Awake() {
		Init();
	}

	void Init() {
		gm = GameManager.instance;
		pawnSelectionView.Init(gm.save.pawns, PawnSelectionView.PawnSelectionViewMode.Sorted);
		initialized = true;
	}

	void OnEnable() {
		if (!initialized)
			return;
		pawnSelectionView.Refresh();
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons) {
			PawnIconStandard icon = (PawnIconStandard)pawnIcon;
			icon.onClick = (clickedIcon) => {
				if (selectedIcon != null) {
					DeselectIcon();
				}
				selectedIcon = clickedIcon;
				selectedIcon.gameObject.SetActive(false);
				bigIconDisplay.Init(clickedIcon.pawnData);
				bigIconDisplay.gameObject.SetActive(true);
				retireMoneyText.text = Mathf.RoundToInt(Formulas.PawnCost(selectedIcon.pawnData) * 0.2f).ToString();
			};
		}
	}

	void OnDisable() {
		if (selectedIcon != null) {
			DeselectIcon();
		}
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons) {
			PawnIconStandard icon = (PawnIconStandard)pawnIcon;
			icon.onClick = null;
		}
	}

	void Update() {
		optionsGroup.interactable = selectedIcon != null;
	}

	public void DeselectIcon() {
		selectedIcon.gameObject.SetActive(true);
		bigIconDisplay.gameObject.SetActive(false);
		selectedIcon = null;
	}

	public void OpenInfoPanel() {
		infoPanel.gameObject.SetActive(true);
		infoPanel.Init(selectedIcon.pawnData);
	}

	public void RetirePawn() {
		gm.save.AddMoney(Mathf.RoundToInt(Formulas.PawnCost(selectedIcon.pawnData) * 0.2f));
		gm.save.RemovePawn(selectedIcon.pawnData.Id);
		bigIconDisplay.gameObject.SetActive(false);
		selectedIcon = null;
	}
}