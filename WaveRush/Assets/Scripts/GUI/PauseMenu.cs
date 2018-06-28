using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public ModalSelectionView modalSelection;
	public PawnIconStandard pawnIcon;
	public PawnInfoPanel infoPanel;
	public Player player;

	void Start() {
		Pawn pawn = GameManager.instance.selectedPawns[player.activePartyMember];
		pawnIcon.Init(pawn);
		pawnIcon.onClick += (foo) => {
			infoPanel.gameObject.SetActive(true);
			infoPanel.Init(pawn);
		};
		modalSelection.OnOptionSelected += (int selection) => {
			if (selection == 1) {
				GameManager.instance.GoToScene(GameManager.SCENE_MAINMENU);
			}
		};
	}
}
