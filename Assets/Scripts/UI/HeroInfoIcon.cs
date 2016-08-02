using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroInfoIcon : MonoBehaviour {

	public string heroName;
	private Button button;

	void Awake()
	{
		button = GetComponent<Button> ();
	}

	void Start()
	{
		//button.onClick.AddListener(OnClick);
	}

/*	private void OnClick()
	{
		GameManager.instance.SelectHero (hero);
	}*/
}
