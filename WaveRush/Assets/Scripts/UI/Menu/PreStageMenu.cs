using UnityEngine;
using UnityEngine.UI;

public class PreStageMenu : MonoBehaviour
{
	public PawnSelectionView pawnSelectionView;
	public PawnInfoPanel pawnInfoPanel;

	void Start()
	{
		pawnSelectionView.Init();
		Init();
	}

	public void Init()
	{
		foreach(PawnIcon pawnIcon in pawnSelectionView.pawnIcons)
		{
			Button button = pawnIcon.GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				pawnInfoPanel.Init(pawnIcon.pawnData);
				pawnInfoPanel.gameObject.SetActive(true);
				GameManager.instance.selectedPawn = pawnIcon.pawnData;
			});
		}
	}
}
