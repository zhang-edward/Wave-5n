using UnityEngine;
using System.Collections;

public class MapInfo : MonoBehaviour
{
	public Sprite[] terrainSprites;
	public GameObject[] terrainObjectPrefabs;
	public GameObject[] bgProps;
	public GameObject bossSpawnPrefab;
	public GameObject borderPrefab;
	[Header("Music")]
	public AudioClip musicLoop;
	public AudioClip musicIntro;

	public Color bgColor;

	void Awake()
	{
		Camera.main.backgroundColor = bgColor;
	}
}

