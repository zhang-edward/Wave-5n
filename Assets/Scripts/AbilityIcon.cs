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
		private PlayerAbility playerAbility;

		void Awake()
		{
			image = GetComponent<Image> ();
			player.OnPlayerInitialized += Init;
		}

		void OnDisabled()
		{
			player.OnPlayerInitialized -= Init;
		}

		void Init()
		{
			playerWasInitialized = true;
			playerAbility = player.ability;
			image.sprite = playerAbility.icon;
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
