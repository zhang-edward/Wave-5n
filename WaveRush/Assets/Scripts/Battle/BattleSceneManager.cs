using UnityEngine;
using System.Collections;

/// <summary>
/// Manages overall Battle Scene state
/// </summary>
public class BattleSceneManager : MonoBehaviour
{
	GameManager gm;

	public Map map;
	public EnemyManager enemyManager;
	public Player player;

	void Awake()
	{
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
		if (enemyManager.IsStageComplete() && IsPlayerOnLatestStage())
		{
			gm.UnlockNextStage();
			print("Stage Complete");
		}

		int enemiesDefeated = enemyManager.enemiesKilled;
		int wavesSurvived = enemyManager.waveNumber;
		int maxCombo = player.hero.maxCombo;

		GameManager.instance.wallet.MergeEarnedMoney();
		GameManager.instance.UpdateScores(enemiesDefeated, wavesSurvived, maxCombo);
	}

	private bool IsPlayerOnLatestStage()
	{
		return (gm.selectedStageIndex == gm.saveGame.latestUnlockedStageIndex &&
				gm.selectedSeriesIndex == gm.saveGame.latestUnlockedSeriesIndex);
	}
}
