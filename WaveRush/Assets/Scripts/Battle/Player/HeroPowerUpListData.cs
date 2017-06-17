using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Player/PowerUpListData", order = 2)]
public class HeroPowerUpListData : ScriptableObject
{
	public HeroType heroName;
	public HeroPowerUp[] t1PPowerUps, t2PPowerUps, t3PPowerups;
	public HeroPowerUp[] t1SPowerUps, t2SPowerUps, t3SPowerups;


	// return the power up corresponding to the level param given
	public HeroPowerUp GetPowerUpFromLevel(int level)
	{
		// levels 8-9 are T3
		if (level >= HeroData.T3_MIN_LEVEL)
		{
			int i = level - HeroData.T3_MIN_LEVEL;
			return t3PPowerups[i];
		}
		// levels 5-7 are T2
		else if (level >= HeroData.T2_MIN_LEVEL)
		{
			int i = level - HeroData.T2_MIN_LEVEL;
			return t2PPowerUps[i];
		}
		// levels 0-4 are T1
		else
		{
			int i = level - HeroData.T1_MIN_LEVEL;
			return t1PPowerUps[i];
		}
	}
}