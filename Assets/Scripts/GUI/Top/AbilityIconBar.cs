using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIconBar : MonoBehaviour {

	public SpecialAbilityIcon specialAbilityIcon;
	private AbilityIcon[] abilityIcons;
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
	}

	void Init()
	{
		playerWasInitialized = true;
		hero = player.hero;
		abilityIcons = new AbilityIcon[hero.NumAbilities];
		//Debug.Log ("There are " + abilityIcons.Length + " player abilities");
		for (int i = 0; i < hero.NumAbilities; i ++)
		{
			GameObject o = Instantiate (iconPrefab);
			o.transform.SetParent (transform, false);
			abilityIcons [i] = o.GetComponent<AbilityIcon> ();
			abilityIcons [i].image.sprite = hero.icons [i];
		}
		int centerIndex = hero.NumAbilities / 2;
		specialAbilityIcon.image.sprite = hero.specialAbilityIcon;
		specialAbilityIcon.transform.parent.SetSiblingIndex (centerIndex);
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
		specialAbilityIcon.SetMultiplierText (hero.chargeMultiplier);
	}
}
