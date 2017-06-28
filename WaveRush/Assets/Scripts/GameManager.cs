using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public SaveGame saveGame;
	public Pawn selectedPawn;
	public int selectedSeriesIndex;
	public int selectedStageIndex;
	public GameObject playerObj;

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

	//private bool didInitializeGameScene = false;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this);

		SaveLoad.Load ();
	}

	void Start()
	{
		if (OnSceneLoaded != null)
			OnSceneLoaded();

#if UNITY_ANDROID
		Application.targetFrameRate = 60;
#else
		Application.targetFrameRate = 60;
#endif
		StartCoroutine(FPS());
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
		/*switch (scene)
		{
		case("Game"):
			InitGameScene();
			break;
		}*/
	}

	// init main game environment
	/*private void InitGameScene()
	{
		Map map = GameObject.Find ("/Game/Map").GetComponent<Map>();
		EnemyManager enemyManager = GameObject.Find ("/Game/EnemyManager").GetComponent<EnemyManager> ();
		playerObj = GameObject.Find ("/Game/Player");
		Assert.IsNotNull (playerObj);
		Player player = playerObj.GetComponentInChildren<Player> ();
		Assert.IsFalse(selectedPawn.type == HeroType.Null);		// will throw an error if this script tries to initialize the player without a selected hero

		map.chosenMap = selectedStage.mapType;
		map.GenerateMap ();
		player.Init (selectedPawn);
		enemyManager.Init(selectedStage);

		SoundManager.instance.PlayMusicLoop (map.data.musicLoop, map.data.musicIntro);
	}*/

	/*public void TeleportMaps(MapType newMap)
	{
		SaveLoad.Save ();
		selectedMap = newMap;
		StartCoroutine ("Teleport");
	}

	private IEnumerator Teleport()
	{
		Color initialColor = Color.clear;
		Color finalColor = Color.white;
		float t = 0;
		while (loadingOverlay.color.a < 0.95f)
		{
			loadingOverlay.color = Color.Lerp (initialColor, finalColor, t * 4);
			t += Time.deltaTime;
			yield return null;
		}
		loadingOverlay.color = finalColor;
		AsyncOperation async = SceneManager.LoadSceneAsync ("Game");
		Assert.IsNotNull (async);

		while (!async.isDone)
			yield return null;
		StartCoroutine(DeactivateLoadingScreen ());
		InitGameScene ();
	}*/

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
		if (saveGame.latestUnlockedStageIndex < regularStages.series[saveGame.latestUnlockedSeriesIndex].stages.Length)
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
			loadingOverlay.color = Color.Lerp (initialColor, finalColor, t * 4);
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
			loadingOverlay.color = Color.Lerp (initialColor, finalColor, t * 8);
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
		debugText.Display (message, 1, 2f, 1f);
	}
}