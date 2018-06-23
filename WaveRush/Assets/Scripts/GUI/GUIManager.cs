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
	public IncrementingText moneyEarnedText;
	public IncrementingText soulsEarnedText;
	public PartyView partyView;
	public UIAnimatorControl partyViewMenu;
	public UIAnimatorControl nextWaveButton;

	[Header("Game Over Panel")]
	public GameObject gameOverUI;   	// game over panel
	public ScoreReport scorePanel;  	// score report in game over panel
	//public GameObject upgradeButton;    // button for upgrading the player character
	//public HeroesRescuedMenu heroesRescuedMenu;

	[Header("Data")]
	public EnemyManager enemyManager;
	public Player player;

	void OnEnable()
	{
		enemyManager.OnEnemyWaveSpawned += ShowEnemyWaveText;
		enemyManager.OnEnemyWaveSpawned += DisableWaveCompletedMenus;
		enemyManager.OnEnemyWaveCompleted += EnableWaveCompletedMenus;
		enemyManager.OnEnemyWaveCompleted += OnEnemyWaveCompletedText;
		enemyManager.OnStageCompleted += OnStageCompletedText;
		enemyManager.OnQueueBossMessage += ShowBossIncomingText;
		partyView.SwitchHero += SwitchHero;
		SaveModifier save = GameManager.instance.save;
	}

	void OnDisabled()
	{
		enemyManager.OnEnemyWaveSpawned -= ShowEnemyWaveText;
		enemyManager.OnEnemyWaveSpawned -= DisableWaveCompletedMenus;
		enemyManager.OnEnemyWaveCompleted -= EnableWaveCompletedMenus;
		enemyManager.OnEnemyWaveCompleted -= OnEnemyWaveCompletedText;
		enemyManager.OnStageCompleted -= OnStageCompletedText;
		enemyManager.OnQueueBossMessage -= ShowBossIncomingText;
		partyView.SwitchHero -= SwitchHero;
	}

	private void EnableWaveCompletedMenus() {
		nextWaveButton.gameObject.SetActive(true);
		partyViewMenu.gameObject.SetActive(true);
	}

	private void DisableWaveCompletedMenus(int foo) {
		nextWaveButton.AnimateOut();
		partyViewMenu.AnimateOut();
		partyView.gameObject.SetActive(false);
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
		//heroesRescuedMenu.Init(BattleSceneManager.instance.acquiredPawns);
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
		yield return new WaitForSeconds(1.0f);
		GameManager.instance.GoToScene(GameManager.SCENE_MAINMENU);
		yield return null;
	}

	private void ShowEnemyWaveText(int waveNumber)
	{
		int goalWave = enemyManager.stageData.goalWave;
		if (waveNumber % goalWave == 0)
			waveIndicatorText.text = goalWave.ToString() + "/" + goalWave.ToString();
		else
			waveIndicatorText.text = (waveNumber % goalWave).ToString() + "/" + goalWave.ToString();

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

	private void SwitchHero(int index) {
		partyView.DeactivateCard(index);
		partyView.ActivateCard(player.activePartyMember);
		partyView.UpdatePartyCard(player.activePartyMember, 
								  (float)player.hero.hardHealth / player.maxHealth, 
								  player.hero.specialAbilityCharge / player.hero.specialAbilityChargeCapacity);
		player.SetHero(index);
	}
} 
