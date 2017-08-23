using UnityEngine;
using System.Collections.Generic;
using Utils;

public class Map : MonoBehaviour {

	public GameObject terrainPrefab;
	public GameObject colliderPrefab;

	public MapData data { get; private set; }

	[Header("Map Info")]
	public MapData[] mapData;
	public MapType chosenMap;

	public GameObject bossSpawn { get; set; }

	private SpriteRenderer[,] terrainSpriteMap = new SpriteRenderer[size, size];
	private List<GameObject> terrainObjects = new List<GameObject>();

	private List<Vector2> openCells = new List<Vector2> ();	// for use in EnemyManager
	public List<Vector2> OpenCells {
		get { return openCells; }
	}

	public SpriteRenderer[] borderSprites;
	public SpriteRenderer[] cornerBorderSprites;
	public const int size = 20;
	public float bgPropsBufferMin = 2;
	public float bgPropsBufferMax = 8;
	public int bgPropsDensity = 25;

	// ids
	private const int EDGE_TILE = 2;
	private const int CORNER_TILE = 3;

	public Texture2D terrainMap, collidersMap, objectsMap;
	public Vector3 bossSpawnPosition;
	public int[,] terrain = new int[size, size];
	public int[,] colliders = new int[size, size];

	[Header("Folders")]
	public Transform terrainFolder;
	public Transform collidersFolder;
	public Transform objectsFolder;
	public Transform bgFolder;

	public Vector3 CenterPosition {
		get {
			return new Vector3 (size / 2, size / 2);
		}
	}

	public void GenerateMap()
	{
		data = GetMapInfo ();

		SetBorderSprite();
		GetIntegerMaps ();
		InitSpriteMap ();
		CreateMap ();
	}
	
	private void SetBorderSprite()
	{
		foreach (SpriteRenderer sr in borderSprites)
		{
			sr.sprite = data.borderSprite;
		}
		foreach(SpriteRenderer sr in cornerBorderSprites)
		{
			sr.sprite = data.cornerBorderSprite;
		}
	}

	private MapData GetMapInfo()
	{
		foreach (MapData data in mapData)
		{
			if (data.mapType == chosenMap)
				return data;
		}
		throw new UnityEngine.Assertions.AssertionException ("Map.cs:", "MapInfo not found");
	}

	private void GetIntegerMaps()
	{
		for (int x = 0; x < size; x ++)
		{
			for (int y = 0; y < size; y ++)
			{
				// process terrainMap
				int id = 0;
				if (terrainMap.GetPixel (x, y).r == 1)
				{
					id = 1;
					openCells.Add (new Vector2 (x, y));	// add to a list of empty cells
				}
				terrain [y, x] = id;
				// process collidersMap
				colliders [y, x] = (int)collidersMap.GetPixel (x, y).a;
				// process objectsMap
				if (Random.value < objectsMap.GetPixel(x, y).a)
				{
					CreateRandomObject (x, y);
				}
			}
		}

		int[,] temp = new int[size, size];
		Int2DArrayUtil.CopyArray (terrain, temp);
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				if (temp[y, x] == 1)
				{
					int sum = SumNeighbors (temp, x, y, 0);
					if (sum > 1)
						terrain [y, x] = sum;
				}
			}
		}
	}

	private void CreateRandomObject(int x, int y)
	{
		// get a random object
		GameObject obj = Instantiate (data.terrainObjectPrefabs[Random.Range(0, data.terrainObjectPrefabs.Length)]);
		obj.transform.SetParent (objectsFolder);
		obj.transform.position = new Vector2 (x, y);
		terrainObjects.Add (obj);
	}

	private void CreateBossSpawn()
	{
		GameObject obj = Instantiate (data.bossSpawnPrefab);
		obj.transform.SetParent (objectsFolder);
		obj.transform.position = bossSpawnPosition;
		terrainObjects.Add (obj);
		bossSpawn = obj;
	}

	public void CreateMap()
	{
		SoundManager.instance.PlayMusicLoop (data.musicLoop, data.musicIntro);
		CreateBossSpawn ();
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				// get terrain id
				int id = terrain [y, x];
				SpriteRenderer sr = terrainSpriteMap [y, x];
				// reset some variables
				sr.flipX = false;
				sr.flipY = false;
				sr.transform.rotation = Quaternion.identity;
				// process id
				if (id <= 1)
				{
					// get sprites
					sr.sprite = data.terrainSprites [terrain [y, x]];
					if (colliders[y, x] == 1)
					{
						GameObject o = Instantiate (colliderPrefab);
						o.transform.SetParent (collidersFolder);
						o.transform.position = new Vector2 (x, y);
					}
				}
				// if id > 1, the terrain is an edge or corner tile
				else
					sr.sprite = data.terrainSprites [EvaluateEdgeId (id, ref sr)];
			}
		}
	}

	private void InitSpriteMap()
	{
		SpawnBGProps ();
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				GameObject o = Instantiate (terrainPrefab);
				o.transform.SetParent (terrainFolder);
				o.transform.position = new Vector2 (x, y);

				SpriteRenderer sr = o.GetComponent<SpriteRenderer> ();
				terrainSpriteMap [y, x] = sr;
			}
		}
	}

	private void SpawnBGProps()
	{
		for (int i = 0; i < bgPropsDensity; i ++)
		{
			float randOffset = Random.Range (bgPropsBufferMin, bgPropsBufferMax);
			GameObject o = Instantiate(data.edgeProps[Random.Range(0, data.edgeProps.Length)]);
			o.transform.position = new Vector3 (-randOffset,
				Random.Range (0, Map.size + bgPropsBufferMax));
			o.transform.SetParent (bgFolder);
		}
		for (int i = 0; i < bgPropsDensity; i ++)
		{
			float randOffset = Random.Range (bgPropsBufferMin, bgPropsBufferMax);
			GameObject o = Instantiate(data.edgeProps[Random.Range(0, data.edgeProps.Length)]);
			o.transform.position = new Vector3 (Map.size + randOffset,
				Random.Range (0, Map.size + bgPropsBufferMax));
			o.transform.SetParent (bgFolder);
		}
		for (int i = 0; i < bgPropsDensity; i ++)
		{
			float randOffset = Random.Range (bgPropsBufferMin, bgPropsBufferMax);
			GameObject o = Instantiate(data.edgeProps[Random.Range(0, data.edgeProps.Length)]);
			o.transform.position = new Vector3 (Random.Range (0, Map.size + bgPropsBufferMax),
				-randOffset);
			o.transform.SetParent (bgFolder);
		}
		for (int i = 0; i < bgPropsDensity; i ++)
		{
			float randOffset = Random.Range (bgPropsBufferMin, bgPropsBufferMax);
			GameObject o = Instantiate(data.edgeProps[Random.Range(0, data.edgeProps.Length)]);
			o.transform.position = new Vector3 (Random.Range(0, Map.size + bgPropsBufferMax),
				Map.size + randOffset);
			o.transform.SetParent (bgFolder);
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

	private int SumNeighbors(int[,] arr, int x, int y, int id)
	{
		int sum = 0;
		if (Int2DArrayUtil.IsInBounds(arr, x, y + 1) && arr [y + 1, x] == id)
			sum += 1;
		if (Int2DArrayUtil.IsInBounds(arr, x + 1, y) && arr [y, x + 1] == id)
			sum += 2;
		if (Int2DArrayUtil.IsInBounds(arr, x, y - 1) && arr [y - 1, x] == id)
			sum += 4;
		if (Int2DArrayUtil.IsInBounds(arr, x - 1, y) && arr [y, x - 1] == id)
			sum += 8;

		return sum + 1;
	}

	public bool WithinOpenCells(Vector3 pos)
	{
		Vector3 roundedPos = new Vector3 (Mathf.RoundToInt (pos.x),
			                     Mathf.RoundToInt (pos.y),
			                     Mathf.RoundToInt (pos.z));
		if (openCells.Contains(roundedPos))
		{
			return true;
		}
		return false;
	}
}
