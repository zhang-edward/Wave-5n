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

	[Header("Audio")]
	public AudioClip onPawnSelectedSound;

	private GameManager gm;
	private PawnIconStandard selectedIcon;

	private bool initialized = false;

	void Awake() {
		Init();
	}

	void Init() {
		gm = GameManager.instance;
		pawnSelectionView.Init(gm.save.pawns, PawnSelectionView.PawnSortMode.Sorted);
		initialized = true;
	}

	public void Refresh() {
		retireMoneyText.text = "0";
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
				int costMoney, costSouls;
				Formulas.PawnCost(selectedIcon.pawnData, out costMoney, out costSouls);
				retireMoneyText.text = ((int)((costMoney + 2 * costSouls) * 0.2f)).ToString();
				SoundManager.instance.RandomizeSFX(onPawnSelectedSound);
			};
		}
	}

	void OnEnable() {
		if (!initialized)
			return;
		Refresh();
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
		selectedIcon.button.interactable = true;
		bigIconDisplay.gameObject.SetActive(false);
		selectedIcon = null;
	}

	public void OpenInfoPanel() {
		infoPanel.gameObject.SetActive(true);
		infoPanel.Init(selectedIcon.pawnData);
		// Debug
		string str = Pawn.Pawn2String(selectedIcon.pawnData);
		Pawn.String2Pawn(str);
	}

	public void RetirePawn() {
		if (gm.save.NumPawns() == 1) {
			gm.DisplayAlert("You cannot retire your last hero!");
			return;
		}
		int costMoney, costSouls;
		Formulas.PawnCost(selectedIcon.pawnData, out costMoney, out costSouls);
		gm.save.AddMoney((int)((costMoney + 2 * costSouls) * 0.2f));
		gm.save.RemovePawn(selectedIcon.pawnData.Id);
		bigIconDisplay.gameObject.SetActive(false);
		selectedIcon = null;
		pawnSelectionView.UpdatePawnList(gm.save.pawns);
	}
}