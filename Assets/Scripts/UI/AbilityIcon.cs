using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
	public class AbilityIcon : MonoBehaviour {

		private Image image;
		public RectTransform cooldownMask;

		public Player player;

		private bool playerWasInitialized = false;
		private PlayerHero playerAbility;

		void Awake()
		{
			image = GetComponent<Image> ();
		}

		void OnEnabled()
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
			playerAbility = player.hero;
			image.sprite = playerAbility.primaryAbilityIcon;
		}

		void LateUpdate()
		{
			if (!playerWasInitialized)
				return;
			float percentCooldown = (playerAbility.AbilityCooldown / playerAbility.cooldownTime);
			cooldownMask.sizeDelta = new Vector2 (16, percentCooldown * 16);
		}
	}
}
