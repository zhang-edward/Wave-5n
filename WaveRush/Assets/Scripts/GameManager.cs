using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public SaveGame saveGame;

	public static GameManager instance;
	public Image loadingOverlay;
	public HeroType selectedHero;
	public MapType selectedMap;
	public GameObject player;

	public ScoreManager scoreManager;
	public Wallet wallet;

	public GameObject debugPanel;
	public MessageText debugText;

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
		//Debug.Log ("Loading scene");
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
		//Debug.Log ("Scene loaded");
	}

	private void InitGameScene()
	{
		// init main game environment
		Map map = GameObject.Find ("/Game/Map").GetComponent<Map>();
		EnemyManager enemyManager = GameObject.Find ("/Game/EnemyManager").GetComponent<EnemyManager> ();

		map.chosenMap = selectedMap;
		map.GenerateMap ();
		enemyManager.chosenMap = selectedMap;

		player = GameObject.Find ("/Game/Player");
		Assert.IsNotNull (player);
		Player playerScript = player.GetComponentInChildren<Player> ();

		Assert.IsFalse (selectedHero.Equals (""));		// will throw an error if this script tries to
														// initialize the player without a selected hero
		playerScript.Init (selectedHero);
		SoundManager.instance.PlayMusicLoop (map.data.musicLoop, map.data.musicIntro);
	}

	public void TeleportMaps(MapType newMap)
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
	}

	public void SelectHero(HeroType heroName)
	{
		selectedHero = heroName;
	}

	public void UpdateScores(int enemiesKilled, int wavesSurvived, int maxCombo)
	{
		string heroName = selectedHero.ToString();
		scoreManager.SubmitScore(heroName, new ScoreManager.Score (enemiesKilled, wavesSurvived, maxCombo));
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

	public void DisplayMessage(string message)
	{
		debugText.SetColor (Color.white);
		debugText.Display (message, 1, 2f, 1f);
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
		for (int i = enemyManager.Enemies.Count - 1; i >= 0; i --)
		{
			enemyManager.Enemies [i].Damage(enemyManager.Enemies[i].maxHealth);
		}
	}

	public void SpawnBoss()
	{
		EnemyManager enemyManager = GameObject.Find ("/Game/EnemyManager").GetComponent<EnemyManager> ();
		enemyManager.SpawnBoss ();
	}

	public void AddPowerUp(string name)
	{
		Player plyr = player.GetComponentInChildren<Player> ();
		plyr.hero.powerUpHolder.AddPowerUp (name);
	}

#endif
}
