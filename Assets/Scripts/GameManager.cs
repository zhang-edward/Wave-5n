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
	}

	void Start()
	{
		if (SceneManager.GetActiveScene ().name == "Game")
		{
			InitGameScene ();
		}
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

	public void GoToGameScene()
	{
		//didInitializeGameScene = false;
		StartCoroutine (LoadGameScene());
		ObjectPooler.objectPoolers.Clear ();
	}

	public void GoToMenuScene()
	{
		SceneManager.LoadScene ("Menu");
		ObjectPooler.objectPoolers.Clear ();
	}

	public IEnumerator LoadGameScene()
	{
		AsyncOperation async = SceneManager.LoadSceneAsync ("Game");
		Debug.Log ("Loading scene");
		while (!async.isDone)
		{
			yield return null;
		}
		InitGameScene ();
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

		//didInitializeGameScene = true;
	}

	public void SelectHero(string name)
	{
		selectedHero = name;
	}
}
