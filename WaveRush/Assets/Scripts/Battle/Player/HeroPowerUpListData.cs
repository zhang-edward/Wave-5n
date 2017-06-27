using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Player/PowerUpListData", order = 2)]
public class HeroPowerUpListData : ScriptableObject
{
	public HeroType heroName;
	public HeroPowerUp[] t1PPowerUps, t2PPowerUps, t3PPowerups;


	// return the power up corresponding to the level param given
	public HeroPowerUp GetPowerUpFromLevel(int level)
	{
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
}