using UnityEngine;
public static class Formulas {

	/** Formula for the amount of damage the player deals to enemies */
	public static float PlayerDamage(int level, int tier = 0) {
		float answer = (0.2f * (Mathf.Pow(level + 3 * tier, 2)) + 10f);
		//if (tier == HeroTier.tier2)
		//	answer += 5f;
		//else if (tier == HeroTier.tier3)
		//	answer += 10f;
		return answer;
	}
	
	/** Formula for the amount of damage the enemies deal to the player */
	public static int EnemyDamage(int damage, int levelDiff) {
		// Debug.Log("Level Diff: " + levelDiff);
		return Mathf.CeilToInt(damage * (-levelDiff / 5 + 10));
	}

	public static int ExperienceFormula(int level, int tier) {
		float tierScalingFactor = tier * 0.5f;
		float baseExperience = 50 + level * level;
		return (int)(baseExperience + (baseExperience * tierScalingFactor));
	}

	public static int StageExperience(int stageLevel, int numWaves, int maxPartySize, int actualPartySize) {
		// Base experience formula
		float baseExperience = Mathf.Sqrt(Formulas.ExperienceFormula(stageLevel, 0)) * 4;
		// Base experience is based on a 5-wave stage standard
		float partySizeScaleFactor = Mathf.Sqrt(maxPartySize / actualPartySize);
		// Scale experience gained appropriately if the player used less heroes than the max party size
		float waveNumScaleFactor = (float)numWaves / 5;
		Debug.Log(string.Format("Base experience: {0}, partySizeScaleFactor: {1}, waveNumScaleFactor: {2}", baseExperience, partySizeScaleFactor, waveNumScaleFactor));
		return (int)(baseExperience * partySizeScaleFactor * waveNumScaleFactor);
	}

	public static void PawnCost(Pawn pawn, out int money, out int souls) {
		int baseCost = 700;
		int levelCost = pawn.level * pawn.level * 20;
		int tierScaleFactor = (int)pawn.tier + 1;
		money = (baseCost + levelCost) * tierScaleFactor;
		souls = (2 * pawn.GetNumBoosts());
	}


	public static int BossSoulsDrop(int level) {
		float numSoulsRaw = (level / 5 - 0.5f);
		int ans = (int)numSoulsRaw;
		if (Random.value < (numSoulsRaw - (int)numSoulsRaw))
			ans ++;
		return ans;
	}

	public static void BoxCost(out int money, out int souls) {
		int baseCost = 1000;
		int formula = (int)(0 + (0.1f * (GameManager.instance.save.money - 500)));
		money = Mathf.Max(baseCost, formula);
		souls = 10;
	}
}
