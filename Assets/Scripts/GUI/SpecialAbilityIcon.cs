using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpecialAbilityIcon : MonoBehaviour
{
	public SimpleAnimationPlayer holder;
	public RectTransform cooldownMask;
	public Sprite holderDefaultSprite;
	public SimpleAnimation holderAbilityAvailableEffect;
	public SimpleAnimation holderCooledDownEffect;
	public Button button;
	public Player player;
	public Image image;

	private bool playedCooledDownEffect = false;

	void Awake()
	{
		image = GetComponent<Image>();
	}

	void Start()
	{
		button.onClick.AddListener (() => {
			player.hero.SpecialAbility ();
		});
	}

	public void SetCooldown(float percent)
	{
		percent = 1 - percent;
		cooldownMask.sizeDelta = new Vector2 (16, percent * 16);
		// if cooled down completely (ability is ready to use again)
		if (percent <= 0)
		{
			if (!playedCooledDownEffect)
				FinishCooldownEffect ();
			return;
		}
		holder.sr.sprite = holderDefaultSprite;
		playedCooledDownEffect = false;
	}

	private void FinishCooldownEffect()
	{
		playedCooledDownEffect = true;
		StartCoroutine (PlayFinishCooldownEffect ());
	}

	private IEnumerator PlayFinishCooldownEffect()
	{
		holder.anim = holderCooledDownEffect;
		holder.looping = false;
		holder.Play();
		while (holder.isPlaying)
			yield return null;
		// play new anim
		holder.anim = holderAbilityAvailableEffect;
		holder.looping = true;
		holder.Play ();
	}
}

