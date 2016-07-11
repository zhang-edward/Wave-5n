using UnityEngine;
using System.Collections;
using Utils;

namespace Unused 
{
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
			terrainIdMap = caGenerator.Overlay (terrainIdMap, 0, -1, prob, noiseReduceReps, true, minPercent);

			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 1, 0, 0.005f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 2, 0, 0.005f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 3, 0, 0.01f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 4, 0, 0.01f, 0);

			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 1, -1, 0.1f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 2, -1, 0.1f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 3, -1, 0.15f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 4, -1, 0.15f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 5, -1, 0.05f, 0);
			terrainObjIdMap = caGenerator.SimpleOverlay (terrainIdMap, terrainObjIdMap, 6, -1, 0.05f, 0);

			/*		terrainIdMap = new int[,] {
			{-1, -1, -1, -1, -1},
			{-1, 0, 0, 0, -1},
			{-1, 0, 0, 0, -1},
			{-1, 0, 0, 0, -1},
			{-1, -1, -1, -1, -1}
		};*/
		}

		public void Reset()
		{
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					terrainIdMap [y, x] = -1;
					terrainObjIdMap [y, x] = 0;
				}
			}
		}


		/* ============== Terrain ID Map Tweaking ================= */

		public void TweakEdges()
		{
			// get an array of only -1s and 0s
			int[,] template = new int[size, size];
			IntArrayUtil.CopyArray (Terrain, template);

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					if (template[y, x] == 0)
					{
						if (x == 0 || x == size - 1 ||
							y == 0 || y == size - 1)
						{
							terrainIdMap [y, x] = -1;
						}
						else
						{
							int sum = SumNeighbors (template, x, y, -1);
							if (sum > 0)
								terrainIdMap [y, x] = sum;
						}
					}
					else if (template[y, x] == -1)
					{
						if (IntArrayUtil.HasNeighbor (template, x, y, 0))
							terrainIdMap [y, x] = Map.BORDER_TILE;
					}
				}
			}
		}

		// See http://www.saltgames.com/article/awareTiles/
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

			return sum;
		}

		/*	private bool HasNeighbor(int x, int y, int id)
	{
		if (inBounds (x, y + 1) && Terrain [y + 1, x] == id)
			return true;
		else if (inBounds(x + 1, y) && Terrain [y, x + 1] == id)
			return true;
		else if (inBounds(x, y - 1) && Terrain [y - 1, x] == id)
			return true;
		else if (inBounds(x - 1, y) && Terrain [y, x - 1] == id)
			return true;

		return false;
	}
*/
		/*	private bool inBounds(int x, int y)
		{
			return (x >= 0 && x < size) &&
				(y >= 0 && y < size);
		}*/
	}

		
}