using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public string selectedHero = "";
	public GameObject player;

	private bool didInitializeGameScene = false;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this);
	}

	void Update()
	{
		if (SceneManager.GetActiveScene ().name == "Game" && !didInitializeGameScene)
		{
			InitGameScene ();	
		}
	}

	public void GoToGameScene()
	{
		StartCoroutine (LoadGameScene());
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
		player = GameObject.Find ("/Game/Player");
		Player playerScript = player.GetComponentInChildren<Player> ();

		Assert.IsFalse (selectedHero.Equals (""));		// will throw an error if this script tries to
														// initialize the player without a selected hero
		playerScript.Init (selectedHero);

		didInitializeGameScene = true;
	}

	public void SelectHero(string name)
	{
		selectedHero = name;
	}
}
