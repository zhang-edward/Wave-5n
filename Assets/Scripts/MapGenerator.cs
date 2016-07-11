using UnityEngine;
using System.Collections;

public class MapGenerator
{
	private CAGenerator caGenerator = new CAGenerator ();
	private int[,] terrainIdMap;
	private int[,] terrainObjIdMap;

	// only for terrain[,]. Terrain Object generation params are hard-coded.
	public float prob;
	public int noiseReduceReps;
	public int size;
	public float minPercent;

	public int[,] Terrain {
		get { return terrainIdMap; }
	}

	public int[,] TerrainObjects {
		get { return terrainObjIdMap; }
	}

	public MapGenerator(int size, float prob = 0.45f, int noiseReduceReps = 5, float minPercent = 0.5f)
	{
		this.size = size;
		this.prob = prob;
		this.noiseReduceReps = noiseReduceReps;
		this.minPercent = minPercent;

		terrainIdMap = new int[size, size];
		terrainObjIdMap = new int[size, size];
	}

	public void GenerateMap()
	{
		terrainIdMap = caGenerator.Overlay (terrainIdMap, 1, 0, prob, noiseReduceReps, true, minPercent);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 1, 1, 0.005f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 2, 1, 0.005f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 3, 1, 0.01f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 4, 1, 0.01f, 0);

		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 1, 0, 0.1f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 2, 0, 0.1f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 3, 0, 0.15f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 4, 0, 0.15f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 5, 0, 0.05f, 0);
		terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 6, 0, 0.05f, 0);
	
/*		terrainIdMap = new int[,] {
			{0, 0, 0, 0, 0},
			{0, 1, 1, 1, 0},
			{0, 1, 1, 1, 0},
			{0, 1, 1, 1, 0},
			{0, 0, 0, 0, 0}
		};*/
	}

	public void Reset()
	{
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				terrainIdMap [y, x] = 0;
				terrainObjIdMap [y, x] = 0;
			}
		}
	}


	/* ============== Terrain ID Map Tweaking ================= */

	// See http://www.saltgames.com/article/awareTiles/
	public void TweakEdges()
	{
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				if (Terrain[y, x] == 1)
				{
					int sum = SumNeighbors (x, y);
					if (sum > 0)
						terrainIdMap [y, x] = sum;
					//Debug.Log(SumNeighbors(x, y));
				}
			}
		}
	}

	private int SumNeighbors(int x, int y)
	{
		int sum = 0;
		if (inBounds(x, y + 1) && Terrain [y + 1, x] == 0)
			sum += 1;
		if (inBounds(x + 1, y) && Terrain [y, x + 1] == 0)
			sum += 2;
		if (inBounds(x, y - 1) && Terrain [y - 1, x] == 0)
			sum += 4;
		if (inBounds(x - 1, y) && Terrain [y, x - 1] == 0)
			sum += 8;

		return sum + 1;
	}

	private bool inBounds(int x, int y)
	{
		return (x >= 0 && x < size) &&
			(y >= 0 && y < size);
	}
}

