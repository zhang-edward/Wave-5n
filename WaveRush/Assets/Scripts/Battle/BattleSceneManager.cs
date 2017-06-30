﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages overall Battle Scene state
/// </summary>
public class BattleSceneManager : MonoBehaviour
{
	public static BattleSceneManager instance;
	GameManager gm;

	public Map map;
	public EnemyManager enemyManager;
	public Player player;
	public GUIManager gui;

	private List<Pawn> acquiredPawns = new List<Pawn>();

	void Awake()
	{
		// Make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(this.gameObject);

		gm = GameManager.instance;
		gm.OnSceneLoaded += Init;
		player.OnPlayerDied += UpdateData;
	}

	// Init main game environment
	private void Init()
	{
		// Get data from GameManager
		Pawn pawn = gm.selectedPawn;
		StageData stage = gm.GetStage(gm.selectedSeriesIndex, gm.selectedStageIndex);

		// Initialize components
		print("Map:" + map);
		map.chosenMap = stage.mapType;
		map.GenerateMap();
		player.Init(pawn);	
		enemyManager.Init(stage);

		SoundManager.instance.PlayMusicLoop(map.data.musicLoop, map.data.musicIntro);   // Plays the game music, looped

		gm.OnSceneLoaded -= Init;	// Remove the listener because it is only run once per scene
	}

	private void UpdateData()
	{
		ScoreReport.ScoreReportData data = new ScoreReport.ScoreReportData(
			enemiesDefeated: 	enemyManager.enemiesKilled,
			wavesSurvived: 		enemyManager.waveNumber - 1,
			maxCombo: 			player.hero.maxCombo,
			money: 				gm.wallet.money,
			moneyEarned: 		gm.wallet.moneyEarned);
		gui.GameOverUI(data);

		if (enemyManager.IsStageComplete() && IsPlayerOnLatestStage())
		{
			gm.UnlockNextStage();
			print("Stage Complete");
		}

		foreach(Pawn pawn in acquiredPawns)
		{
			gm.saveGame.AddPawn(pawn);
		}

		int enemiesDefeated = enemyManager.enemiesKilled;
		int wavesSurvived = enemyManager.waveNumber;
		int maxCombo = player.hero.maxCombo;

		gm.wallet.MergeEarnedMoney();
		gm.UpdateScores(enemiesDefeated, wavesSurvived, maxCombo);
	}

	private void ShowGameOverUI()
	{
		
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
}
