using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Player/HeroData", order = 0)]
public class HeroData : ScriptableObject
{
	[System.Serializable]
	private class BoostInfo {
		public float	strMin = 1.0f;
		public float	strMax = 1.5f;
		public float	vitMin = 40;
		public float	vitMax = 60;
		public float	chgMin = 1;
		public float	chgMax = 2;
		public float	dexMin = 0.02f;
		public float	dexMax = 0.1f;
		public float	critMin = 1.5f;
		public float	critMax = 2.5f;
		public float	luckMin = 0.05f;
		public float	luckMax = 0.15f;
	}
	public HeroType heroType;

	[Header("Hero Data")]
	public Sprite[] abilityIcons;
	public Sprite specialAbilityIcon;
	public HeroPowerUpListData powerUpData;

	[Header("Boosts")]
	[SerializeField] private BoostInfo boostInfo;

	[Header("Descriptions")]
	[TextArea(2, 5)]  public string[] tips;
	[TextArea(3, 10)] public string heroDescription;
	public string ability1Name, ability2Name, specialName;
	[TextArea(3, 10)] public string ability1Description,
		ability2Description,
		specialDescription,
		parryDescription;

	[Header("Graphics")]
	public AnimationSet[] t1Skins;
	public AnimationSet[] t2Skins;
	public AnimationSet[] t3Skins;

	public float[,] GetStatsMinMax() {
		float[,] arr = new float[,] {
			{boostInfo.strMin,  boostInfo.strMax},
			{boostInfo.vitMin,  boostInfo.vitMax},
			{boostInfo.chgMin,  boostInfo.chgMax},
			{boostInfo.dexMin,  boostInfo.dexMax},
			{boostInfo.critMin, boostInfo.critMax},
			{boostInfo.luckMin, boostInfo.luckMax},
		};
		return arr;
	}
}
