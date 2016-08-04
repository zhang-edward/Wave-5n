using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	public GameObject gameUI;
	public GameObject gameOverUI;
	public EnemyWaveText enemyWaveText;

	public EnemyManager enemyManager;
	public Player player;

	void Awake()
	{
	}

	void OnEnable()
	{
		player.OnPlayerDied += GameOverUI;
		enemyManager.OnEnemyWaveSpawned += ShowEnemyWaveText;
		enemyManager.OnBossIncoming += ShowBossIncomingText;
	}

	void OnDisabled()
	{
		player.OnPlayerDied -= GameOverUI;
		enemyManager.OnEnemyWaveSpawned -= ShowEnemyWaveText;
		enemyManager.OnBossIncoming -= ShowBossIncomingText;

	}

	private void GameOverUI()
	{
		Invoke ("InitGameOverUI", 1.0f);
	}

	private void InitGameOverUI()
	{
		gameUI.SetActive (false);
		gameOverUI.GetComponent<Animator> ().SetTrigger ("In");
		gameOverUI.SetActive (true);
	}

	private void ShowEnemyWaveText(int waveNumber)
	{
		enemyWaveText.DisplayWaveNumber (waveNumber);
	}

	private void ShowBossIncomingText()
	{
//		Debug.Log ("yo");
		enemyWaveText.DisplayBossIncoming ();
	}
}
