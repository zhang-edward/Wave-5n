using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	/// <summary>
	/// Game data to be loaded and saved.
	/// </summary>
	[System.Serializable]
	public class SaveGame
	{
		public Dictionary<string, ScoreManager.Score> highScores;
		public bool[] unlockedHeroes;
		public Wallet wallet;

		public SaveGame()
		{
			// default all heroes locked but the first hero (the knight)
			unlockedHeroes = new bool[PlayerHero.HERO_TYPES.Count];
			unlockedHeroes[0] = true;
			// high scores are all 0 by default
			highScores = new Dictionary<string, ScoreManager.Score>();
			// wallet money = 0 by default
			wallet = new Wallet();
		}

		public void ClearHighScores()
		{
			highScores = new Dictionary<string, ScoreManager.Score> ();
		}
	}
	public SaveGame saveGame = new SaveGame();

	public static GameManager instance;
	public Image loadingOverlay;
	private string selectedHero = "knight";
	public string selectedMap = "grass";
	public GameObject player;

	public ScoreManager scoreManager;
	public Wallet wallet;

	public GameObject debugPanel;

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
		if (SceneManager.GetActiveScene ().name == "Game")
		{
			InitGameScene ();
		}
		Application.targetFrameRate = 60;
	}

	void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.R))
		{
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		}
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			debugPanel.SetActive(!debugPanel.activeInHierarchy);
		}
#endif
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
		Debug.Log ("Loading scene");
		StartCoroutine(ActivateLoadingScreen ());
		while (loadingOverlay.color.a <= 0.95f)
			yield return null;
		AsyncOperation async = SceneManager.LoadSceneAsync (scene);
		Assert.IsNotNull (async);

		while (!async.isDone)
			yield return null;
		StartCoroutine(DeactivateLoadingScreen ());

		// On finished scene loading
		switch (scene)
		{
			case("Game"):
				InitGameScene();
				break;
		}
		Debug.Log ("Scene loaded");
	}

	private void InitGameScene()
	{
		// init main game environment
		Map map = GameObject.Find ("/Game/Map").GetComponent<Map>();
		EnemyManager enemyManager = GameObject.Find ("/Game/EnemyManager").GetComponent<EnemyManager> ();

		map.chosenInfo = selectedMap;
		map.GenerateMap ();
		enemyManager.chosenInfo = selectedMap;


		player = GameObject.Find ("/Game/Player");
		Assert.IsNotNull (player);
		Player playerScript = player.GetComponentInChildren<Player> ();

		Assert.IsFalse (selectedHero.Equals (""));		// will throw an error if this script tries to
														// initialize the player without a selected hero
		playerScript.Init (selectedHero);
		SoundManager.instance.PlayMusicLoop (map.info.musicLoop, map.info.musicIntro);
	}

	public void TeleportMaps(string newMap)
	{
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

	}

	public void SelectHero(string name)
	{
		selectedHero = name;
	}

	public void UpdateScores(int enemiesKilled, int wavesSurvived, int maxCombo)
	{
		scoreManager.SubmitScore (selectedHero, new ScoreManager.Score (enemiesKilled, wavesSurvived, maxCombo));
		SaveLoad.Save ();
	}

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


	// ========================== DEBUG FUNCTIONS ======================
#if UNITY_EDITOR
	public void SetMoneyDebugString(string str)
	{
		int i = 0;
		if (int.TryParse (str, out i))
			SetMoney (i);
		else
			Debug.LogWarning ("Error: not an int");
	}

	public void SetMoneyEarnedDebugString(string str)
	{
		int i = 0;
		if (int.TryParse (str, out i))
			SetMoneyEarned (i);
		else
			Debug.LogWarning ("Error: not an int");
	}

	public void SetMoney(int amt) 
	{
		wallet.SetMoneyDebug (amt);
	}

	public void SetMoneyEarned (int amt)
	{
		wallet.SetEarnedMoneyDebug(amt);
	}

	public void KillPlayer()
	{
		Player plyr = player.GetComponentInChildren<Player> ();
		plyr.Damage (plyr.health);
	}

	public void FullChargeSpecial()
	{
		Player plyr = player.GetComponentInChildren<Player> ();
		plyr.hero.IncrementSpecialAbilityCharge (int.MaxValue);
	}

	public void KillAllEnemies()
	{
		EnemyManager enemyManager = GameObject.Find ("/Game/EnemyManager").GetComponent<EnemyManager> ();
		foreach (Enemy e in enemyManager.Enemies)
		{
			e.Die ();
		}
	}

	public void SpawnBoss()
	{
		EnemyManager enemyManager = GameObject.Find ("/Game/EnemyManager").GetComponent<EnemyManager> ();
		enemyManager.SpawnBoss ();
	}
#endif
}
