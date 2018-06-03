using UnityEngine;
public static class Formulas {

	public static float DamageFormula(int level) {
		float answer = (0.2f * (level * level) + 10f);
		//if (tier == HeroTier.tier2)
		//	answer += 5f;
		//else if (tier == HeroTier.tier3)
		//	answer += 10f;
		return answer;
	}

	public static int ExperienceFormula(int level) {
		return 50 + level * level;
	}
}
