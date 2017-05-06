using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboMeter : MonoBehaviour {

	public Player player;
	private PlayerHero hero;

	public Text text;
	public Slider slider;

	private int oldCombo;

	void OnEnable()
	{
		player.OnPlayerInitialized += Init;
	}

	void OnDisable()
	{
		player.OnPlayerInitialized -= Init;
	}

	public void Init()
	{
		hero = player.GetComponentInChildren<PlayerHero> ();
		slider.maxValue = hero.maxComboTimer;
		text.gameObject.SetActive (false);
		slider.gameObject.SetActive (false);
	}

	void LateUpdate()
	{
		if (hero.combo > 0)
		{
			text.gameObject.SetActive (true);
			slider.gameObject.SetActive (true);
			text.text = "x" + hero.combo;
			// on combo changed
			if (hero.combo != oldCombo)
			{
				oldCombo = hero.combo;
				Animate ();
			}
		}
		else
		{
			text.gameObject.SetActive (false);
			slider.gameObject.SetActive (false);
		}
		slider.value = hero.comboTimer;
	}

	private void Animate()
	{
		text.gameObject.GetComponent<Animator> ().CrossFade ("pop_in", 0);
	}
}
