using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIconBar : MonoBehaviour {

	public SpecialAbilityIcon specialAbilityIcon;
	public AbilityIcon[] abilityIcons;
	public GameObject iconPrefab;

	public Player player;

	private bool playerWasInitialized = false;
	private PlayerHero hero;

	private void Awake()
	{
		abilityIcons = new AbilityIcon[2];
		player = GetComponentInParent<GUIManager>().player;
	}

	void OnEnable()
	{
		player.OnPlayerInitialized += Init;
	}

	void OnDisabled()
	{
		player.OnPlayerInitialized -= Init;
		player.hero.OnAbilityFailed -= FlashRedAbilityIcon;
	}

	void Init()
	{
		playerWasInitialized = true;
		hero = player.hero;
		player.hero.OnAbilityFailed += FlashRedAbilityIcon;
		for (int i = 0; i < 2; i ++)
		{
			if (abilityIcons[i] != null) {
				abilityIcons [i].image.sprite = DataManager.GetHeroData(player.hero.heroType).abilityIcons [i];
			}
			else {
				GameObject o = Instantiate (iconPrefab);
				o.transform.SetParent (transform, false);
				abilityIcons [i] = o.GetComponent<AbilityIcon> ();
				abilityIcons [i].image.sprite = DataManager.GetHeroData(player.hero.heroType).abilityIcons [i];
			}
		}
		specialAbilityIcon.icon.sprite = DataManager.GetHeroData(player.hero.heroType).specialAbilityIcon;
		specialAbilityIcon.transform.parent.SetSiblingIndex (0);
	}

	void LateUpdate()
	{
		if (!playerWasInitialized)
			return;
		for (int i = 0; i < 2; i ++)
		{
			float percentCooldown = (hero.cooldownTimers[i] / hero.GetCooldownTime(i));
			//print (i + ": " + hero.GetCooldownTime (i));
			abilityIcons [i].SetCooldown (percentCooldown);
		}
		float percent = (hero.specialAbilityCharge / hero.specialAbilityChargeCapacity);
		specialAbilityIcon.SetCooldown (percent);
		if (hero.chargeMultiplier <= 1)
			specialAbilityIcon.SetMultiplierText("");
		else
			specialAbilityIcon.SetMultiplierText ("x" + hero.chargeMultiplier);
	}

	private void FlashRedAbilityIcon(int index)
	{
		abilityIcons[index].FlashHighlight(Color.red, 0.1f, 0.1f);
	}
}
