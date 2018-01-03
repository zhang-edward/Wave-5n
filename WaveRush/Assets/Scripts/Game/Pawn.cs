using UnityEngine;

[System.Serializable]
public class Pawn
{
	public const int T1_MIN_LEVEL = 1;
	public const int T2_MIN_LEVEL = 5;
	public const int T3_MIN_LEVEL = 8;
	public const int MAX_LEVEL = 9;

	public HeroType type;		// the type of the hero
	public int level;           // the level of the hero
	public int id { get; private set; }
	public float unlockTime;	// time, in realTime seconds, for which this hero is unlocked

	public HeroTier tier {
		get {
			if (level <= 4)
				return HeroTier.tier1;
			else if (level <= 7)
				return HeroTier.tier2;
			else if (level > 7)
				return HeroTier.tier3;
			else
				return 0;
		}
	}

	public bool atThresholdLevel {
		get {
			return level == T2_MIN_LEVEL - 1 ||
				level == T3_MIN_LEVEL - 1;
		}
	}

	public Pawn(HeroType type)
	{
		this.type = type;
	}

	public void SetID(int id)
	{
		this.id = id;
	}

	public override string ToString()
	{
		return string.Format("[Pawn: type={0}, id={1}, level={2}]", type.ToString(), id, level);
	}

	public static float DamageEquation(Pawn pawn)
	{
		float answer = (0.08f * Mathf.Pow(pawn.level, 2.8f) + 5.3f);
		if (pawn.level >= T2_MIN_LEVEL)
			answer += 5f;
		if (pawn.level >= T3_MIN_LEVEL)
			answer += 5f;
		return answer;
	}

	public static float DamageEquation(int level)
	{
		float answer = (0.08f * Mathf.Pow(level, 2.8f) + 5.3f);
		if (level >= T2_MIN_LEVEL)
			answer += 5f;
		if (level >= T3_MIN_LEVEL)
			answer += 5f;
		return answer;
	}

	public AnimationSet GetAnimationSet()
	{
		AnimationSet answer;
		HeroData data = DataManager.GetHeroData(type);
		switch(tier)
		{
			case HeroTier.tier1:
				answer = data.t1Skin;
				break;
			case HeroTier.tier2:
				answer = data.t2Skin;
				break;
			case HeroTier.tier3:
				answer = data.t3Skin;
				break;
			default:
				throw new UnityEngine.Assertions.AssertionException("Pawn.cs", "Pawn is out of the level range?");
		}
		return answer;
	}

	public string GetTimerID()
	{
		return "Pawn:" + id;
	}
}

public enum HeroTier {
	tier1,
	tier2,
	tier3
}
