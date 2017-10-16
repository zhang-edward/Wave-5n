using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PawnRetireMenu : MonoBehaviour
{
	public PawnSelectionView pawnSelectionView;
	private List<PawnIcon> selectedIcons = new List<PawnIcon>();

	public void Init()
	{
		pawnSelectionView.Init();
	}

	void OnEnable()
	{
		// Clicking on a pawn in the pawnSelectionView brings up the highlight menu
		pawnSelectionView.Refresh();
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons)
		{
			DeselectIcon(pawnIcon);
			PawnIconStandard icon = (PawnIconStandard)pawnIcon;
			icon.onClick = (iconData) =>
			{
				if (selectedIcons.Contains(iconData))
					DeselectIcon(iconData);
				else
					SelectIcon(iconData);
			};
		}
	}

	public void SelectIcon(PawnIcon icon)
	{
		selectedIcons.Add(icon);
		PawnIconStandard iconStandard = (PawnIconStandard)icon;
		iconStandard.highlight.gameObject.SetActive(true);
	}

	public void DeselectIcon(PawnIcon icon)
	{
		selectedIcons.Remove(icon);
		PawnIconStandard iconStandard = (PawnIconStandard)icon;
		iconStandard.highlight.gameObject.SetActive(false);
	}

	public void RetirePawns()
	{
		for (int i = selectedIcons.Count - 1; i >= 0; i --)
		{
			PawnIcon icon = selectedIcons[i];
			//print(icon + ": " + icon.pawnData);
			GameManager.instance.saveGame.RemovePawn(icon.pawnData.id);
			DeselectIcon(icon);
		}
		SaveLoad.Save();
		pawnSelectionView.Refresh();
	}
}
