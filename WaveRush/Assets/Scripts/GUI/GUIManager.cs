using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	// hud
	[Header("GUI")]
	public GameObject gameUI;
	public EnemyWaveText enemyWaveText;

	// game over panel
	[Header("Game Over Panel")]
	public GameObject gameOverUI;
	// score report in game over panel
	public ScoreReport scorePanel;

	[Header("Data")]
	public EnemyManager enemyManager;
	public Player player;

	void OnEnable()
	{
		player.OnPlayerDied += GameOverUI;
		enemyManager.OnEnemyWaveSpawned += ShowEnemyWaveText;
		enemyManager.OnEnemyWaveCompleted += OnEnemyWaveCompletedText;
		enemyManager.OnBossIncoming += ShowBossIncomingText;
	}

	void OnDisabled()
	{
		player.OnPlayerDied -= GameOverUI;
		enemyManager.OnEnemyWaveSpawned -= ShowEnemyWaveText;
		enemyManager.OnEnemyWaveCompleted -= OnEnemyWaveCompletedText;
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
		scorePanel.moneyText.text.text = GameManager.instance.wallet.money.ToString();
		scorePanel.moneyEarned.text.text = GameManager.instance.wallet.moneyEarned.ToString();

		gameOverUI.SetActive (true);
		Invoke("ReportScore", 0.5f);	
	}

	private void ReportScore()
	{
		scorePanel.ReportScore (
			enemyManager.enemiesKilled, 
			enemyManager.waveNumber - 1, 
			player.hero.maxCombo);
	}

	private void ShowEnemyWaveText(int waveNumber)
	{
		enemyWaveText.DisplayWaveNumberAfterDelay (waveNumber, 1.0f);
	}

	private void OnEnemyWaveCompletedText()
	{
		enemyWaveText.DisplayWaveComplete ();
	}

	private void ShowBossIncomingText()
	{
		enemyWaveText.DisplayBossIncoming ();
	}
} 
