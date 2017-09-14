using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Map/MapData", order = 1)]
public class MapData : ScriptableObject
{
	[System.Serializable]
	public struct TerrainObjectData
	{
		public Sprite sprite;	// The sprite for this terrain object
		public bool flattened;	// Whether this object sticks up out of the ground or not (controls the sorting layer)
	}

	public MapType mapType;
	[Space]
	public Sprite[] terrainSprites;
	public TerrainObjectData[] terrainObjects;

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

