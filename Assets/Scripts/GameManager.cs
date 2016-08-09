using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public string selectedHero = "";
	public GameObject player;
	private Map map;

	public ScoreManager scoreManager;

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
#if UNITY_ANDROID
		Application.targetFrameRate = 30;
#endif

	}

	void Update()
	{
		/*if (SceneManager.GetActiveScene ().name == "Game" && !didInitializeGameScene)
		{
			InitGameScene ();	
		}*/

		if (Input.GetKeyDown (KeyCode.R))
		{
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			//didInitializeGameScene = false;
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
		AsyncOperation async = SceneManager.LoadSceneAsync (scene);
		Debug.Log ("Loading scene");
		Assert.IsNotNull (async);
		while (!async.isDone)
		{
			yield return null;
		}
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
		map = GameObject.Find ("/Game/Map").GetComponent<Map>();
		map.GenerateMap ();
		player = GameObject.Find ("/Game/Player");
		Assert.IsNotNull (player);
		Player playerScript = player.GetComponentInChildren<Player> ();

		Assert.IsFalse (selectedHero.Equals (""));		// will throw an error if this script tries to
														// initialize the player without a selected hero
		playerScript.Init (selectedHero);
		SoundManager.instance.PlayMusicLoop (map.info.musicLoop, map.info.musicIntro);
		//didInitializeGameScene = true;
	}

	public void SelectHero(string name)
	{
		selectedHero = name;
	}

	public void UpdateScores(int enemiesKilled, int wavesSurvived)
	{
		scoreManager.SubmitScore (selectedHero, new ScoreManager.Score (enemiesKilled, wavesSurvived));
		SaveLoad.Save ();
	}
}
