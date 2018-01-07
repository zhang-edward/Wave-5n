using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Player/PowerUpData", order = 2)]
public class HeroPowerUpData : ScriptableObject
{
	public string powerUpName;
	public Sprite icon;

	[Space]
	[TextArea(5, 20)]
	public string description;
}