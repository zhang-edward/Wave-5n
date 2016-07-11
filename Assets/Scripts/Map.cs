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

	private const int EDGE_TILE = 2;
	private const int CORNER_TILE = 3;


	void Awake()
	{
		InitSpriteMap ();
		GenerateMap ();
	}

	void GenerateMap()
	{
		mapGenerator.Reset ();
		mapGenerator.GenerateMap ();
		mapGenerator.TweakEdges ();
		ClearTerrainObjects ();
		for (int x = 0; x < size; x ++)
		{
			for (int y = 0; y < size; y ++)
			{
				int terrainId = mapGenerator.Terrain [y, x];
				// if terrainId > 1, the tile is an edge or corner piece
				if (terrainId > 1)
				{
					terrainSpriteMap [y, x].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
					terrainSpriteMap [y, x].flipX = false;		// reset any changes that evaluateEdgeId may have made
					terrainSpriteMap [y, x].flipY = false;		// (same as above)


					Debug.Log (x + ", " + y + ": " + terrainId);
					int id = EvaluateEdgeId (terrainId, ref terrainSpriteMap [y, x]);
					terrainSpriteMap [y, x].sprite = terrainSprites [id];
				}
				else
				{
//					Debug.Log (terrainId);
					terrainSpriteMap [y, x].sprite = terrainSprites [terrainId];
				}

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
		terrainObjects.Clear ();
	}

	// See http://www.saltgames.com/article/awareTiles/
	private int EvaluateEdgeId(int id, ref SpriteRenderer sr)
	{
		id--;
		int returnId;
		if (id % 3 == 0)
			returnId = CORNER_TILE;
		else
			returnId = EDGE_TILE;

		if (id == 3 || id == 2 || id == 6)
			sr.flipX = true;
		if (id == 3 || id == 9)
			sr.flipY = true;
		if (id == 1)
			sr.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
		if (id == 4)
			sr.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
		
/*		UnityEngine.Assertions.Assert.IsTrue (
			id == 1 ||
			id == 3 ||
			id == 2 ||
			id == 6 ||
			id == 4 ||
			id == 12 ||
			id == 8 ||
			id == 9
		);
*/
		return returnId;
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

