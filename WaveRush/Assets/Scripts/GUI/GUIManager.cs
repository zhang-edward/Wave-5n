﻿using UnityEngine;
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
	[Header("Inter-wave menus")]
	public PartyView partyView;
	public GameObject interWaveMenuContainer;
	[Tooltip("Components you want to animate out upon the interwave menus closing")]
	public UIAnimatorControl[] interWaveMenus;
	[Header("Custom UI")]
	public Transform customUI;					// Used for heroes to have custom hero-specific ui elements (located under pause button)

	[Header("Data")]
	public EnemyManager enemyManager;
	public Player player;

	public delegate void GUIEvent();
	public event GUIEvent OnStageCompletedTextDone; 

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
		interWaveMenuContainer.SetActive(true);
		foreach (UIAnimatorControl uiac in interWaveMenus)
			uiac.gameObject.SetActive(false);
	}

	private void DisableWaveCompletedMenus(int foo) {
		interWaveMenuContainer.GetComponent<UIAnimatorControl>().AnimateOut();
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

	public void OnStageCompletedText()
	{
		StartCoroutine(OnStageCompletedTextRoutine());
	}

	private IEnumerator OnStageCompletedTextRoutine() {
		enemyWaveText.DisplayWaveComplete();
		while (enemyWaveText.messageText.displaying) {
			yield return null;
		}
		enemyWaveText.DisplayStageComplete();
		while (enemyWaveText.messageText.displaying) {
			yield return null;
		}
		if (OnStageCompletedTextDone != null)
			OnStageCompletedTextDone();
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
								  player.hero.specialAbilityCharge / PlayerHero.SPECIAL_ABILITY_CHARGE_CAPACITY);
		player.SetHero(index);
	}
} 
