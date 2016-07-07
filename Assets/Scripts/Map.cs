using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	/*[System.Serializable]
	public class TerrainObjectIds
	{
		public string name;
		public Sprite sprite;
		private int identifier;

		public int id {
			get {
				return identifier;
			}
			set {
				identifier = value;
			}
		}
	}*/

	//public TerrainObjectIds[] terrainObjectIds;
	//private Dictionary<string, int> terrainObject;

	public GameObject terrainPrefab;
	public Sprite[] terrainSprites;
	public GameObject[] terrainObjects;

	private SpriteRenderer[,] terrainSpriteMap;

	private MapGenerator mapGenerator = new MapGenerator ();

	[Range(0.4f, 1f)]
	public float prob;
	[Range(0, 10)]
	public int noiseReduceReps;

	private int[,] terrain;
	public int size;
	public float minPercent;

	void Awake()
	{
		InitMap ();
	}

	void InitMap()
	{
		terrain = new int[size, size];
		terrainSpriteMap = new SpriteRenderer[size, size];

		terrain = mapGenerator.Overlay (terrain, 1, 0, prob, noiseReduceReps, true, minPercent);
		for (int x = 0; x < terrain.GetLength(1); x ++)
		{
			for (int y = 0; y < terrain.GetLength(0); y ++)
			{
				GameObject o = Instantiate (terrainPrefab);
				o.transform.SetParent (this.transform);
				o.transform.position = new Vector2 (x, y);

				SpriteRenderer sr = o.GetComponent<SpriteRenderer> ();
				terrainSpriteMap [y, x] = sr;
				sr.sprite = terrainSprites [terrain [y, x]];
			}
		}
	}

	void GenerateNewMap()
	{
		terrain = new int[size, size];
		terrain = mapGenerator.Overlay (terrain, 1, 0, prob, noiseReduceReps, true, minPercent);
		for (int x = 0; x < terrain.GetLength(1); x ++)
		{
			for (int y = 0; y < terrain.GetLength(0); y ++)
			{
				terrainSpriteMap[y, x].sprite = terrainSprites [terrain [y, x]];
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			GenerateNewMap ();
		}

/*		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			//Debug.Log (terrain[Mathf.RoundToInt (mousePosition.y), Mathf.RoundToInt (mousePosition.x)]);
			int count = mapGenerator.GetContiguousCells (terrain, 1,
				Mathf.RoundToInt (mousePosition.x),
				Mathf.RoundToInt (mousePosition.y)).Count;
			Debug.Log (count);
		}*/
	}

}

