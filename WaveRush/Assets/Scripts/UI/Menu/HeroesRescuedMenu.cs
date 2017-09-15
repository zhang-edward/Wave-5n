using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroesRescuedMenu : MonoBehaviour
{
	public GameObject pawnIconPrefab;
	public PawnInfoPanel infoPanel;
	public Transform content;

	private List<GameObject> pawnIcons = new List<GameObject>();

	public void Init(List<Pawn> acquiredPawns)
	{
		StartCoroutine(InitRoutine(acquiredPawns));
	}

	private IEnumerator InitRoutine(List<Pawn> acquiredPawns)
	{
		int j = 0;  // track the pawnIcons list position
		for (int i = 0; i < acquiredPawns.Count; i++)      // iterate through the master list of pawns (may contain holes)
		{
			Pawn pawn = acquiredPawns[i];
			if (pawn != null)
			{
				if (j >= pawnIcons.Count)   // if we need more pawn icons, add new ones to the list
				{
					AddNewPawnIcon(pawn);
				}
				else
				{
					PawnIconStandard pawnIcon = pawnIcons[j].GetComponent<PawnIconStandard>();
					pawnIcon.Init(pawn);
					pawnIcon.onClick = (iconData) =>
					{
						infoPanel.gameObject.SetActive(true);
						infoPanel.Init(iconData.pawnData);
					};
				}
				j++;
			}
		}
		yield return null;	// Wait one frame to avoid any strange glitches
		StartCoroutine(AnimateIn(acquiredPawns.Count));
	}

	public bool AllIconsRevealed()
	{
		foreach (GameObject obj in pawnIcons)
		{
			PawnIconReveal pawnIcon = obj.GetComponent<PawnIconReveal>();
			if (!pawnIcon.revealed)
				return false;
		}
		return true;
	}

	private void AddNewPawnIcon(Pawn pawn)
	{
		GameObject o = Instantiate(pawnIconPrefab);
		o.transform.SetParent(content, false);
		o.SetActive(false);
		PawnIconStandard pawnIcon = o.GetComponent<PawnIconStandard>();
		pawnIcon.Init(pawn);
		pawnIcon.onClick = (iconData) =>
		{
			infoPanel.gameObject.SetActive(true);
			infoPanel.Init(iconData.pawnData);
		};
		pawnIcons.Add(o);

	}

	private IEnumerator AnimateIn(int numToShow)
	{
		for (int i = 0; i < numToShow; i ++)
		{
			GameObject o = pawnIcons[i];
			o.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
	}

	public void Reset()
	{
		foreach (GameObject o in pawnIcons)
		{
			o.SetActive(false);
		}
	}
}
