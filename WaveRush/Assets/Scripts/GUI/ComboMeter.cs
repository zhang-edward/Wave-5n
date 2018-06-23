using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboMeter : MonoBehaviour {

	public Player player;
	private PlayerHero hero;

	public TMPro.TMP_Text text;
	public Image fillImage;

	private int oldCombo;

 	void Awake()
	{
		player = GetComponentInParent<GUIManager>().player;
	}

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
		text.gameObject.SetActive (false);
		fillImage.fillAmount = 0;
		StartCoroutine(UpdateRoutine());
	}

	private IEnumerator UpdateRoutine()
	{
		for (;;)
		{
			//print("hero:" + hero.combo);
			if (hero.combo > 0)
			{
				text.gameObject.SetActive(true);
				fillImage.fillAmount = (float)hero.comboTimer / hero.maxComboTimer;
				text.text = "x" + hero.combo;
				
				// on combo changed
				if (hero.combo != oldCombo)
				{
					oldCombo = hero.combo;
					Animate();
				}
			}
			else
			{
				text.gameObject.SetActive(false);
			}
			yield return null;
		}
	}

	private void Animate()
	{
		text.gameObject.GetComponent<Animator> ().CrossFade ("pop_in", 0);
	}
}
