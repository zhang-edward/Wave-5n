using UnityEngine;
using System.Collections.Generic;

public class PawnSelectionView : MonoBehaviour
{
	public GameObject pawnIconPrefab;
	public GameObject selectedContentPlaceholder;
	public Transform contentFolder;
	public Transform selectedContentFolder;

	private Pawn[] pawns;
	public List<PawnIcon> pawnIcons { get; private set; }

	public void Init()
	{
		pawnIcons = new List<PawnIcon>();
		Refresh();
	}

	public void Refresh()
	{
		this.pawns = GameManager.instance.saveGame.pawns;	// retrieve the correct data
		foreach (PawnIcon icon in pawnIcons)
			icon.gameObject.SetActive(false);
		int j = 0;									// track the pawnIcons list position
		for (int i = 0; i < pawns.Length; i ++)		// iterate through the master list of pawns (may contain holes)
		{
			Pawn pawn = pawns[i];
			if (pawn != null)
			{
				if (j >= pawnIcons.Count)			// if we need more pawn icons, add new ones to the list
				{
					AddNewPawnIcon(pawn);
				}
				else
				{
					pawnIcons[j].Init(pawn);        // if not, re-initialize the pawn icon
					pawnIcons[j].gameObject.SetActive(true);
				}
				j++;
			}
		}
	}

	public void SortByHeroType()
	{
		int numHeroTypes = System.Enum.GetNames(typeof(HeroType)).Length;	// get the number of hero types
		for (int heroType = 0; heroType < numHeroTypes; heroType++)			// for each hero type, find each pawn of that type in the list and add it to the beginning
		{
			for (int i = 0; i < pawnIcons.Count; i ++)
			{
				if (pawnIcons[i].pawnData.type == (HeroType)heroType)
				{
					pawnIcons.Insert(0, pawnIcons[i]);
					pawnIcons.RemoveAt(i);
				}
			}
		}
		for (int i = 0; i < pawnIcons.Count; i ++)		// set the hierarchy position of each pawnIcon
		{
			pawnIcons[i].transform.SetSiblingIndex(i);
		}
	}


	// ==== Helper methods ====

	private void AddNewPawnIcon(Pawn pawn)
	{
		GameObject o = Instantiate(pawnIconPrefab);
		o.transform.SetParent(contentFolder, false);        // set physical position
		PawnIcon pawnIcon = o.GetComponent<PawnIcon>();
		pawnIcon.Init(pawn);                                // initialize PawnIcon data
		pawnIcons.Add(pawnIcon);                            // add this object to the list for future manipulation
	}
}
