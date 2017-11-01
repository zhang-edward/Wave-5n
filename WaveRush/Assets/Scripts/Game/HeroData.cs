using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Player/HeroData", order = 0)]
public class HeroData : ScriptableObject
{
	public HeroType heroType;
	[Header("Hero Data")]
	public Sprite[] abilityIcons;
	public Sprite specialAbilityIcon;
	public HeroPowerUpListData powerUpData;
	[TextArea(3, 10)]
	public string heroDescription, ability1Description, ability2Description, specialDescription;
	public AnimationSet t1Skin, t2Skin, t3Skin;
	public Sprite[] icons;
}
