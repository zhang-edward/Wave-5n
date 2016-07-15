using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityIcon : MonoBehaviour {

	private Image image;
	public RectTransform cooldownMask;
	public Player player;
	private PlayerAbility playerAbility;

	void Awake()
	{
		image = GetComponent<Image> ();
	}

	void Start()
	{
		playerAbility = player.ability;
		image.sprite = playerAbility.icon;
	}

	void LateUpdate()
	{
		float percentCooldown = (playerAbility.AbilityCooldown / playerAbility.cooldownTime);
		cooldownMask.sizeDelta = new Vector2 (16, percentCooldown * 16);
	}
}
