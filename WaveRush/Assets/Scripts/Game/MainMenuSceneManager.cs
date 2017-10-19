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
		if (gm.saveGame.latestUnlockedSeriesIndex == 0 && 
		    gm.saveGame.latestUnlockedStageIndex == 0)
		{
			tutorialDialogueManager.Init(0);
		}
	}
}
