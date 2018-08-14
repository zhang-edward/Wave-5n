using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpecialAbilityIcon : MonoBehaviour
{
	public SimpleAnimationPlayer holder;
	public Image cooldownMask;
	public Sprite holderDefaultSprite;
	public SimpleAnimation holderAbilityAvailableEffect;
	public SimpleAnimation holderCooledDownEffect;
	public Button button;
	public Player player;
	public Text chargeMultiplierText;
	[HideInInspector]
	public Image icon;
	[Header("Audio")]
	public AudioClip abilityReadySound;

	private bool playedCooledDownEffect = false;

	void Awake()
	{
		player = GetComponentInParent<GUIManager>().player;
		icon = GetComponent<Image>();
	}

	void Start()
	{
		button.onClick.AddListener (() => {
			player.hero.SpecialAbility();
		});
	}

	public void SetMultiplierText(string text)
	{
		chargeMultiplierText.text = text;
	}

	public void SetCooldown(float percent)
	{
		percent = 1 - percent;
		cooldownMask.fillAmount = percent;
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

		SoundManager.instance.PlayUISound(abilityReadySound);
	}

	private IEnumerator PlayFinishCooldownEffect()
	{
		holder.looping = false;
		holder.Play(holderCooledDownEffect);
		while (holder.isPlaying)
			yield return null;
		// play new anim
		holder.looping = true;
		holder.Play (holderAbilityAvailableEffect);
	}
}

