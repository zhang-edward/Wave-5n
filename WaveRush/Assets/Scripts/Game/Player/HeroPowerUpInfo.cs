using UnityEngine;
public class HeroPowerUpInfo : ScriptableObject
{
	public string powerUpName;
	public Sprite icon;

	public HeroPowerUpInfo[] unlockable;
	public HeroPowerUp prefab;
	public bool isRoot = true;

	public int maxStacks;
	public int cost;

	[TextArea]
	public string description;
	[TextArea]
	public string stackDescription;
}
