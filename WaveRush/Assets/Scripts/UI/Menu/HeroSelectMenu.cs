using UnityEngine;
using UnityEngine.UI;

public class HeroSelectMenu : MonoBehaviour  {

	public PawnSelectionView pawnSelectionView;
	public PawnIconStandard selectedPawnIcon;		// The big view of the selected pawn icon
	private PawnIconStandard highlightedPawnIcon;	// The highlighted pawn icon in pawnSelectionView
	public PawnInfoPanel pawnInfoPanel;
	public StageIcon selectedStageIcon;
	public Button toBattleSceneButton;

	void Start()
	{
		pawnSelectionView.Init(GameManager.instance.save.pawns, PawnSelectionView.PawnSelectionViewMode.Sorted);
		Init();
	}

	public void Init()
	{
		print("Initializing HeroSelectMenu!");
		selectedPawnIcon.onClick = (PawnIconStandard iconData) => {
			pawnInfoPanel.gameObject.SetActive(true);
			pawnInfoPanel.Init(iconData.pawnData);
		};
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons)
		{
			PawnIconStandard pawnIconStandard = (PawnIconStandard)pawnIcon;
			pawnIconStandard.onClick = (PawnIconStandard iconData) => {
				// If this is not the selected pawnIcon
				if (selectedPawnIcon.pawnData != iconData.pawnData)	
				{
					// Select this pawnIcon and deselect previous pawn icon (if it exists)
					if (highlightedPawnIcon != null)
						highlightedPawnIcon.SetHighlight(false);
					highlightedPawnIcon = iconData;
					highlightedPawnIcon.SetHighlight(true);

					selectedPawnIcon.Init(iconData.pawnData);
					// Set gameobject to false then true so animation plays
					selectedPawnIcon.gameObject.SetActive(false);
					selectedPawnIcon.gameObject.SetActive(true);
					GameManager.instance.selectedPawns[0] = pawnIcon.pawnData;
				}
				else
				{
					// Deselect this pawnIcon
					highlightedPawnIcon.SetHighlight(false);
					highlightedPawnIcon = null;

					selectedPawnIcon.pawnData = null;
					selectedPawnIcon.gameObject.SetActive(false);
					GameManager.instance.selectedPawns[0] = null;
				}
			};
		}
	}

	void Update()
	{
		toBattleSceneButton.interactable = highlightedPawnIcon != null;
	}

	void OnEnable()
	{
		GameManager gm = GameManager.instance;
		StageData selectedStage = gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex);
		selectedStageIcon.Init(selectedStage);
	}
}
