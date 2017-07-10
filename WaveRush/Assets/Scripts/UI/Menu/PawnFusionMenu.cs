using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PawnFusionMenu : MonoBehaviour
{
	private int numSelected;
	private PawnIconStandard highlightedIcon;
	private GameObject[] selectedIcons = new GameObject[2];

	public PawnSelectionView pawnSelectionView;
	public PawnInfoPanel infoPanel;
	[Header("Fusion Menu")]
	public PawnIconStandard resultIcon;
	public PawnIconStandard fuseMatIcon1, fuseMatIcon2;
	[Header("Highlight Menu")]
	public GameObject highlightMenu;
	public Button selectButton, infoButton;

	void Awake()
	{
		pawnSelectionView.Init();
		// Clicking on the fuseMatIcons deselects that pawn for fusion
		fuseMatIcon1.onClick = (iconData) =>
		{
			selectedIcons[0].SetActive(true);
			fuseMatIcon1.gameObject.SetActive(false);
			numSelected--;
			UpdatePawnSelectionViewInteractability();
		};
		fuseMatIcon2.onClick = (iconData) =>
		{
			selectedIcons[1].SetActive(true);
			fuseMatIcon2.gameObject.SetActive(false);
			numSelected--;
			UpdatePawnSelectionViewInteractability();
		};
	}

	void OnEnable()
	{
		// Clicking on a pawn in the pawnSelectionView brings up the highlight menu
		pawnSelectionView.Refresh();
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons)
		{
			PawnIconStandard icon = (PawnIconStandard)pawnIcon;
			icon.onClick = (iconData) =>
			{
				highlightMenu.SetActive(false);		// Disable then enable so the animation plays
				highlightedIcon = iconData;
				highlightMenu.SetActive(true);
				highlightMenu.transform.SetParent(iconData.transform, false);
			};
		}
	}

	private void OnDisable()
	{
		Reset();
	}


	public void Reset()
	{
		foreach (GameObject icon in selectedIcons)
			if (icon != null)
				icon.SetActive(true);
		fuseMatIcon1.gameObject.SetActive(false);
		fuseMatIcon2.gameObject.SetActive(false);
		resultIcon.gameObject.SetActive(false);
		highlightMenu.SetActive(false);
		numSelected = 0;
		UpdatePawnSelectionViewInteractability();
		pawnSelectionView.Refresh();
	}

	private void UpdatePawnSelectionViewInteractability()
	{
		pawnSelectionView.GetComponent<CanvasGroup>().interactable = numSelected < 2;
	}

	// Called by selectButton (in inspector, in the highlight menu)
	public void SelectPawnIcon()
	{
		PawnIcon fuseMatIcon;
		int index;
		if (!fuseMatIcon1.gameObject.activeInHierarchy)
		{
			fuseMatIcon = fuseMatIcon1;
			index = 0;
		}
		else
		{
			fuseMatIcon = fuseMatIcon2;
			index = 1;
		}

		selectedIcons[index] = highlightedIcon.gameObject;
		highlightedIcon.gameObject.SetActive(false);
		highlightMenu.SetActive(false);
		fuseMatIcon.Init(highlightedIcon.pawnData);
		fuseMatIcon.gameObject.SetActive(true);
		numSelected++;
		if (numSelected > 2)
			numSelected = 2;
		UpdatePawnSelectionViewInteractability();
		//print("numSelected:" + numSelected);
	}

	// Called by infoButton (in inspector, in the highlight menu)
	public void OpenInfoPanel()
	{
		infoPanel.gameObject.SetActive(true);
		infoPanel.Init(highlightedIcon.pawnData);
	}

	public void ConfirmResult()
	{
		print("Refreshing page");
		pawnSelectionView.Refresh();
		Reset();
	}

	// ==========
	// Pawn Fusion Logic
	// ==========

	public void FusePawns()
	{
		SaveGame saveGame = GameManager.instance.saveGame;
		if (numSelected != 2)
		{
			Debug.LogWarning("You must select 2 heroes!");
			return;
		}
		Pawn pawn1 = selectedIcons[0].GetComponent<PawnIcon>().pawnData;
		Pawn pawn2 = selectedIcons[1].GetComponent<PawnIcon>().pawnData;
		Debug.Log("Pawn1:" + pawn1 +
		  "\nPawn2:" + pawn2);

		if (CheckCanFusePawns(pawn1, pawn2))
		{
			Pawn pawn = GetFusedPawn(pawn1, pawn2);
			saveGame.RemovePawn(pawn1.id);
			saveGame.RemovePawn(pawn2.id);
			saveGame.AddPawn(pawn, true, 100);
			SaveLoad.Save();
			fuseMatIcon1.gameObject.SetActive(false);
			fuseMatIcon2.gameObject.SetActive(false);
			resultIcon.gameObject.SetActive(true);
			resultIcon.Init(pawn);
		}
	}

	// NOTE: Order of the condition checks here matters!
	public bool CheckCanFusePawns(Pawn pawn1, Pawn pawn2)
	{
		if (pawn1.type != pawn2.type)
		{
			Debug.LogWarning("Heroes must be the same type!");
			return false;
		}
		if (pawn1.level >= Pawn.MAX_LEVEL || pawn2.level >= Pawn.MAX_LEVEL)
		{
			Debug.LogWarning("You cannot fuse a max level hero!");
			return false;
		}
		if (pawn1.tier != pawn2.tier)
		{
			Debug.LogWarning("The heroes must be the same tier!");
			return false;
		}
		if (pawn1.atThresholdLevel != pawn2.atThresholdLevel) // Either they are both at threshold or both aren't
		{
			Debug.LogWarning("Heroes must both be at max level to ascend tiers!");
			return false;
		}
		return true;
	}

	private Pawn GetFusedPawn(Pawn pawn1, Pawn pawn2)
	{
		UnityEngine.Assertions.Assert.IsTrue(CheckCanFusePawns(pawn1, pawn2));

		int level;  // the level of the fused pawn
					// Get higher level out of the two pawns
		if (pawn1.level > pawn2.level)
			level = pawn1.level + 1;
		else
			level = pawn2.level + 1;
		// Make the new pawn
		Pawn pawn = new Pawn(pawn1.type);
		pawn.level = level;
		return pawn;
	}
}