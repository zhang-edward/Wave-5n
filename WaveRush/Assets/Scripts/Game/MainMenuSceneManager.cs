using UnityEngine;
using System.Collections;

/// <summary>
/// Manages MainMenu Scene State
/// </summary>
public class MainMenuSceneManager : MonoBehaviour
{
	public static MainMenuSceneManager instance;
	private GameManager gm;

	public TutorialDialogueManager tutorialDialogueManager;
	public MainMenu mainMenu;

	public GameObject heroManagementOptions1;

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

	private void Init()
	{
		if (CheckStage(0, 0))
		{
			tutorialDialogueManager.Init(0);
			if (!tutorialDialogueManager.HasPlayedTutorial(1))
			{
				print("player has not played 1");
				mainMenu.OnGoToBattle -= GoToBattle;
				mainMenu.OnGoToBattle += () =>
				{
					print("button pressed");
					tutorialDialogueManager.Init(1);
					tutorialDialogueManager.dialogueView.onDialogueFinished += GoToBattle;
				};
			}
		}
	}

	private bool CheckStage(int series, int stage)
	{
		return (gm.saveGame.latestUnlockedSeriesIndex == series &&
				gm.saveGame.latestUnlockedStageIndex == stage);
	}

	void GoToBattle()
	{
		GameManager.instance.GoToScene("HeroSelect");
	}
}
