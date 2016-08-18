using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboMeter : MonoBehaviour {

	public Player player;
	private PlayerHero hero;

	public Text text;
	public Slider slider;

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
	}

	void LateUpdate()
	{
		if (hero.combo > 0)
		{
			text.gameObject.SetActive (true);
			text.text = "x" + hero.combo;
		}
		else
		{
			text.gameObject.SetActive (false);
		}
		slider.value = hero.comboTimer;
	}
}
