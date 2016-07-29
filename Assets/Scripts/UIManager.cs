using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameObject gameUI;
	public GameObject menuUI;
	public GameObject gameOverUI;

	private Player player;
	private GameManager gameManager;

	void Awake()
	{
		gameManager = GameManager.instance;
		player = gameManager.player.GetComponentInChildren<Player> ();
		gameManager.OnInitGameMode += InitGameUI;
	}

	private void InitGameUI()
	{
		gameUI.SetActive (true);
	}
}
