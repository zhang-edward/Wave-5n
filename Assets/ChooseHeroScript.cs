using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChooseHeroScript : MonoBehaviour {

	public string hero;
	private Button button;

	void Awake()
	{
		button = GetComponent<Button> ();
	}

	void Start()
	{
		button.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		GameManager.instance.SelectHero (hero);
		GameManager.instance.GoToGameScene ();
	}
}
