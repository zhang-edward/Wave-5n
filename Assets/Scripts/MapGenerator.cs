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
	}
}

