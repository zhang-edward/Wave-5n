using UnityEngine;
using UnityEngine.UI;

public class HeroTypeIcon : MonoBehaviour {
	
	private const string NFI_KEY_PREFIX = "UNLOCKED_HERO_";

	public SimpleAnimationPlayerImage animPlayer;
	public Image stripe;
	public Sprite[] tierSprites;
	public bool updateUnlocked = true;
	public HeroType type { get; private set; }
	public HeroTier tier { get; private set; }

	public virtual void Init(HeroType type, HeroTier tier) {
		this.type = type;
		this.tier = tier;
		HeroData data = DataManager.GetHeroData(type);
		// Set portrait animation
		switch (tier) {
			case HeroTier.tier1:
				animPlayer.anim = data.t1Skins[0].GetAnimation("Default");
				break;
			case HeroTier.tier2:
				animPlayer.anim = data.t2Skins[0].GetAnimation("Default");
				break;
			case HeroTier.tier3:
				animPlayer.anim = data.t3Skins[0].GetAnimation("Default");
				break;
		}
		animPlayer.looping = true;
		animPlayer.Play();
		// Set stripe sprite
		stripe.sprite = tierSprites[(int)tier];
		// Set unlocked-ness
		if (updateUnlocked) {
			if (!GameManager.instance.save.UnlockedHeroes[(3 * (int)type) + (int)tier]) {
				animPlayer.image.color = Color.black;
			}
			else {
				animPlayer.image.color = Color.white;
			}
		}
	}
}
