using UnityEngine;
using System.Collections;

public class PawnGenerator
{
	public static readonly int[][] PAWN_CRYSTAL_DROP_PROBABILITY_TABLE =
	{
		new int[] {10, 100, 60, 25, 15, 3, 3, 2, 2, 1},
		new int[] {10, 50, 100, 30, 20, 3, 3, 2, 2, 1},
		new int[] {10, 20, 50, 100, 30, 3, 3, 2, 2, 1},
		new int[] {10, 20, 30, 50, 100, 3, 3, 2, 2, 1},
		new int[] {1, 2, 4, 4, 20, 100, 60, 25, 8, 8},
		new int[] {1, 2, 4, 4, 16, 55, 100, 30, 8, 8},
		new int[] {1, 2, 4, 4, 12, 25, 60, 100, 8, 8},
		new int[] {0, 1, 1, 4, 4, 10, 20, 30, 100, 50},
		new int[] {0, 1, 1, 4, 4, 10, 20, 30, 75, 75}
	};

	public static Pawn GenerateCrystalDrop(int level)
	{
		Pawn pawn = new Pawn(HeroType.Knight);
		//HeroType type = (HeroType)Enum.GetValues(typeof(HeroType)).GetValue(UnityEngine.Random.Range(1, numHeroTypes));
		pawn.level = GetLevelFromCrystalLevel(level);
		return pawn;
	}

	private static int GetLevelFromCrystalLevel(int level)
	{
		int index = level - 1;
		if (index < 0) 
			index = 0;
		if (index > 8)
			index = 8;
		int[] probabilityTable = PAWN_CRYSTAL_DROP_PROBABILITY_TABLE[index];
		// Method similar to inverse cumulative distribution function: 
		// https://stackoverflow.com/questions/9956486/distributed-probability-random-number-generator
		int total = SumArray(probabilityTable);
		int random = Random.Range(0, total);
		int cumulativeChecker = 0;
		for (int i = 0; i < probabilityTable.Length; i ++)
		{
			cumulativeChecker += probabilityTable[i];
			if (random < cumulativeChecker)
			{
				return i;
			}
		}
		throw new UnityEngine.Assertions.AssertionException("PawnGenerator.cs",
		                                                    "PawnGenerator reached an impossible statement");
	}

	private static int SumArray(int[] arr)
	{
		int sum = 0;
		for (int i = 0; i < arr.Length; i ++)
		{
			sum += arr[i];
		}
		return sum;
	}
}
