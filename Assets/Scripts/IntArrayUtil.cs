using System;
namespace Utils
{
	public static class IntArrayUtil
	{
		public static bool HasNeighbor(int[,] arr, int x, int y, int id)
		{
			if (InBounds (arr, x, y + 1) && arr [y + 1, x] == id)
				return true;
			else if (InBounds(arr, x + 1, y) && arr [y, x + 1] == id)
				return true;
			else if (InBounds(arr, x, y - 1) && arr [y - 1, x] == id)
				return true;
			else if (InBounds(arr, x - 1, y) && arr [y, x - 1] == id)
				return true;

			return false;
		}

		public static bool InBounds(int[,] arr, int x, int y)
		{
			return (x >= 0 && x < arr.GetLength (1)) &&
				(y >= 0 && y < arr.GetLength (0));
		}

		public static void CopyArray(int[,] src, int[,] dest)
		{
			for (int r = 0; r < src.GetLength(1); r ++)
			{
				for (int c = 0; c < src.GetLength(0); c ++)
				{
					dest [r, c] = src [r, c];
				}
			}
		}
	}
}
