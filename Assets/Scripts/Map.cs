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
	public GameObject[] terrainObjectPrefabs;

	private SpriteRenderer[,] terrainSpriteMap = new SpriteRenderer[size, size];
	private List<GameObject> terrainObjects = new List<GameObject>();

	private MapGenerator mapGenerator = new MapGenerator(size);

	public const int size = 10;


	void Awake()
	{
		InitSpriteMap ();
		GenerateMap ();
	}

	void GenerateMap()
	{
		mapGenerator.GenerateMap ();
		ClearTerrainObjects ();
		for (int x = 0; x < size; x ++)
		{
			for (int y = 0; y < size; y ++)
			{
				terrainSpriteMap[y, x].sprite = terrainSprites [mapGenerator.Terrain [y, x]];

				if (mapGenerator.TerrainObjects[y, x] > 0)
				{
					//Debug.Log ("Terrain Objects matrix: " + terrainObjects [y, x]);
					GameObject obj = Instantiate (terrainObjectPrefabs[mapGenerator.TerrainObjects[y, x] - 1]);
					obj.transform.SetParent (this.transform);
					obj.transform.position = new Vector2 (x, y);
					terrainObjects.Add (obj);
				}
			}
		}
	}

	private void InitSpriteMap()
	{
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				GameObject o = Instantiate (terrainPrefab);
				o.transform.SetParent (this.transform);
				o.transform.position = new Vector2 (x, y);

				SpriteRenderer sr = o.GetComponent<SpriteRenderer> ();
				terrainSpriteMap [y, x] = sr;
			}
		}
	}

	private void ClearTerrainObjects()
	{
		foreach (GameObject o in terrainObjects)
			Destroy (o);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			GenerateMap ();
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

