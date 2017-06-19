using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PawnInfoPanel : MonoBehaviour
{
	public PawnIconStandard pawnIcon;
	public Text damageText, healthText, livesText;
	public Text infoText;

	public void Init(Pawn pawn)
	{
		pawnIcon.Init(pawn);
	}
}
