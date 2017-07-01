using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroesRescuedMenu : MonoBehaviour
{
	public GameObject pawnIconPrefab;
	public PawnInfoPanel infoPanel;
	public Transform content;

	private List<GameObject> pawnIcons = new List<GameObject>();

	public void Init()
	{
		List<Pawn> acquiredPawns = BattleSceneManager.instance.acquiredPawns;
		foreach (Pawn pawn in acquiredPawns)
		{
			GameObject o = Instantiate(pawnIconPrefab);
			o.transform.SetParent(content, false);
			o.SetActive(false);
			PawnIconStandard pawnIcon = o.GetComponent<PawnIconStandard>();
			pawnIcon.Init(pawn);
			pawnIcon.onClick = (iconData) => {
				infoPanel.gameObject.SetActive(true);
				infoPanel.Init(iconData.pawnData);
			};
			pawnIcons.Add(o);
		}
		StartCoroutine(AnimateIn());
	}

	private IEnumerator AnimateIn()
	{
		foreach (GameObject o in pawnIcons)
		{
			o.SetActive(true);
			yield return new WaitForSeconds(0.2f);
		}
	}
}
