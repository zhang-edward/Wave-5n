[System.Serializable]
public class Pawn
{
	public const int T1_MIN_LEVEL = 1;
	public const int T2_MIN_LEVEL = 5;
	public const int T3_MIN_LEVEL = 8;

	public HeroType type;		// the type of the hero
	public int level;			// the level of the hero
	public int id { get; private set; }

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

	public void SetID(int id)
	{
		this.id = id;
	}

	public override string ToString()
	{
		return string.Format("[Pawn: type={0}, id={1}, level={2}]", type.ToString(), id, level);
	}
}
