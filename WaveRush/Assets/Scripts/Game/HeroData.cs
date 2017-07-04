using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Player/HeroData", order = 0)]
public class HeroData : ScriptableObject
{
	public HeroType heroType;
	public HeroPowerUpListData powerUpData;
	[TextArea]
	public string heroDescription, ability1Description, ability2Description, specialDescription;
	public AnimationSet t1Skin, t2Skin, t3Skin;
	public Sprite[] icons;
}
