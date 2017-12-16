﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public const string BattleSceneName = "Game";

	public const float LOADING_SCREEN_SPEED = 8; 
	public static GameManager instance;

	[Header("Realtime Timer")]
	public RealtimeTimerCounter timerCounter;
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
	public MessageText alertText;
	public Text fpsDisplay;

	public delegate void GameStateUpdate();
	public GameStateUpdate OnSceneLoaded;
	public GameStateUpdate OnAppLoaded;
	public GameStateUpdate OnAppClosed;
	public GameStateUpdate OnTimersUpdated;
	public delegate void SaveStateUpdate();
	public SaveStateUpdate OnHasViewedDictionaryUpdated;



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

		//print("GameManager awake");
		SaveLoad.Load ();
		InitPawnTimers();
		questManager.Init();
		loadingOverlay.gameObject.SetActive(false);
	}

	public void InitPawnTimers()
	{
		foreach (Pawn pawn in saveGame.pawnWallet.pawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				timerCounter.SetTimer(pawn.GetTimerID(), pawn.unlockTime);
			}
		}
		foreach (Pawn pawn in saveGame.pawnWallet.extraPawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				timerCounter.SetTimer(pawn.GetTimerID(), pawn.unlockTime);
			}
		}
		timerCounter.UpdateTimersSinceLastClosed();
	}

	public void AddPawnTimer(int id)
	{
		Pawn pawn = saveGame.pawnWallet.GetPawn(id);
		timerCounter.SetTimer(pawn.GetTimerID(), pawn.unlockTime);
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
			PlayerPrefs.SetString(RealtimeTimerCounter.LAST_CLOSED_KEY, System.DateTime.Now.ToString());
			if (OnAppClosed != null)
				OnAppClosed();
			SavePawnTimers();
		}
		else
		{
			//print("Application Unpaused");
			if (OnAppLoaded != null)
				OnAppLoaded();
			timerCounter.UpdateTimersSinceLastClosed();
			if (OnTimersUpdated != null)
				OnTimersUpdated();
		}
	}

	private void SavePawnTimers()
	{
		foreach (Pawn pawn in saveGame.pawnWallet.pawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				RealtimeTimer timer = timerCounter.GetTimer(pawn.GetTimerID());
				pawn.unlockTime = timer.time;
			}
		}
		foreach (Pawn pawn in saveGame.pawnWallet.extraPawns)
		{
			if (pawn != null && pawn.unlockTime > 0)
			{
				RealtimeTimer timer = timerCounter.GetTimer(pawn.GetTimerID());
				pawn.unlockTime = timer.time;
			}
		}
		SaveLoad.Save();
	}

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetString(RealtimeTimerCounter.LAST_CLOSED_KEY, System.DateTime.Now.ToString());
		print("Application Quit");
		if (OnAppClosed != null)
			OnAppClosed();
		SavePawnTimers();
		SaveLoad.Save();
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

	/// <summary>
	/// Sets the key in the viewed dictionary to the given value
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="val">value to set</param>
	public void SetHasPlayerViewedKey(string key, bool val)
	{
//		print("Key: " + key);
		if (!saveGame.hasPlayerViewedDict.ContainsKey(key))
			saveGame.hasPlayerViewedDict.Add(key, val);
		else
			saveGame.hasPlayerViewedDict[key] = val;
		// Event for NewFeatureIndicators to refresh their 
		if (OnHasViewedDictionaryUpdated != null)
			OnHasViewedDictionaryUpdated();
	}

	public void InitHasPlayerViewedKey(string key, bool val)
	{
		Assert.IsTrue(!saveGame.hasPlayerViewedDict.ContainsKey(key));
		saveGame.hasPlayerViewedDict.Add(key, val);
	}

	public void DisplayAlert(string message)
	{
		alertText.SetColor(Color.white);
		alertText.Display(new MessageText.Message(message, 1, 0, 2f, 1f, Color.white));
	}

	// ==========
	// Debug Text UI
	// ==========

	public void DisplayMessage(string message)
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