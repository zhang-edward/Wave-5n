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

	private void Init()
	{
		if (CheckStage(0, 0))
		{
			tutorialDialogueManager.Init(0);
		}
		if (CheckStage(0, 1))
		{
			
		}
	}

	private bool CheckStage(int series, int stage)
	{
		return (gm.saveGame.latestUnlockedSeriesIndex == series &&
				gm.saveGame.latestUnlockedStageIndex == stage);
	}
}
