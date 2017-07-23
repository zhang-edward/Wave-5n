using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PawnInfoPanel : MonoBehaviour
{
	public PawnIconStandard pawnIcon;
	public TMP_Text damageText, healthText, livesText;
	public TMP_Text infoText;

	public void Init(Pawn pawn)
	{
		pawnIcon.Init(pawn);
	}
}
