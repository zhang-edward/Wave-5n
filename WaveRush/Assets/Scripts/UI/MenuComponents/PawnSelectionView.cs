﻿using UnityEngine;
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
		Pawn[] pawnsArr = GameManager.instance.saveGame.pawnWallet.pawns;  // retrieve the correct data
		Pawn[] extraPawnsArr = GameManager.instance.saveGame.pawnWallet.extraPawns;
		pawns = new Pawn[pawnsArr.Length + extraPawnsArr.Length];
		for (int i = 0; i < pawnsArr.Length; i ++)
		{
			pawns[i] = pawnsArr[i];
		}
		for (int i = 0; i < extraPawnsArr.Length; i ++)
		{
			pawns[i + pawnsArr.Length] = extraPawnsArr[i];
		}
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
		SortByHeroType();
	}


	/// <summary>
	/// Sorts the list of pawnIcons by the hero type, and also by level within each hero type.
	/// Also sets the sibling index so the order is rendered in any layout groups. Uses insertion sort.
	/// </summary>
	public void SortByHeroType()
	{
		for (int i = 1; i < pawnIcons.Count; i ++)
		{
			PawnIcon x = pawnIcons[i];
			int j = i - 1;
			while (j >= 0 && CompareHeroAndLevel(pawnIcons[j].pawnData, x.pawnData) < 0)
			{
				pawnIcons[j + 1] = pawnIcons[j];
				j--;
			}
			pawnIcons[j + 1] = x;
		}

		for (int i = 0; i < pawnIcons.Count; i ++)		// set the hierarchy position of each pawnIcon
		{
			pawnIcons[i].transform.SetSiblingIndex(i);
		}
	}

	/// <summary>
	/// Compares the hero type and level of two pawns.
	/// </summary>
	/// <returns> less than 0 if pawn1 has a lower level or heroType than pawn2, 0 if they are the same hero and level
	/// and greater than 0 if pawn1 has a higher level or heroType than pawn2. </returns>
	/// <param name="pawn1">Pawn1.</param>
	/// <param name="pawn2">Pawn2.</param>
	private int CompareHeroAndLevel(Pawn pawn1, Pawn pawn2)
	{
		if (pawn1.type == pawn2.type)
		{
			if (pawn1.level < pawn2.level)
			{
				return -1;
			}
			else if (pawn1.level > pawn2.level)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
		else
		{
			return pawn1.type.CompareTo(pawn2.type);
		}
	}


	/// <summary>
	/// Sorts the list of pawnIcons by level.
	/// Also sets the sibling index so the order is rendered in any layout groups. Uses insertion sort.
	/// </summary>
	public void SortByLevel()
	{
		for (int i = 1; i < pawnIcons.Count; i++)
		{
			PawnIcon x = pawnIcons[i];
			int j = i - 1;
			while (j >= 0 && pawnIcons[j].pawnData.level < x.pawnData.level)
			{
				pawnIcons[j + 1] = pawnIcons[j];
				j--;
			}
			pawnIcons[j + 1] = x;
		}
		for (int i = 0; i < pawnIcons.Count; i++)       // set the hierarchy position of each pawnIcon
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
