using UnityEngine;
using System.Collections;

/// <summary>
/// Manages MainMenu Scene State
/// </summary>
public class MainMenuSceneManager : MonoBehaviour
{
	public static MainMenuSceneManager instance;
	private GameManager gm;

	void Awake()
	{
		// Make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);

		gm.OnSceneLoaded += Init;
	}

	private void Init()
	{
		
	}
}
