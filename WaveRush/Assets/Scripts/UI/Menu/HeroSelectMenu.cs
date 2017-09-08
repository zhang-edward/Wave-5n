using UnityEngine;
using UnityEngine.UI;

public class HeroSelectMenu : MonoBehaviour
{
	public PawnSelectionView pawnSelectionView;
	public PawnInfoPanel pawnInfoPanel;
	public StageIcon selectedStageIcon;

	void Start()
	{
		pawnSelectionView.Init();
		Init();
	}

	public void Init()
	{
		print("Initializing HeroSelectMenu!");
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons)
		{
			PawnIconStandard pawnIconStandard = (PawnIconStandard)pawnIcon;
			pawnIconStandard.onClick = (PawnIconStandard iconData) => {
				pawnInfoPanel.gameObject.SetActive(true);
				pawnInfoPanel.Init(iconData.pawnData);
				GameManager.instance.selectedPawn = pawnIcon.pawnData;
			};
		}
	}

	void OnEnable()
	{
		GameManager gm = GameManager.instance;
		StageData selectedStage = gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex);
		selectedStageIcon.Init(selectedStage);
	}
}
