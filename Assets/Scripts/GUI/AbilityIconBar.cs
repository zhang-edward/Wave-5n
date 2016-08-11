using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIconBar : MonoBehaviour {

	public SpecialAbilityIcon specialAbilityIcon;
	private AbilityIcon[] abilityIcons;
	public GameObject iconPrefab;

	public Player player;

	private bool playerWasInitialized = false;
	private PlayerHero playerHero;

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
		playerHero = player.hero;
		abilityIcons = new AbilityIcon[playerHero.NumAbilities];
		//Debug.Log ("There are " + abilityIcons.Length + " player abilities");
		for (int i = 0; i < playerHero.NumAbilities; i ++)
		{
			GameObject o = Instantiate (iconPrefab);
			o.transform.SetParent (transform, false);
			abilityIcons [i] = o.GetComponent<AbilityIcon> ();
			abilityIcons [i].image.sprite = playerHero.icons [i];
		}
		int centerIndex = playerHero.NumAbilities / 2;
		specialAbilityIcon.image.sprite = playerHero.specialAbilityIcon;
		specialAbilityIcon.transform.SetSiblingIndex (centerIndex);
	}

	void LateUpdate()
	{
		if (!playerWasInitialized)
			return;
		for (int i = 0; i < playerHero.NumAbilities; i ++)
		{
			float percentCooldown = (playerHero.AbilityCooldowns[i] / playerHero.cooldownTime[i]);
			abilityIcons [i].SetCooldown (percentCooldown);
		}
		float percent = (playerHero.specialAbilityCharge / playerHero.specialAbilityChargeCapacity);
		specialAbilityIcon.SetCooldown (percent);
	}
}
