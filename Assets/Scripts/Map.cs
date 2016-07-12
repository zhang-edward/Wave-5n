using UnityEngine;
using System.Collections.Generic;
using Utils;

public class Map : MonoBehaviour {

	public GameObject terrainPrefab;
	public Sprite[] terrainSprites;
	public GameObject[] terrainObjectPrefabs;
	public GameObject borderPrefab;

	private SpriteRenderer[,] terrainSpriteMap = new SpriteRenderer[size, size];
	private List<Vector2> openCells = new List<Vector2> ();
	private List<GameObject> terrainObjects = new List<GameObject>();

	public List<Vector2> OpenCells {
		get { return openCells; }
	}

	public const int size = 10;

	private const int EDGE_TILE = 2;
	private const int CORNER_TILE = 3;

	public Texture2D terrainMap, collidersMap, objectsMap;
	public int[,] terrain = new int[size, size];
	public int[,] colliders = new int[size, size];

	void Awake()
	{
		GetMaps ();
		InitSpriteMap ();
		InitMap ();
	}

	private void GetMaps()
	{
		for (int x = 0; x < size; x ++)
		{
			for (int y = 0; y < size; y ++)
			{
				int id = 0;
				if (terrainMap.GetPixel (x, y).r == 1)
				{
					id = 1;
					openCells.Add (new Vector2 (x, y));
				}
				terrain [y, x] = id;
				colliders [y, x] = (int)collidersMap.GetPixel (x, y).a;
				if (Random.value < objectsMap.GetPixel(x, y).a)
				{
					SpawnRandomObject (x, y);
				}
			}
		}

		int[,] temp = new int[size, size];
		IntArrayUtil.CopyArray (terrain, temp);
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

	private void SpawnRandomObject(int x, int y)
	{
		GameObject obj = Instantiate (terrainObjectPrefabs[Random.Range(0, terrainObjectPrefabs.Length)]);
		obj.transform.SetParent (this.transform);
		obj.transform.position = new Vector2 (x, y);
		terrainObjects.Add (obj);
	}

	public void InitMap()
	{
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				int id = terrain [y, x];
				SpriteRenderer sr = terrainSpriteMap [y, x];

				sr.flipX = false;
				sr.flipY = false;
				sr.transform.rotation = Quaternion.identity;

				if (id <= 1)
				{
					sr.sprite = terrainSprites [terrain [y, x]];
					if (colliders[y, x] == 1)
					{
						GameObject o = Instantiate (borderPrefab);
						o.transform.SetParent (this.transform);
						o.transform.position = new Vector2 (x, y);
					}
				}
				else
				{
					sr.sprite = terrainSprites [EvaluateEdgeId (id, ref sr)];
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

	private int SumNeighbors(int[,] arr, int x, int y, int id)
	{
		int sum = 0;
		if (IntArrayUtil.InBounds(arr, x, y + 1) && arr [y + 1, x] == id)
			sum += 1;
		if (IntArrayUtil.InBounds(arr, x + 1, y) && arr [y, x + 1] == id)
			sum += 2;
		if (IntArrayUtil.InBounds(arr, x, y - 1) && arr [y - 1, x] == id)
			sum += 4;
		if (IntArrayUtil.InBounds(arr, x - 1, y) && arr [y, x - 1] == id)
			sum += 8;

		return sum + 1;
	}

}
