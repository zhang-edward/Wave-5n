using UnityEngine;
using System.Collections.Generic;

public class MapGenerator {

	public class Cell
	{
		public int x;
		public int y;
		public Cell(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public override string ToString ()
		{
			return "" + x + ", " + y;
		}
	}

	/// <summary>
	/// Overlay the specified id, prob, noiseReductionReps and overlayOnTo.
	/// </summary>
	/// <param name="id">ID of tile to generate.</param>
	/// <param name="prob">Higher prob means denser blobs generated.</param>
	/// <param name="noiseReductionReps">Noise reduction reps. Less = more randomized/jagged</param>
	/// <param name="overlayOnTo">ID of tile to overlay on to. The generator will only replace
	/// tiles which have this ID.</param>
	/// <param name="contiguous">Whether or not the returned area should be one contiguous area</param>
	/// <param name="minPercent">The minimum area of that the largest area should have</param>
	public int[,] Overlay(int[,] arr, int id, int overlayOnTo, float prob, int noiseReductionReps, bool contiguous = true, float minPercent = 0.5f)
	{
		int area = arr.GetLength (0) * arr.GetLength (1);

		if (contiguous)
		{
			// do a simple overlay
			int[,] testArr = new int[arr.GetLength (0), arr.GetLength (1)];
			for(int x = 0; x < arr.GetLength(1); x ++)
				for(int y = 0; y < arr.GetLength(0); y ++)
					testArr[y, x] = arr[y, x];


			testArr = SimpleOverlay (testArr, id, overlayOnTo, prob, noiseReductionReps);

			// check a random contiguous area
			Cell c = GetRandomCellWithId (testArr, id);
			List<Cell> contiguousArea = GetContiguousCells (testArr, id, c.x, c.y);

			int i = 0;

			// if contiguous area is smaller than expected
			while (contiguousArea.Count < area * minPercent)
			{
				testArr = new int[arr.GetLength (0), arr.GetLength (1)];
				for(int x = 0; x < arr.GetLength(1); x ++)
					for(int y = 0; y < arr.GetLength(0); y ++)
						testArr[y, x] = arr[y, x];
				testArr = SimpleOverlay (testArr, id, overlayOnTo, prob, noiseReductionReps);

				// check a random contiguous area
				c = GetRandomCellWithId (testArr, id);
				contiguousArea = GetContiguousCells (testArr, id, c.x, c.y);

				// DEBUG
				i++;
				if (i > 100)
				{
					Debug.LogError ("Unexpected behavior in Overlay");
					break;
				}
			}
			Debug.Log (i);
			arr = SetCells (arr, contiguousArea, id);
		}
		return arr;
	}

	public int[,] SimpleOverlay(int[,] arr, int id, int overlayOnTo, float prob, int noiseReductionReps)
	{
		int[,] overlay = new int[arr.GetLength(0), arr.GetLength(1)];
		overlay = RandomizeArray(overlay, id, 0, prob);

		for (int i = 0; i < noiseReductionReps; i ++)
			overlay = ReduceNoiseRule2(overlay, id);
		for (int i = 0; i < noiseReductionReps / 2; i ++)
			overlay = ReduceNoise(overlay, id);

		for (int y = 0; y < arr.GetLength(0); y ++)
		{
			for (int x = 0; x < arr.GetLength(1); x ++)
			{
				if (arr[y, x] == overlayOnTo && overlay[y, x] == id)
					arr[y, x] = id;
			}
		}
		return arr;
	}

	// places random values into an array
	private int[,] RandomizeArray(int[,] arr, int one, int two, float prob)
	{
		for (int r = 0; r < arr.GetLength (0); r ++)
		{
			for (int c = 0; c < arr.GetLength(1); c ++)
			{
				if (Random.value < prob)
					arr[r, c] = one;
				else
					arr[r, c] = two;
			}
		}
		return arr;
	}

	// use cell automata to reduce noise
	private int[,] ReduceNoise(int[,] arr, int check)
	{
		int[,] answer = new int[arr.GetLength(0), arr.GetLength(1)];
		for (int y = 0; y < arr.GetLength(0); y ++)
		{
			for (int x = 0; x < arr.GetLength(1); x ++)
			{
				int wallNeighbors = CountNeighbors(arr, x, y, check);
				if (wallNeighbors >= 5)
					answer[y, x] = check;
			}
		}
		return answer;
	}

	// use cell automata to reduce noise, with an additional rule 
	// (see http://www.roguebasin.com/index.php?title=Cellular_Automata_Method_for_Generating_Random_Cave-Like_Levels)
	private int[,] ReduceNoiseRule2(int[,] arr, int check)
	{
		int[,] answer = new int[arr.GetLength(0), arr.GetLength(1)];
		for (int y = 0; y < arr.GetLength(0); y ++)
		{
			for (int x = 0; x < arr.GetLength(1); x ++)
			{
				int wallNeighbors = CountNeighbors(arr, x, y, check);
				if (wallNeighbors >= 5 || wallNeighbors == 0)
					answer[y, x] = check;
			}
		}
		return answer;
	}

	/// <summary>
	/// Counts the neighbors.
	/// </summary>
	/// <returns>The number of neighbors.</returns>
	/// <param name="arr">The array.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="check">The value to count the number of.</param>
	// note: this also checks the cell at x and y, not just neighbors
	private int CountNeighbors(int[,] arr, int x, int y, int check)
	{
		int counter = 0;
		for (int yy = -1; yy <= 1; yy ++)
		{
			for (int xx = -1; xx <= 1; xx ++)
			{
				// coords in array to be checked
				int xCheck = x + xx;
				int yCheck = y + yy;
				if (0 <= xCheck && xCheck < arr.GetLength(1) &&
					0 <= yCheck && yCheck < arr.GetLength(0) &&
					arr[y + yy, x + xx] == check)
					counter ++;
			}
		}
		return counter;
	}

	/// <summary>
	/// Gets a random cell in array 'arr' with the value 'check'.
	/// </summary>
	/// <returns>The coordinates of the cell.</returns>
	/// <param name="arr">Array to check through.</param>
	/// <param name="check">The value to check for.</param>
	private Cell GetRandomCellWithId(int[,] arr, int check)
	{
		int x = Random.Range (0, arr.GetLength (1));
		int y = Random.Range (0, arr.GetLength (0));
		int i = 0;
		while (arr[y, x] != check)
		{
			x = Random.Range (0, arr.GetLength (1));
			y = Random.Range (0, arr.GetLength (0));
			i++;
			if (i > 1000)
			{
				Debug.LogError("GetRandomCellWithId took too long (1000+ tries)");
				break;
			}
		}
		return new Cell(x, y);
	}

	/// <summary>
	/// Gets a list of contiguous cells of the same value 'check' (see CountRecursively below).
	/// </summary>
	/// <returns>The contiguous cells.</returns>
	/// <param name="arr">Arr.</param>
	/// <param name="check">Check.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public List<Cell> GetContiguousCells(int[,] arr, int check, int x, int y)
	{
		bool[,] visited = new bool[arr.GetLength (0), arr.GetLength (1)];
		return CountRecursively (arr, visited, x, y, check, new List<Cell>());
	}

	/// <summary>
	/// Gets the cells in the array 'arr' with the value 'check', recursively.
	/// </summary>
	/// <returns>The number of cells.</returns>
	/// <param name="arr">The original array to count.</param>
	/// <param name="visited">An array of visited cells.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="check">The number to check.</param>
	/// <param name="cells">The list of cells to add to upon additional loops.</param>
	private List<Cell> CountRecursively(int[,] arr, bool[,] visited, int x, int y, int check, List<Cell> cells)
	{
		if (!InBounds (arr, x, y) || visited [y, x])
			return cells;
		else
		{
			visited [y, x] = true;
			if (arr [y, x] == check)
			{
				cells.Add (new Cell (x, y));
				cells = CountRecursively (arr, visited, x + 1, y, check, cells);
				cells = CountRecursively (arr, visited, x - 1, y, check, cells);
				cells = CountRecursively (arr, visited, x, y + 1, check, cells);
				cells = CountRecursively (arr, visited, x, y - 1, check, cells);
//				Debug.Log (count);
			}
			return cells;
		}
	}

	private bool InBounds(int[,] arr, int x, int y)
	{
		return (x >= 0 && x < arr.GetLength (1)) &&
			(y >= 0 && y < arr.GetLength (0));
	}

	private int[,] SetCells(int[,] arr, List<Cell> cells, int id)
	{
		foreach (Cell c in cells)
		{
			arr [c.y, c.x] = id;
//			Debug.Log (c);
		}
		return arr;
	}
}
