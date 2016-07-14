using System;
namespace Utils
{
	/// <summary>
	/// Some utility methods for 2D integer arrays
	/// </summary>
	public static class Int2DArrayUtil
	{
		/// <summary>
		/// Determines if a cell in the array has neighbor of the specified id.
		/// </summary>
		/// <returns><c>true</c> if has neighbor of the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="arr">Array to process.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="id">integer identifier value to check for.</param>
		public static bool HasNeighbor(int[,] arr, int x, int y, int id)
		{
			if (IsInBounds (arr, x, y + 1) && arr [y + 1, x] == id)
				return true;
			else if (IsInBounds(arr, x + 1, y) && arr [y, x + 1] == id)
				return true;
			else if (IsInBounds(arr, x, y - 1) && arr [y - 1, x] == id)
				return true;
			else if (IsInBounds(arr, x - 1, y) && arr [y, x - 1] == id)
				return true;

			return false;
		}

		/// <summary>
		/// Determines if a cell is in bounds for the specified array, x, y.
		/// </summary>
		/// <returns><c>true</c> if is in bounds for the specified array, x, y; otherwise, <c>false</c>.</returns>
		/// <param name="arr">Array to process.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static bool IsInBounds(int[,] arr, int x, int y)
		{
			return (x >= 0 && x < arr.GetLength (1)) &&
				(y >= 0 && y < arr.GetLength (0));
		}

		/// <summary>
		/// Copies the array <see cref="src"/> to the array <see cref="dest"/>.
		/// </summary>
		/// <param name="src">Source array.</param>
		/// <param name="dest">Destination array.</param>
		/// <remarks>
		/// <see cref="src"/> must be the same size as <see cref="dest"/>
		/// </remarks>
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
