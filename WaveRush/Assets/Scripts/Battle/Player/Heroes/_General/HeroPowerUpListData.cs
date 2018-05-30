using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Player/PowerUpListData", order = 2)]
public class HeroPowerUpListData : ScriptableObject
{
	public HeroType heroName;
	public GameObject[] powerUps;

	public static readonly int[] powerUpUnlockLevels = {5, 11, 16, 20};		// the levels for which each powerup are unlocked

	// Return the number of powerUps unlocked based on the level
	public static int GetNumPowerUpsUnlocked(int level)
	{
		for (int i = 0; i < powerUpUnlockLevels.Length; i ++)
			if (powerUpUnlockLevels[i] > level)
				return i;
		return powerUpUnlockLevels.Length;
	}
}