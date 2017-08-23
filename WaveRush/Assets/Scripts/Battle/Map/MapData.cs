using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Map/MapData", order = 1)]
public class MapData : ScriptableObject
{
	public MapType mapType;
	[Space]
	public Sprite[] terrainSprites;
	public GameObject[] terrainObjectPrefabs;

	public GameObject bossSpawnPrefab;
	[Header("Background")]
	public GameObject[] edgeProps;
	public Sprite borderSprite;
	public Sprite cornerBorderSprite;
	public Color bgColor;

	[Header("Music")]
	public AudioClip musicLoop;
	public AudioClip musicIntro;
}

