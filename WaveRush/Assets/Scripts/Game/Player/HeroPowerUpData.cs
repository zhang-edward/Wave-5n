using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Player/PowerUpData", order = 2)]
public class HeroPowerUpData : ScriptableObject
{
	public string powerUpName;
	public Sprite icon;

	public HeroPowerUp[] unlockable;
	public bool isRoot = true;

	public int maxStacks;
	public int cost;

	[Space]
	[TextArea]
	public string description;
	[TextArea]
	public string stackDescription;
}