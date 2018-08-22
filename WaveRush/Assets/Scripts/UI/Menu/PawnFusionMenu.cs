using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PawnFusionMenu : MonoBehaviour {
	private int numSelected;
	private PawnIconStandard highlightedIcon;
	private GameObject[] selectedIcons = new GameObject[2];

	public PawnSelectionView pawnSelectionView;
	public PawnInfoPanel infoPanel;
	[Header("Fusion Menu")]
	public Animator animator;
	public PawnIconStandard resultIcon;
	public PawnIconStandard fuseMatIcon1, fuseMatIcon2;
	public TMP_Text moneyCostText;
	public TMP_Text soulsCostText;
	public Animator resourceReqAnim;
	[Header("Highlight Menu")]
	public GameObject highlightMenu;
	public Button selectButton, infoButton;

	private bool initialized;

	public void Init() {
		// pawnSelectionView.Init(GameManager.instance.save.pawns, PawnSelectionView.PawnSelectionViewMode.Sorted);
		// Clicking on the fuseMatIcons deselects that pawn for fusion
		fuseMatIcon1.onClick = (iconData) => {
			selectedIcons[0].SetActive(true);
			fuseMatIcon1.pawnData = null;
			fuseMatIcon1.gameObject.SetActive(false);
			numSelected--;
			UpdatePawnSelectionViewInteractability();
			UpdateCostText();
		};
		fuseMatIcon2.onClick = (iconData) => {
			selectedIcons[1].SetActive(true);
			fuseMatIcon2.pawnData = null;
			fuseMatIcon2.gameObject.SetActive(false);
			numSelected--;
			UpdatePawnSelectionViewInteractability();
			UpdateCostText();
		};
		initialized = true;
	}

	void OnEnable() {
		if (!initialized)
			Init();
		pawnSelectionView.Refresh();
		moneyCostText.text = "0";
		soulsCostText.text = "0";
		SetOnClick();
	}

	private void SetOnClick() {
				// Clicking on a pawn in the pawnSelectionView brings up the highlight menu
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons) {
			PawnIconStandard icon = (PawnIconStandard)pawnIcon;

			//print("Icon: " + icon.pawnData + ":");
			if (!icon.pawnData.AtMaxLevel) {
				//print("Not interactable");
				icon.gameObject.SetActive(false);
			}
			else {
				//print("Interactable");
				//icon.gameObject.SetActive(true);
			}

			icon.onClick = (iconData) => {
				highlightMenu.SetActive(false);     // Disable then enable so the animation plays
				highlightedIcon = iconData;
				highlightMenu.SetActive(true);
				highlightMenu.transform.SetParent(iconData.transform, false);
			};
		}
	}

	private void OnDisable() {
		Reset();
	}


	public void Reset() {
		// foreach (GameObject icon in selectedIcons)
		// 	if (icon != null)
		// 		icon.SetActive(true);
		fuseMatIcon1.gameObject.SetActive(false);
		fuseMatIcon2.gameObject.SetActive(false);
		resultIcon.gameObject.SetActive(false);
		highlightMenu.SetActive(false);
		numSelected = 0;
		UpdatePawnSelectionViewInteractability();
		pawnSelectionView.Refresh();
		SetOnClick();
	}

	private void UpdatePawnSelectionViewInteractability() {
		pawnSelectionView.GetComponent<CanvasGroup>().interactable = numSelected < 2;
	}

	private void UpdateCostText() {
		Pawn pawn1 = fuseMatIcon1.pawnData;
		Pawn pawn2 = fuseMatIcon2.pawnData;
		moneyCostText.text = GetFusionCost(pawn1, pawn2).ToString();
		soulsCostText.text = GetFusionSoulsCost(pawn1, pawn2).ToString();
	}

	// Called by selectButton (in inspector, in the highlight menu)
	public void SelectPawnIcon() {
		PawnIcon fuseMatIcon;
		int index;
		if (!fuseMatIcon1.gameObject.activeInHierarchy) {
			fuseMatIcon = fuseMatIcon1;
			index = 0;
		}
		else {
			fuseMatIcon = fuseMatIcon2;
			index = 1;
		}

		selectedIcons[index] = highlightedIcon.gameObject;
		highlightedIcon.gameObject.SetActive(false);
		//highlightedIcon.GetComponent<Button>().interactable = false;
		highlightMenu.SetActive(false);
		fuseMatIcon.Init(highlightedIcon.pawnData);
		fuseMatIcon.gameObject.SetActive(true);
		numSelected++;
		if (numSelected > 2)
			numSelected = 2;
		UpdatePawnSelectionViewInteractability();
		UpdateCostText();
		//print("numSelected:" + numSelected);
	}

	// Called by infoButton (in inspector, in the highlight menu)
	public void OpenInfoPanel() {
		infoPanel.gameObject.SetActive(true);
		infoPanel.Init(highlightedIcon.pawnData);
	}

	public void ConfirmResult() {
		print("Refreshing page");
		Reset();
	}

	// ==========
	// Pawn Fusion Logic
	// ==========

	public void FusePawns() {
		GameManager gm = GameManager.instance;
		SaveModifier save = gm.save;
		if (numSelected != 2) {
			gm.DisplayAlert("You must select 2 heroes!");
			return;
		}
		Pawn pawn1 = selectedIcons[0].GetComponent<PawnIcon>().pawnData;
		Pawn pawn2 = selectedIcons[1].GetComponent<PawnIcon>().pawnData;
		Debug.Log("Pawn1:" + pawn1 + "\nPawn2:" + pawn2);

		if (CheckCanFusePawns(pawn1, pawn2)) {
			// Add the pawns to the save file
			Pawn pawn = GetFusedPawn(pawn1, pawn2);
			save.RemovePawn(pawn1.Id);
			save.RemovePawn(pawn2.Id);
			save.AddPawn(pawn);
			// Spend resources
			bool spentMoney = save.TrySpendMoney(GetFusionCost(pawn1, pawn2));
			bool spentSouls = save.TrySpendSouls(GetFusionSoulsCost(pawn1, pawn2));
			UnityEngine.Assertions.Assert.IsTrue(spentMoney && spentSouls);
			// Save Game
			gm.Save();
			// Update UI
			fuseMatIcon1.pawnData = null;
			fuseMatIcon2.pawnData = null;
			resultIcon.Init(pawn);
			StartCoroutine(AnimateFuseRoutine());
		}
		UpdateCostText();
	}

	private IEnumerator AnimateFuseRoutine() {
		resultIcon.gameObject.SetActive(true);
		animator.Play("Fuse");
		yield return new WaitForEndOfFrame();
		while (animator.GetCurrentAnimatorStateInfo (0).IsName ("Fuse"))
			yield return null;
		fuseMatIcon2.gameObject.SetActive(false);
		fuseMatIcon1.gameObject.SetActive(false);
	}

	// NOTE: Order of the condition checks here matters!
	public bool CheckCanFusePawns(Pawn pawn1, Pawn pawn2) {
		GameManager gm = GameManager.instance;
		if (pawn1.type != pawn2.type) {
			gm.DisplayAlert("Heroes must be the same type!");
			return false;
		}
		if (pawn1.tier != pawn2.tier) {
			gm.DisplayAlert("The heroes must be the same tier!");
			return false;
		}
		if (!CanAffordFusion(pawn1, pawn2)) {
			resourceReqAnim.CrossFade("Warning", 0);
			return false;
		}
		return true;
	}

	private bool CanAffordFusion(Pawn pawn1, Pawn pawn2) {
		SaveModifier save = GameManager.instance.save;
		if (save.money < GetFusionCost(pawn1, pawn2) ||
			save.souls < GetFusionSoulsCost(pawn1, pawn2))
			return false;
		return true;
	}

	private int GetFusionCost(Pawn pawn1, Pawn pawn2) {
		int pawn1Level = 0;
		int pawn2Level = 0;
		if (pawn1 != null)
			pawn1Level = pawn1.level;
		if (pawn2 != null)
			pawn2Level = pawn2.level;
		int cost = ((pawn1Level + pawn2Level) / 2) * 75;
		return cost;
	}

	private int GetFusionSoulsCost(Pawn pawn1, Pawn pawn2) {
		int pawn1Level = 0;
		int pawn2Level = 0;
		if (pawn1 != null)
			pawn1Level = pawn1.level;
		if (pawn2 != null)
			pawn2Level = pawn2.level;
		return 0;
	}

	private Pawn GetFusedPawn(Pawn pawn1, Pawn pawn2) {
		UnityEngine.Assertions.Assert.IsTrue(CheckCanFusePawns(pawn1, pawn2));
		Pawn fusedPawn = new Pawn(pawn1);
		// Fuse the boosts of the two pawns
		for (int i = 0; i < StatData.NUM_STATS; i ++) {
			fusedPawn.AddBoost(i, pawn2.boosts[i]);
			// fusedPawn.AddBoost(i, )
		}
		// Add two random boosts
		for (int i = 0; i < 2; i ++) {
			fusedPawn.AddBoost(Random.Range(0, StatData.NUM_STATS), 1);
		}
		return fusedPawn;
	}
}