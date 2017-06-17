[System.Serializable]
public class HeroData
{
	public const int T1_MIN_LEVEL = 0;
	public const int T2_MIN_LEVEL = 5;
	public const int T3_MIN_LEVEL = 8;

	public HeroType type;		// the type of the hero
	public int level;			// the level of the hero

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
}
