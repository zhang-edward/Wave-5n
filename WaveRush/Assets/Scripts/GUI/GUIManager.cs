using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour {

	// hud
	[Header("GUI")]
	public GameObject gameUI;
	public EnemyWaveText enemyWaveText;
	public TMP_Text waveIndicatorText;
	public TMP_Text moneyText;
	public TMP_Text soulsText;
	public IncrementingText moneyEarnedText;
	public IncrementingText soulsEarnedText;

	[Header("Game Over Panel")]
	public GameObject gameOverUI;   	// game over panel
	public ScoreReport scorePanel;  	// score report in game over panel
	public GameObject stageClearPanel;
	public GameObject upgradeButton;    // button for upgrading the player character
	public HeroesRescuedMenu heroesRescuedMenu;

	[Header("Data")]
	public EnemyManager enemyManager;
	public Player player;

	void OnEnable()
	{
		enemyManager.OnEnemyWaveSpawned += ShowEnemyWaveText;
		enemyManager.OnEnemyWaveCompleted += OnEnemyWaveCompletedText;
		enemyManager.OnStageCompleted += OnStageCompletedText;
		enemyManager.OnQueueBossMessage += ShowBossIncomingText;
		Wallet wallet = GameManager.instance.wallet;
		moneyText.text = wallet.money.ToString();
		soulsText.text = wallet.souls.ToString();
	}

	void OnDisabled()
	{
		enemyManager.OnEnemyWaveSpawned -= ShowEnemyWaveText;
		enemyManager.OnEnemyWaveCompleted -= OnEnemyWaveCompletedText;
		enemyManager.OnStageCompleted -= OnStageCompletedText;
		enemyManager.OnQueueBossMessage -= ShowBossIncomingText;
	}

	public void GameOverUI(ScoreReport.ScoreReportData data)
	{
		StartCoroutine(GameOverUIRoutine(data));
	}

	public void TryShowStageCompleteView()
	{
		StartCoroutine(StageCompleteViewRoutine());
	}

	public void DisplayIntroMessage()
	{
		MessageText.Message introMessage = new MessageText.Message
			(enemyManager.stageData.stageName,
			 numTimes: 1,
			 fadeInTime: 1.0f,
			 persistTime: 3.0f,
			 fadeOutTime: 1.0f,
			 color: Color.white);
		enemyWaveText.DisplayCustomMessage(introMessage);
	}

	public void InitializeHeroesRescuedMenu()
	{
		heroesRescuedMenu.Init(BattleSceneManager.instance.acquiredPawns);
	}

	private IEnumerator GameOverUIRoutine(ScoreReport.ScoreReportData data)
	{
		yield return new WaitForSeconds(1f);
		gameUI.SetActive(false);
		gameOverUI.SetActive(true);
		scorePanel.moneyText.text.text = data.money.ToString();
		scorePanel.moneyEarned.text.text = data.moneyEarned.ToString();
		scorePanel.soulsText.text.text = data.souls.ToString();
		scorePanel.soulsEarned.text.text = data.soulsEarned.ToString();

		yield return new WaitForSeconds(0.5f);
		scorePanel.ReportScore(data);
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
		waveIndicatorText.text = waveNumber.ToString();
		enemyWaveText.DisplayWaveNumber (waveNumber);
	}

	private void OnEnemyWaveCompletedText()
	{
		enemyWaveText.DisplayWaveComplete ();
	}

	private void OnStageCompletedText()
	{
		enemyWaveText.DisplayStageComplete();
	}

	private void ShowBossIncomingText()
	{
		enemyWaveText.DisplayBossIncoming ();
	}

	public void UpdateMoney(int num)
	{
		moneyEarnedText.DisplayNumber(num);
	}

	public void UpdateSouls(int num)
	{
		soulsEarnedText.DisplayNumber(num);
	}
} 
