using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public string SelectedHero = "";
	public GameObject player;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this);
	}

	public void GoToGameScene()
	{
		StartCoroutine (LoadGameSceneAsync());
	}

	public IEnumerator LoadGameSceneAsync()
	{
		AsyncOperation async = SceneManager.LoadSceneAsync ("Game");
		Debug.Log ("Loading scene");
		while (!async.isDone)
		{
			yield return null;
		}
		Debug.Log ("Scene loaded");
		player = GameObject.Find ("/Game/Player");
	}
}
