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
	//public DailyHeroRewardButton dailyHeroRewardButton;

	public GameObject[] lockedObjects0;
	//public GameObject[] lockedObjects1;

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
			// Disable all locked objects
			foreach (GameObject o in lockedObjects0)
				o.SetActive(false);
			// Play tutorial dialogue
			tutorialDialogueManager.Init(0);
			// Play a tutorial dialogue upon pressing the "Battle" button
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
		//if (CheckStage(0, 1))
		//{
		//	// Disable all locked objects
		//	foreach (GameObject o in lockedObjects1)
		//		o.SetActive(false);
		//	// Play tutorial dialogue
		//	tutorialDialogueManager.Init(2);
		//	// Play a tutorial dialogue upon pressing the Daily Hero Reward button
		//	if (PlayerPrefs.HasKey(DailyHeroRewardButton.TUTORIAL_KEY))
		//	{
		//		dailyHeroRewardButton.EnableTutorial();
		//		PlayerPrefs.SetInt(DailyHeroRewardButton.TUTORIAL_KEY, 1);
		//	}
		//}
		//if (CheckStage(0, 2))
		//{
		//	// Disable all locked objects
		//	foreach (GameObject o in lockedObjects1)
		//		o.SetActive(false);
		//}
		//if (CheckStage(0, 3))
		//{
		//	tutorialDialogueManager.Init(3);
		//}
		gm.OnSceneLoaded -= Init; // Since this only runs once per scene
	}

	private bool CheckStage(int series, int stage)
	{
		return (gm.save.LatestSeriesIndex == series &&
		        gm.save.LatestStageIndex == stage);
	}

	void GoToBattle()
	{
		GameManager.instance.GoToScene("HeroSelect");
	}
}
