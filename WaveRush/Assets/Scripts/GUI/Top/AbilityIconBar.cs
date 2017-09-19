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

	void Awake()
	{
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
		abilityIcons = new AbilityIcon[hero.NumAbilities];
		//Debug.Log ("There are " + abilityIcons.Length + " player abilities");
		for (int i = 0; i < hero.NumAbilities; i ++)
		{
			GameObject o = Instantiate (iconPrefab);
			o.transform.SetParent (transform, false);
			abilityIcons [i] = o.GetComponent<AbilityIcon> ();
			abilityIcons [i].image.sprite = hero.icons [i];
		}
		specialAbilityIcon.icon.sprite = hero.specialAbilityIcon;
		specialAbilityIcon.transform.parent.SetSiblingIndex (0);
	}

	void LateUpdate()
	{
		if (!playerWasInitialized)
			return;
		for (int i = 0; i < hero.NumAbilities; i ++)
		{
			float percentCooldown = (hero.CooldownTimers[i] / hero.GetCooldownTime(i));
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
