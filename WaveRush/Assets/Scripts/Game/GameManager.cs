using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public const string BattleSceneName = "Game";

	public const float LOADING_SCREEN_SPEED = 12; 
	public static GameManager instance;

	[Header("Realtime Timer")]
	public RealtimeTimerManager timerManager;
	[Header("Quest Manager")]
	public QuestManager questManager;
	[Header("Save Game")]
	public SaveGame saveGame;
	[Header("Selected Items for Battle Scene")]
	public Pawn selectedPawn;
	public int selectedSeriesIndex;
	public int selectedStageIndex;
	[Header("Data")]
	public StageCollectionData regularStages;
	public ScoreManager scoreManager;
	public Wallet wallet;
	[Header("Persistent UI")]
	public Image loadingOverlay;
	public GameObject debugPanel;
	public MessageText debugText;
	public Text fpsDisplay;

	public delegate void GameStateUpdate();
	public GameStateUpdate OnSceneLoaded;


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

		SaveLoad.Load ();
		InitRealtimeTimers();
		questManager.Init();
	}

	public void InitRealtimeTimers()
	{
		timerManager.AddTimer("Debug", saveGame.debugTimer);
		foreach (Pawn pawn in saveGame.pawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				timerManager.AddTimer("Pawn:" + pawn.id, pawn.unlockTime);
			}
		}
		foreach (Pawn pawn in saveGame.extraPawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				timerManager.AddTimer("Pawn:" + pawn.id, pawn.unlockTime);
			}
		}
		timerManager.UpdateTimersSinceLastClosed();
	}

	public void ScheduleSimple()
	{
		Assets.SimpleAndroidNotifications.NotificationManager.Send(System.TimeSpan.FromSeconds(5), "Simple notification", "Customize icon and color", new Color(1, 0.3f, 0.15f));
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
			//Debug.Log("Pause");
			PlayerPrefs.SetString(RealtimeTimerManager.LAST_CLOSED_KEY, System.DateTime.Now.ToString());
			SaveTimers();
		}
		else
		{
			//Debug.Log("Resume");
			timerManager.UpdateTimersSinceLastClosed();
		}
	}

	private void SaveTimers()
	{
		foreach (Pawn pawn in saveGame.pawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				RealtimeTimer timer = timerManager.GetTimer("Pawn:" + pawn.id);
				pawn.unlockTime = timer.timer;
			}
		}
		foreach (Pawn pawn in saveGame.extraPawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				RealtimeTimer timer = timerManager.GetTimer("Pawn:" + pawn.id);
				pawn.unlockTime = timer.timer;
			}
		}
		SaveLoad.Save();
	}

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetString(RealtimeTimerManager.LAST_CLOSED_KEY, System.DateTime.Now.ToString());
		saveGame.debugTimer = timerManager.GetTimer("Debug").timer;
		SaveTimers();
		SaveLoad.Save();
	}

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

	public void GoToScene(string sceneName)
	{
		//didInitializeGameScene = false;
		Time.timeScale = 1;
		StartCoroutine (LoadScene(sceneName));
		ObjectPooler.objectPoolers.Clear ();
	}

	public IEnumerator LoadScene(string scene)
	{
		//Debug.Log ("Loading scene");
		StartCoroutine(ActivateLoadingScreen ());
		while (loadingOverlay.color.a < 0.95f)
			yield return null;
		AsyncOperation async = SceneManager.LoadSceneAsync (scene);
		Assert.IsNotNull (async);

		while (!async.isDone)
			yield return null;
		StartCoroutine(DeactivateLoadingScreen ());

		while (loadingOverlay.color.a > 0.5f)
			yield return null;
		// On finished scene loading
		if (OnSceneLoaded != null)
			OnSceneLoaded();
	}

	// ==========
	// Stages and Series methods
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
			return saveGame.latestUnlockedStageIndex + 1;
	}

	public void UnlockNextStage()
	{
		if (saveGame.latestUnlockedStageIndex < regularStages.series[saveGame.latestUnlockedSeriesIndex].stages.Length - 1)
			saveGame.latestUnlockedStageIndex++;
		else
		{
			saveGame.latestUnlockedSeriesIndex++;
			saveGame.latestUnlockedStageIndex = 0;
		}
	}

	public StageSeriesData GetLatestSeries()
	{
		return regularStages.series[saveGame.latestUnlockedSeriesIndex];
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
		for (int i = 0; i <= saveGame.latestUnlockedSeriesIndex; i ++)
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
		SaveLoad.Save ();
	}

	// ==========
	// Loading Screen
	// ==========

	private IEnumerator ActivateLoadingScreen()
	{
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

	private IEnumerator DeactivateLoadingScreen()
	{
		Color initialColor = Color.black;
		Color finalColor = Color.clear;
		float t = 0;
		while (loadingOverlay.color.a > 0.05f)
		{
			loadingOverlay.color = Color.Lerp (initialColor, finalColor, t * LOADING_SCREEN_SPEED);
			t += Time.deltaTime;
			yield return null;
		}
		loadingOverlay.color = finalColor;
	}

	// ==========
	// Save and Load
	// ==========

	public void PrepareSaveFile()
	{
		saveGame.highScores = scoreManager.highScores;
		saveGame.wallet = wallet;
	}

	public void LoadSaveFile()
	{
		scoreManager.highScores = saveGame.highScores;
		wallet = saveGame.wallet;
	}

	public void DeleteSaveData()
	{
		saveGame = new SaveGame ();
		LoadSaveFile ();
		SaveLoad.Save ();
	}

	// ==========
	// Debug Text UI
	// ==========

	public void DisplayMessage(string message)
	{
		debugText.SetColor (Color.white);
		debugText.Display (new MessageText.Message(message, 1, 2f, 1f, Color.white));
	}
}