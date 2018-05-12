using UnityEngine;

[System.Serializable]
public class Pawn
{
	//public const int T1_MIN_LEVEL = 1;
	//public const int T2_MIN_LEVEL = 5;
	//public const int T3_MIN_LEVEL = 8;
	//public const int MAX_LEVEL = 9;
	public const int T1_MAX_LEVEL = 5;
	public const int T2_MAX_LEVEL = 15;
	public const int T3_MAX_LEVEL = 20;

	public HeroType type;		// the type of the hero
	public int level;           // the level of the hero
	public int id		  { get; private set; }
	public int experience { get; private set; }
	public int MaxExperience {
		get {
			return (int)Mathf.Sqrt(level * 64);
		}
	}
	public int MaxLevel {
		get {
			switch (tier) {
				case HeroTier.tier1:
					return T1_MAX_LEVEL;
				case HeroTier.tier2:
					return T2_MAX_LEVEL;
				case HeroTier.tier3:
					return T3_MAX_LEVEL;
				default:
					return -1;
			}
		}
	}


	/// <summary>
	/// Gets the tier. 0 if T1, 1 if T2, 2 if T3.
	/// </summary>
	/// <value>The tier.</value>
	public HeroTier tier { get; private set; }

	//public bool atThresholdLevel {
	//	get {
	//		return level == T2_MIN_LEVEL - 1 ||
	//			level == T3_MIN_LEVEL - 1;
	//	}
	//}

	public Pawn(HeroType type, HeroTier tier, int level = 1)
	{
		this.type = type;
		this.tier = tier;
		this.level = level;
	}

	public void SetID(int id) {
		this.id = id;
	}

	/// <summary>
	/// Increases the experience of the pawn
	/// </summary>
	/// <returns><c>true</c>, if the pawn gained a level, <c>false</c> otherwise.</returns>
	/// <param name="amt">Amt.</param>
	public void AddExperience(int amt) {
		Debug.Log("Added experience to " + this);
		if (level > MaxLevel)
			return;
		experience += amt;
		while (experience > MaxExperience) {
			experience -= MaxExperience;
			level++;
			Debug.Log(this + " gained a level");
		}
	}

	public override string ToString()
	{
		return string.Format("[Pawn: type={0}, tier={1}, level={2}]", type.ToString(), tier.ToString(), level);
	}

	public static float DamageEquation(int level)
	{
		float answer = (0.08f * Mathf.Pow(level, 2.8f) + 5.3f);
		//if (tier == HeroTier.tier2)
		//	answer += 5f;
		//else if (tier == HeroTier.tier3)
		//	answer += 10f;
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

	//public string GetTimerID()
	//{
	//	return "Pawn:" + id;
	//}
}

public enum HeroTier {
	tier1 = 0,
	tier2 = 1,
	tier3 = 2
}
