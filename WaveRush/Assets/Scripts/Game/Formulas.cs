using UnityEngine;
public static class Formulas {

	/** Formula for the amount of damage the player deals to enemies */
	public static float PlayerDamageFormula(int level, int tier = 0) {
		float answer = (0.2f * (Mathf.Pow(level + 3 * tier, 2)) + 10f);
		//if (tier == HeroTier.tier2)
		//	answer += 5f;
		//else if (tier == HeroTier.tier3)
		//	answer += 10f;
		return answer;
	}
	
	/** Formula for the amount of damage the enemies deal to the player */
	public static int EnemyDamageFormula(int damage, int levelDiff) {
		// Debug.Log("Level Diff: " + levelDiff);
		return Mathf.CeilToInt(damage * (-levelDiff / 5 + 10));
	}

	public static int ExperienceFormula(int level) {
		return 50 + level * level;
	}

	public static int PawnCost(Pawn pawn) {
		return 200 + Random.Range(0, 100);
	}
}
