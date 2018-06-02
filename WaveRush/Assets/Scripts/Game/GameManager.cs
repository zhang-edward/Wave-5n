using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public const string SCENE_TUTORIAL = "Tutorial1";
	public const string SCENE_MAINMENU = "MainMenu";
	public const string SCENE_BATTLE = "Game";
	public const float LOADING_SCREEN_SPEED = 8;
	public static GameManager instance;

	/** Public Fields */
	[Header("Quest Manager")]
	public QuestManager questManager;
	//[Header("Save Game")]
	private SaveGame sg;
	public SaveModifier save;	// Use this to modify the save game. Allows events to be handled properly
	[Header("Selected Items for Battle Scene")]
	public Pawn selectedPawn;
	public int selectedSeriesIndex;
	public int selectedStageIndex;
	[Header("Data")]
	public StageCollectionData regularStages;
	public ScoreManager scoreManager;
	[Header("Persistent UI")]
	public Image loadingOverlay;
	public GameObject debugPanel;
	public MessageText debugText;
	public MessageText alertText;
	public Text fpsDisplay;

	/** Delegates and Events */
	public delegate void GameStateUpdate();
	public GameStateUpdate OnSceneLoaded;
	public GameStateUpdate OnAppLoaded;
	public GameStateUpdate OnAppClosed;
	public GameStateUpdate OnTimersUpdated;


	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
		{
			Destroy(this.gameObject);
			return;		// Don't execute any of the following code if this is not the singleton instance
		}
		DontDestroyOnLoad (this);

		SaveLoad.Load (out sg);
		save = new SaveModifier(sg);
		//InitPawnTimers();
		questManager.Init();
		loadingOverlay.gameObject.SetActive(false);
	}

	void Start()
	{
		if (OnSceneLoaded != null)
			OnSceneLoaded();

		Application.targetFrameRate = 60;
		StartCoroutine(FPS());
		//ScheduleSimple();
	}

	void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			debugPanel.SetActive(!debugPanel.activeInHierarchy);
		}
#endif
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			//print("Application Paused");
			//PlayerPrefs.SetString(RealtimeTimerCounter.LAST_CLOSED_KEY, System.DateTime.Now.ToString());
			if (OnAppClosed != null)
				OnAppClosed();
		}
		else
		{
			//print("Application Unpaused");
			if (OnAppLoaded != null)
				OnAppLoaded();
			if (OnTimersUpdated != null)
				OnTimersUpdated();
		}
	}

	private void OnApplicationQuit()
	{
		//PlayerPrefs.SetString(RealtimeTimerCounter.LAST_CLOSED_KEY, System.DateTime.Now.ToString());
		print("Application Quit");
		if (OnAppClosed != null)
			OnAppClosed();
		//SavePawnTimers();
		SaveLoad.Save(sg);
	}

	public void GoToScene(string sceneName, float fadeInSpeed = 1f)
	{
		//didInitializeGameScene = false;
		Time.timeScale = 1;
		StartCoroutine (LoadScene(sceneName, fadeInSpeed));
		ObjectPooler.objectPoolers.Clear ();
	}

	private IEnumerator LoadScene(string scene, float fadeInSpeed)
	{
		//Debug.Log ("Loading scene");
		StartCoroutine(ActivateLoadingScreen ());
		while (loadingOverlay.color.a < 0.95f)
			yield return null;
		AsyncOperation async = SceneManager.LoadSceneAsync (scene);
		Assert.IsNotNull (async);

		while (!async.isDone)
			yield return null;
		StartCoroutine(DeactivateLoadingScreen (fadeInSpeed));

		// On finished scene loading
		if (OnSceneLoaded != null)
			OnSceneLoaded();

		while (loadingOverlay.color.a > 0.05f)
			yield return null;
	}

	// ==========
	/** Stages and Series methods */
	// ==========

	/// <summary>
	/// Returns the number of stages unlocked for the specified series. Does NOT return the last unlocked index!!
	/// </summary>
	/// <returns>The number of stages unlocked.</returns>
	/// <param name="seriesName">Series name.</param>
	public int NumStagesUnlocked(string seriesName)
	{
		if (!IsSeriesUnlocked(seriesName))
			return 0;

		// if the series is not the latest, then that means it has been completed
		if (!GetLatestSeries().seriesName.Equals(seriesName))
			return GetSeries(seriesName).stages.Length;
		else
			return save.LatestStageIndex + 1;
	}

	public void UnlockNextStage()
	{
		if (save.LatestStageIndex < regularStages.series[save.LatestSeriesIndex].stages.Length - 1)
			sg.saveDict[SaveGame.LATEST_UNLOCKED_STAGE_INDEX_KEY]++;
		else
		{
			sg.saveDict[SaveGame.LATEST_UNLOCKED_SERIES_INDEX_KEY]++;
			sg.saveDict[SaveGame.LATEST_UNLOCKED_STAGE_INDEX_KEY] = 0;
		}
	}

	public StageSeriesData GetLatestSeries()
	{
		return regularStages.series[save.LatestSeriesIndex];
	}

	public bool IsSeriesUnlocked(string seriesName)
	{
		List<StageSeriesData> unlockedSeries = GetAllUnlockedSeries();
		foreach(StageSeriesData series in unlockedSeries)
		{
			if (seriesName.Equals(series.seriesName))
				return true;
		}
		return false;
	}

	public StageData GetStage(int seriesIndex, int stageIndex)
	{
		return regularStages.series[seriesIndex].stages[stageIndex];
	}

	private List<StageSeriesData> GetAllUnlockedSeries()
	{
		List<StageSeriesData> answer = new List<StageSeriesData>();
		for (int i = 0; i <= save.LatestSeriesIndex; i ++)
		{
			answer.Add(regularStages.series[i]);
		}
		return answer;
	}

	private StageSeriesData GetSeries(string seriesName)
	{
		foreach (StageSeriesData series in regularStages.series)
		{
			if (series.seriesName.Equals(seriesName))
			{
				return series;
			}
		}
		return null;
	}

	public void UpdateScores(int enemiesKilled, int wavesSurvived, int maxCombo)
	{
		HeroType type = selectedPawn.type;
		scoreManager.SubmitScore(type, new ScoreManager.Score (enemiesKilled, wavesSurvived, maxCombo));
		SaveLoad.Save(sg);
	}

	// ==========
	// Loading Screen
	// ==========

	private IEnumerator ActivateLoadingScreen()
	{
		loadingOverlay.gameObject.SetActive(true);
		Color initialColor = Color.clear;
		Color finalColor = Color.black;
		float t = 0;
		while (loadingOverlay.color.a < 0.95f)
		{
			loadingOverlay.color = Color.Lerp (initialColor, finalColor, t * LOADING_SCREEN_SPEED / 2);
			t += Time.deltaTime;
			yield return null;
		}
		loadingOverlay.color = finalColor;
	}

	private IEnumerator DeactivateLoadingScreen(float fadeInSpeed)
	{
		Color initialColor = Color.black;
		Color finalColor = Color.clear;
		float t = 0;
		while (loadingOverlay.color.a > 0.05f)
		{
			loadingOverlay.color = Color.Lerp (initialColor, finalColor, t * LOADING_SCREEN_SPEED * fadeInSpeed);
			t += Time.deltaTime;
			yield return null;
		}
		loadingOverlay.color = finalColor;
		loadingOverlay.gameObject.SetActive(false);
	}

	// ==========
	// Save and Load
	// ==========

	public void PrepareSaveFile()
	{
		sg.highScores = scoreManager.highScores;
	}

	public void LoadSaveFile()
	{
		scoreManager.highScores = sg.highScores;
	}

	public void DeleteSaveData()
	{
		sg = new SaveGame ();
		int foo;
		sg.pawnWallet.AddPawn(new Pawn(HeroType.Knight, HeroTier.tier1), out foo);
		LoadSaveFile ();
		SaveLoad.Save(sg);
	}

	public void Save() {
		SaveLoad.Save(sg);
	}

	public void DisplayAlert(string message)
	{
		alertText.SetColor(Color.white);
		alertText.Display(new MessageText.Message(message, 1, 0, 2f, 1f, Color.white));
	}

	/** Debug Text UI */

	public void DisplayDebugMessage(string message)
	{
		debugText.SetColor (Color.white);
		debugText.Display (new MessageText.Message(message, 1, 0, 2f, 1f, Color.white));
	}

	/// <summary>
	/// FPS counter
	/// </summary>
	private IEnumerator FPS()
	{
		string fps;
		for (;;)
		{
			// Capture frame-per-second
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(1.0f);
			float timeSpan = Time.realtimeSinceStartup - lastTime;
			int frameCount = Time.frameCount - lastFrameCount;

			// Display it

			fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
			fpsDisplay.text = fps;
		}
	}
}