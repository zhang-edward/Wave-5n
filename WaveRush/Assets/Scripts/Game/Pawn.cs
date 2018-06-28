using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Pawn : System.IComparable<Pawn>
{
	public const int T1_MAX_LEVEL = 5;
	public const int T2_MAX_LEVEL = 15;
	public const int T3_MAX_LEVEL = 20;
	public const int MAX_BOOST_LEVEL = 10;

	public HeroType type;			// the hero type of this pawn
	public int level;           	// the current level of the pawn
	public int[] boosts;			// the boosts that this pawn has
	private int maxExperience;		// the experience cap for this pawn

	/** Properties */
	public int Id		  { get; private set; }
	public int Experience { get; private set; }
	public int MaxExperience { get { return maxExperience; } }

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

	/// <summary>
	/// Gets the tier. 0 if T1, 1 if T2, 2 if T3.
	/// </summary>
	/// <value>The tier.</value>
	public HeroTier tier { get; private set; }

#region Initialization
	public Pawn(HeroType type, HeroTier tier, int level = 1)
	{
		this.type = type;
		this.tier = tier;
		this.level = level;
		maxExperience = Formulas.ExperienceFormula(level);
		boosts = new int[StatData.NUM_STATS];
	}

	public Pawn(Pawn other) {
		this.type = other.type;
		this.tier = other.tier;
		this.level = other.level;
		this.Experience = other.Experience;
		maxExperience = Formulas.ExperienceFormula(level);
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
			maxExperience = Formulas.ExperienceFormula(level);
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

	public override string ToString() {
		return string.Format("[Pawn: type={0}, tier={1}, level={2}]", type.ToString(), tier.ToString(), level);
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
#endregion
}

public enum HeroTier {
	tier1 = 0,
	tier2 = 1,
	tier3 = 2
}
