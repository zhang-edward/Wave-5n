using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
	public class AbilityIconBar : MonoBehaviour {

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
		}

		void LateUpdate()
		{
			if (!playerWasInitialized)
				return;
			for (int i = 0; i < playerHero.NumAbilities; i ++)
			{
				float percentCooldown = (playerHero.AbilityCooldowns[i] / playerHero.cooldownTime[i]);
				abilityIcons [i].SetCoolDown (percentCooldown);
			}
		}
	}
}
