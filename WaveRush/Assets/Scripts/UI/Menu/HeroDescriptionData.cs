using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Player/HeroDescription", order = 2)]
public class HeroDescriptionData : ScriptableObject
{
	public HeroType heroName;
	[TextArea]
	public string heroDescription, ability1Description, ability2Description, specialDescription;
}