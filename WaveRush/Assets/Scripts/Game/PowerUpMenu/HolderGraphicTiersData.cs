using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Other/TiersGraphicData", order = 2)]
public class HolderGraphicTiersData : ScriptableObject
{
	public Sprite tier1Graphic, tier2Graphic, tier3Graphic;
}