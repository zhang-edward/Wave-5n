[System.Serializable]
public class HeroConsumable
{
	public HeroType hero;		// the type of the hero
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
