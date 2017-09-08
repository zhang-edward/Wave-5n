using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Player/PowerUpListData", order = 2)]
public class HeroPowerUpListData : ScriptableObject
{
	public HeroType heroName;
	public HeroPowerUp[] t1PPowerUps, t2PPowerUps, t3PPowerups;

	public static int[] powerUpUnlockLevels = {1, 2, 3, 4, 5, 6, 7, 8, 9};		// the levels for which each powerup (powerups 1 - 9) are unlocked
	// TODO: Change to {3, 5, 7, 10, 12, 14, 16, 18, 20} later

	// return the power up corresponding to the level param given
	public HeroPowerUp GetPowerUpFromIndex(int level)
	{
		level += 1;
		HeroPowerUp powerUp;
		// levels 8-9 are T3
		if (level >= Pawn.T3_MIN_LEVEL)
		{
			int i = level - Pawn.T3_MIN_LEVEL;
			powerUp = t3PPowerups[i];
		}
		// levels 5-7 are T2
		else if (level >= Pawn.T2_MIN_LEVEL)
		{
			int i = level - Pawn.T2_MIN_LEVEL;
			powerUp = t2PPowerUps[i];
		}
		// levels 0-4 are T1
		else
		{
			int i = level - Pawn.T1_MIN_LEVEL;
			powerUp = t1PPowerUps[i];
		}
		return powerUp;
	}

	// Return the number of powerUps unlocked based on the level
	public static int GetNumPowerUpsUnlocked(int level)
	{
		for (int i = 0; i < powerUpUnlockLevels.Length; i ++)
		{
			if (powerUpUnlockLevels[i] > level)
			{
				return i;
			}
		}
		return powerUpUnlockLevels.Length;
	}
}