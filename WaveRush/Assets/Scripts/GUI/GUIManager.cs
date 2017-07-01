using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	// hud
	[Header("GUI")]
	public GameObject gameUI;
	public EnemyWaveText enemyWaveText;

	[Header("Game Over Panel")]
	public GameObject gameOverUI;   // game over panel
	public ScoreReport scorePanel;  // score report in game over panel
	public GameObject stageClearPanel;

	[Header("Data")]
	public EnemyManager enemyManager;
	public Player player;

	void OnEnable()
	{
		enemyManager.OnEnemyWaveSpawned += ShowEnemyWaveText;
		enemyManager.OnEnemyWaveCompleted += OnEnemyWaveCompletedText;
		enemyManager.OnQueueBossMessage += ShowBossIncomingText;
	}

	void OnDisabled()
	{
		enemyManager.OnEnemyWaveSpawned -= ShowEnemyWaveText;
		enemyManager.OnEnemyWaveCompleted -= OnEnemyWaveCompletedText;
		enemyManager.OnQueueBossMessage -= ShowBossIncomingText;
	}

	public void GameOverUI(ScoreReport.ScoreReportData data)
	{
		StartCoroutine(GameOverUIRoutine(data));
	}

	private IEnumerator GameOverUIRoutine(ScoreReport.ScoreReportData data)
	{
		yield return new WaitForSeconds(1f);
		gameUI.SetActive (false);
		gameOverUI.GetComponent<Animator> ().SetTrigger ("In");
		scorePanel.moneyText.text.text = data.money.ToString();
		scorePanel.moneyEarned.text.text = data.moneyEarned.ToString();

		gameOverUI.SetActive (true);
		yield return new WaitForSeconds(0.5f);
		scorePanel.ReportScore(data);
	}

	public void TryShowStageCompleteView()
	{
		StartCoroutine(StageCompleteViewRoutine());
	}

	private IEnumerator StageCompleteViewRoutine()
	{
		if (enemyManager.IsStageComplete())
			stageClearPanel.SetActive(true);
		yield return new WaitForSeconds(1.0f);
		GameManager.instance.GoToScene("MainMenu");
		yield return null;
	}

	private void ShowEnemyWaveText(int waveNumber)
	{
		enemyWaveText.DisplayWaveNumber (waveNumber);
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
