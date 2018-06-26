using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroSelectMenu : MonoBehaviour  {

	/** Public members */
	public PawnSelectionView pawnSelectionView;
	public PawnInfoPanel pawnInfoPanel;
	public StageIcon selectedStageIcon;		// The stage icon
	public Button toBattleSceneButton;
	public Transform highlightMenu;			// The menu that opens when a pawn is highlighted in pawnSelectionView
	public Transform partyViewContent;		// The transform that holds the partyView
	public ObjectPooler partyPawnIconPool;

	[Header("Prefabs")]
	public GameObject partyPlaceholderPrefab;

	/** Private members */
	private PawnIconStandard highlightedIcon;		// The highlighted pawn icon in pawnSelectionView
	private List<PawnIconStandard> selectedIcons = new List<PawnIconStandard>();	// The selected pawn icons from pawnSelectionView
	private List<GameObject> partyPlaceholders = new List<GameObject>();
	private int numPawnsAllowed = 1;
	private int numPawnsInParty;

	public void Init()
	{
		/** Initialize pawn selection view */
		pawnSelectionView.Init(GameManager.instance.save.pawns, PawnSelectionView.PawnSelectionViewMode.Sorted);
		foreach (PawnIcon pawnIcon in pawnSelectionView.pawnIcons)
		{
			// Set the onClick
			PawnIconStandard pawnIconStandard = (PawnIconStandard)pawnIcon;
			pawnIconStandard.onClick = (PawnIconStandard iconData) => {
				// If this is not a selected pawnIcon, select it and update the info button
				if (!selectedIcons.Contains(iconData)) {
					if (partyPawnIconPool.GetAllActiveObjects().Count < numPawnsAllowed) {
						HighlightIcon(iconData);
					}
				}
				else Debug.LogError("Pawn icon should not be enabled! You probably didn't disable the pawn icon after selecting it to be in the party.");
			};
		}

		toBattleSceneButton.onClick.AddListener(() => {
			// Initialize the party
			List<GameObject> partyIcons = partyPawnIconPool.GetAllActiveObjects();
			Pawn[] party = new Pawn[partyIcons.Count];
			foreach (GameObject o in partyIcons) {
				PawnIconStandard pawnIcon = o.GetComponent<PawnIconStandard>();
				party[pawnIcon.transform.GetSiblingIndex()] = pawnIcon.pawnData;
			}
			GameManager.instance.selectedPawns = party;
		});
	}

	private void HighlightIcon(PawnIconStandard icon) {
		highlightedIcon = icon;
		highlightMenu.gameObject.SetActive(true);
		highlightMenu.SetParent(icon.transform, false);
	}

	public void AddToParty() {
		highlightedIcon.gameObject.SetActive(false);
		highlightMenu.gameObject.SetActive(false);
		selectedIcons.Add(highlightedIcon);
		// Initialize party icon
		GameObject o = partyPawnIconPool.GetPooledObject();
		PawnIconStandard icon = o.GetComponent<PawnIconStandard>();
		o.SetActive(true);
		o.transform.SetSiblingIndex(numPawnsInParty);
		icon.onClick += RemoveFromParty;
		icon.Init(highlightedIcon.pawnData);
		partyPlaceholders[numPawnsInParty].SetActive(false);
		// Increment count
		numPawnsInParty ++;
	}

	private void RemoveFromParty(PawnIconStandard icon) {
		icon.onClick = null;	// Reset the onClick when it is disabled (because it is re-added every time it is re-enabled)
		icon.gameObject.SetActive(false);
		// Re-enable icon in pawnSelectionView
		foreach (PawnIconStandard selectedIcon in selectedIcons) {
			if (selectedIcon.pawnData.Id == icon.pawnData.Id) {
				selectedIcon.gameObject.SetActive(true);
				selectedIcons.Remove(selectedIcon);
				break;
			}
		}
		// Decrement count
		numPawnsInParty --;
		partyPlaceholders[numPawnsInParty].SetActive(true);
	}

	public void OpenInfoPanel() {
		pawnInfoPanel.gameObject.SetActive(true);
		pawnInfoPanel.Init(highlightedIcon.pawnData);
	}

	void Update() {
		// We can only go to battle if we have at least one pawn selected
		toBattleSceneButton.interactable = partyPawnIconPool.GetAllActiveObjects().Count > 0;
	}

	void OnEnable() {
		// Re-initialize the stage icon
		GameManager gm = GameManager.instance;
		StageData selectedStage = gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex);
		selectedStageIcon.Init(selectedStage);
		// Create placeholders and pawnIcons to indicate max party size
		for (int i = 0; i < numPawnsAllowed; i ++) {
			// Placeholders
			if (i >= partyPlaceholders.Count) {
				GameObject o = Instantiate(partyPlaceholderPrefab);
				o.transform.SetParent(partyViewContent, false);
				partyPlaceholders.Add(o);
			}
			else {
				partyPlaceholders[i].SetActive(true);
			}
		}
	}

	void OnDisable() {
		foreach (GameObject o in partyPawnIconPool.GetAllActiveObjects())
			o.SetActive(true);
		foreach (GameObject o in partyPlaceholders)
			o.SetActive(false);
		foreach (PawnIconStandard icon in selectedIcons) 
			icon.gameObject.SetActive(true);
	}

	public void SetNumPawnsAllowed(int num) {
		numPawnsAllowed = num;
	}
}
