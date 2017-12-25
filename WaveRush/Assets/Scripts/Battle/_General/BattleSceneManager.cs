﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages overall Battle Scene state
/// </summary>
public class BattleSceneManager : MonoBehaviour
{
	public static BattleSceneManager instance;
	private GameManager gm;

	public Map map;
	public EnemyManager enemyManager;
	public Player player;
	public GUIManager gui;
	[Header("UI")]
	public LosePanel losePanel;
	public DialogueView dialogueView;
	public GameObject stageCompleteOptions;
	public ModalSelectionView stageCompleteModal;
	public Button exitButton;
	public Button continueButton;
	public GameObject pauseButton;

	public List<Pawn> acquiredPawns { get; private set; }   // pawns acquired this session
	public int moneyEarned { get; private set; } 			// money earned in this session
	public int soulsEarned { get; private set; }            // souls earned in this session
	public bool leaveOrContinueOptionOpen { get; private set; }

	public delegate void BattleSceneEvent();
	public BattleSceneEvent OnStageCompleted;

	void Awake()
	{
		// Make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);

		// Do BattleSceneManager-specific initialization which has no external dependencies
		acquiredPawns = new List<Pawn>();
		gm = GameManager.instance;
		enemyManager.OnStageCompleted += () => { StartCoroutine(StageCompleteRoutine()); };
		player.OnPlayerDied += UpdateData;
		//gm.OnSceneLoaded += Init;
	}

	void Start()
	{
		Init();
	}

	// Init main game environment
	private void Init()
	{
		StartCoroutine(InitRoutine());
	}

	private IEnumerator InitRoutine()
	{
		// Get data from GameManager
		Pawn pawn = gm.selectedPawn;
		StageData stage = gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex);

		// Initialize components
		map.chosenMap = stage.mapType;
		map.GenerateMap();
		player.Init(pawn);

		// Do dialogue before starting the game
		if (stage.dialogueSets.Length > 0)
			dialogueView.Init(stage.dialogueSets);
		player.input.enabled = false;
		gui.enemyWaveText.gameObject.SetActive(false);
		while (dialogueView.dialoguePlaying)
			yield return null;

		SoundManager.instance.PlayMusicLoop(map.data.musicLoop, map.data.musicIntro);
		player.input.enabled = true;
		gui.enemyWaveText.gameObject.SetActive(true);
		enemyManager.Init(stage);
		gui.DisplayIntroMessage();

		//gm.OnSceneLoaded -= Init;   // Remove the listener because it is only run once per scene
	}

	private IEnumerator StageCompleteRoutine()
	{
		enemyManager.paused = true;
		// Delay before showing end dialogue
		yield return new WaitForSecondsRealtime(1.0f);
		stageCompleteOptions.SetActive(true);
		leaveOrContinueOptionOpen = true;
		// Modal View Selection
		int selection = -1;
		stageCompleteModal.OnOptionSelected += (s) => {
			selection = s;
		};
		while (leaveOrContinueOptionOpen)
		{
			pauseButton.gameObject.SetActive(false);
			player.input.isInputEnabled = false;
			if (selection == 1)
			{
				player.transform.parent.gameObject.SetActive(false);
				stageCompleteOptions.GetComponent<UIAnimatorControl>().AnimateOut();
				UpdateData();
				yield break;
			}
			if (selection == 0)
			{
				leaveOrContinueOptionOpen = false;
			}
			yield return null;
		}
		pauseButton.gameObject.SetActive(true);
		player.input.isInputEnabled = true;
		enemyManager.paused = false;
		stageCompleteOptions.GetComponent<UIAnimatorControl>().AnimateOut();
		//print("No sacrifice detected; continue");
	}

	private void UpdateData()
	{
		ScoreReport.ScoreReportData data = new ScoreReport.ScoreReportData(
			enemiesDefeated: 	enemyManager.enemiesKilled,
			wavesSurvived: 		Mathf.Max(enemyManager.waveNumber - 1, 0),
			maxCombo: 			player.hero.maxCombo,
			money: 				gm.wallet.money,
			moneyEarned: 		moneyEarned,
			souls: 				gm.wallet.souls,
			soulsEarned:		soulsEarned
		);

		gui.gameOverUI.SetActive(true);
		losePanel.Init(data, acquiredPawns, gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex).stageName);

		if (enemyManager.isStageComplete)
		{
			if (OnStageCompleted != null)
				OnStageCompleted();
			if (IsPlayerOnLatestStage())
			{
				gm.UnlockNextStage();
			}
		}

		gm.saveGame.pawnWallet.RemovePawn(gm.selectedPawn.id);
		foreach(Pawn pawn in acquiredPawns)
		{
			gm.saveGame.pawnWallet.AddPawn(pawn);
		}

		int enemiesDefeated = enemyManager.enemiesKilled;
		int wavesSurvived = enemyManager.waveNumber;
		int maxCombo = player.hero.maxCombo;

		gm.wallet.AddMoney(moneyEarned);
		gm.wallet.AddSouls(soulsEarned);
		gm.UpdateScores(enemiesDefeated, wavesSurvived, maxCombo);
	}

	private bool IsPlayerOnLatestStage()
	{
		return (gm.selectedStageIndex == gm.saveGame.latestUnlockedStageIndex &&
				gm.selectedSeriesIndex == gm.saveGame.latestUnlockedSeriesIndex);
	}

	public void AddPawn(Pawn pawn)
	{
		acquiredPawns.Add(pawn);
	}

	public void AddMoney(int amt)
	{
		moneyEarned += amt;
		gui.UpdateMoney(moneyEarned);
	}

	public void AddSouls(int amt)
	{
		soulsEarned += amt;
		gui.UpdateSouls(soulsEarned);
	}
}
