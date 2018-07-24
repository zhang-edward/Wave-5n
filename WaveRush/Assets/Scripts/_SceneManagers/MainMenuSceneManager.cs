using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages MainMenu Scene State
/// </summary>
public class MainMenuSceneManager : MonoBehaviour
{
	public static MainMenuSceneManager instance;
	private GameManager gm;

	public TutorialDialogueManager tutorialDialogueManager;
	public MainMenu mainMenu;
	public Button pawnShopButton;

	void Awake()
	{
		// Make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);

		gm = GameManager.instance;
		gm.OnSceneLoaded += Init;
	}

	void OnEnable()
	{
		mainMenu.OnGoToBattle += GoToBattle;
	}

	void OnDisable()
	{
		mainMenu.OnGoToBattle -= GoToBattle;
	}

	private void Init() {
		gm.OnSceneLoaded -= Init; // Since this only runs once per scene
	}



	void GoToBattle() {
		GameManager.instance.GoToScene("HeroSelect");
	}
}
