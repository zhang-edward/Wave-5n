using UnityEngine;
using UnityEngine.UI;

public class KnightShieldMeter : MonoBehaviour {

	public Image shieldTimerImage;
	public Image shieldHealthImage;
	public Sprite[] shieldHealthSprites;

	private KnightHero knight;
	
	public void Init(KnightHero knight) {
		this.knight = knight;
	}

	void Update() {
		if (knight.shieldTimer <= 0)
			shieldHealthImage.color = Color.clear;
		else {
			shieldHealthImage.color = Color.white;
			shieldHealthImage.sprite = shieldHealthSprites[KnightHero.SHIELD_MAXHEALTH - knight.shieldHealth];
		}
		shieldTimerImage.fillAmount = knight.shieldTimer / KnightHero.SHIELD_TIME;
	}
}