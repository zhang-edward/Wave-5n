using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class DebugCheatMenu : MonoBehaviour
{
	GameManager gm;

	public GameObject battleSceneDebugOptions;
	public GameObject menuSceneDebugOptions;

	private Player player;
	private EnemyManager enemyManager;

	void OnEnable()
	{
		gm = GameManager.instance;
		UpdateDebugMenuOptions();
	}

	void UpdateDebugMenuOptions()
	{
		bool isBattleSceneOpen = SceneManager.GetActiveScene().name.Equals("Game");
		menuSceneDebugOptions.SetActive(!isBattleSceneOpen);
		battleSceneDebugOptions.SetActive(isBattleSceneOpen);
		if (isBattleSceneOpen)
		{
			player = GameObject.Find("/Game/Player").GetComponentInChildren<Player>();
			enemyManager = GameObject.Find("/Game/EnemyManager").GetComponent<EnemyManager>();
		}
	}

	// ==========
	// Menu Scene Options
	// ==========
	public void SetMoneyDebugString(string amt)
	{
		gm.wallet.SetMoneyDebug(Convert.ToInt32(amt));
	}

	public void SetMoneyEarnedDebugString(string amt)
	{
		gm.wallet.SetMoneyDebug(Convert.ToInt32(amt));
	}

	public void SaveGame()
	{
		GameManager.instance.PrepareSaveFile();
		SaveLoad.Save();
	}

	public void AddNewPawn(string level)
	{
		int numHeroTypes = Enum.GetNames(typeof(HeroType)).Length;
		//HeroType type = (HeroType)Enum.GetValues(typeof(HeroType)).GetValue(UnityEngine.Random.Range(1, numHeroTypes));
		Pawn pawn = new Pawn();
		pawn.level = Convert.ToInt32(level);
		pawn.type = HeroType.Knight;//type;
		GameManager.instance.saveGame.AddPawn(pawn);
		SaveLoad.Save();
	}

	public void ClearSaveData()
	{
		GameManager.instance.DeleteSaveData();
	}

	// ==========
	// Battle Scene Options
	// ==========
	public void KillPlayer()
	{
		player.Damage(player.health);
	}

	public void HealPlayer()
	{
		player.Heal(player.health);
	}

	public void FullChargeSpecial()
	{
		player.hero.IncrementSpecialAbilityCharge(int.MaxValue);
	}

	public void KillAllEnemies()
	{
		for (int i = enemyManager.Enemies.Count - 1; i >= 0; i--)
		{
			enemyManager.Enemies[i].Damage(enemyManager.Enemies[i].maxHealth);
		}
	}

	public void SpawnBoss()
	{
		enemyManager.SpawnBoss();
	}

	public void AddPowerUp(string name)
	{
		player.hero.powerUpManager.AddPowerUp(name);
	}

}
