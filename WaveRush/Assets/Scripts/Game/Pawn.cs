using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Pawn : System.IComparable<Pawn>
{
	public const int T1_MAX_LEVEL = 5;
	public const int T2_MAX_LEVEL = 15;
	public const int T3_MAX_LEVEL = 20;
	public const int MAX_BOOST_LEVEL = 9;

	public HeroType type;			// the hero type of this pawn
	public int level;           	// the current level of the pawn
	public int[] boosts;			// the boosts that this pawn has
	public HeroTier tier  { get; private set; }
	public int Id		  { get; private set; }
	public int Experience { get; private set; }

	/** Properties */
	public int MaxExperience { get; private set; }

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

	public bool AtMaxLevel {
		get { return level == MaxLevel; }
	}

#region Initialization
	public Pawn() {
		type = HeroType.Knight;
		tier = HeroTier.tier1;
		level = 1;
		MaxExperience = Formulas.ExperienceFormula(level);
		boosts = new int[StatData.NUM_STATS];
	}

	public Pawn(HeroType type, HeroTier tier, int level = 1)
	{
		this.type = type;
		this.tier = tier;
		this.level = level;
		MaxExperience = Formulas.ExperienceFormula(level);
		boosts = new int[StatData.NUM_STATS];
	}

	public Pawn(Pawn other) {
		this.type = other.type;
		this.tier = other.tier;
		this.level = other.level;
		this.Experience = other.Experience;
		MaxExperience = Formulas.ExperienceFormula(level);
		boosts = new int[StatData.NUM_STATS];
		other.boosts.CopyTo(this.boosts, 0);
	}

	public void SetID(int id) {
		this.Id = id;
	}
#endregion
#region Experience
	/// <summary>
	/// Increases the experience of the pawn
	/// </summary>
	/// <returns>How many levels the pawn gained</returns>
	/// <param name="amt">Amt.</param>
	public int AddExperience(int amt) {
		int numLevelsGained = 0;
		if (level >= MaxLevel)
			return 0;
		Experience += amt;
		while (Experience > MaxExperience) {
			Experience -= MaxExperience;
			level++;
			numLevelsGained++;
			MaxExperience = Formulas.ExperienceFormula(level);
			if (level >= MaxLevel)
			{
				Experience = 0;
				return numLevelsGained;
			}
		}
		return numLevelsGained;
	}

	public void LoseExperience(int amt) {
		Experience -= amt;
		if (Experience < 0)
			Experience = 0;
	}

	public static int GetMaxExperience(int level) {
		return Formulas.ExperienceFormula(level);
	}
#endregion
#region Boosts
	public bool AddBoost(int statIndex, int amt) {
		if (boosts[statIndex] < MAX_BOOST_LEVEL) {
			boosts[statIndex] = Mathf.Min(MAX_BOOST_LEVEL, boosts[statIndex] + amt);
			return true;
		}
		else
			return false;
	}

	public float[] GetStatsArray() {
		float[,] statsMinMax = DataManager.GetHeroData(type).GetStatsMinMax();
		float[] ans = new float[StatData.NUM_STATS];
		for (int i = 0; i < StatData.NUM_STATS; i ++) {
			ans[i] = Mathf.Lerp(statsMinMax[i, 0], statsMinMax[i, 1], (float)boosts[i] / MAX_BOOST_LEVEL);
		}
		return ans;
	}

	public int GetNumBoosts() {
		int sum = 0;
		for (int i = 0; i < boosts.Length; i ++) {
			sum += boosts[i];
		}
		return sum;
	}
#endregion
#region Misc
	public AnimationSet GetAnimationSet() {
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

	public override string ToString() {
		return string.Format("[Pawn({0}): type={1}, tier={2}, level={3}, experience={4}, ]", 
							 Id, type.ToString(), tier.ToString(), level, Experience);
	}

	public int CompareTo(Pawn other) {
		if (other == null) 
			return 1;

		if (type.CompareTo(other.type) != 0)
			return type.CompareTo(other.type);
		if (tier.CompareTo(other.tier) != 0)
			return tier.CompareTo(other.tier);
		else if (level.CompareTo(other.level) != 0)
			return level.CompareTo(other.level);
		return 0;
	}

	/// <summary>
	///	Converts the string to a pawn. 
	/// String has format: "IIEEERTTLLBBBBBB"
	/// I = id
	/// E = experience
	/// R = tier
	/// T = type
	/// L = level
	/// B = boosts
	/// </summary>
	public static Pawn String2Pawn(string str) {
		Debug.Log("Converting string: " + str);
		Pawn pawn = new Pawn();
		pawn.Id = 			System.Convert.ToInt32(str.Substring(0, 2));
		pawn.Experience = 	System.Convert.ToInt32(str.Substring(2, 3));
		pawn.tier = 		(HeroTier)System.Convert.ToInt32(str.Substring(5, 1));
		pawn.type = 		(HeroType)System.Convert.ToInt32(str.Substring(6, 2));
		pawn.level = 		System.Convert.ToInt32(str.Substring(8, 2));
		int boostsStartIndex = 10;
		for (int i = 0; i < StatData.NUM_STATS; i ++) {
			int stringIndex = boostsStartIndex + i;
			pawn.boosts[i] = System.Convert.ToInt32(str[stringIndex]);
		}
		Debug.Log(pawn.ToString());
		return pawn;
	}

	/// <summary>
	/// Converts a pawn to a string
	/// String has format: "IIEEERTTLLBBBBBB"
	/// I = id
	/// E = experience
	/// R = tier
	/// T = type
	/// L = level
	/// B = boosts
	/// </summary>
	public static string Pawn2String(Pawn pawn) {
		Debug.Log("Converting pawn: " + pawn.ToString());
		string str = "";
		str += Num2String(pawn.Id, 2);
		str += Num2String(pawn.Experience, 3);
		str += Num2String((int)pawn.tier, 1);
		str += Num2String((int)pawn.type, 2);
		str += Num2String(pawn.level, 2);
		for (int i = 0; i < StatData.NUM_STATS; i ++) {
			str += Num2String(pawn.boosts[i], 1);
		}
		Debug.Log(str);
		return str;
	}

	private static string Num2String(int num, int strLength) {
		string str = num.ToString();
		str = str.PadLeft(strLength, '0');
		return str;
	}

#endregion
}

public enum HeroTier {
	tier1 = 0,
	tier2 = 1,
	tier3 = 2
}
