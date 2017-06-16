using UnityEngine;
using System.Collections;

public class DebugCheatMenu : MonoBehaviour
{
	GameManager gm;

	void Start()
	{
		gm = GameManager.instance;
	}

	// ========================== DEBUG FUNCTIONS ======================
#if UNITY_EDITOR
	public void SetMoneyDebugString(string str)
	{
		int i = 0;
		if (int.TryParse(str, out i))
			SetMoney(i);
		else
			Debug.LogWarning("Error: not an int");
	}

	public void SetMoneyEarnedDebugString(string str)
	{
		int i = 0;
		if (int.TryParse(str, out i))
			SetMoneyEarned(i);
		else
			Debug.LogWarning("Error: not an int");
	}

	public void SetMoney(int amt)
	{
		gm.wallet.SetMoneyDebug(amt);
	}

	public void SetMoneyEarned(int amt)
	{
		gm.wallet.SetEarnedMoneyDebug(amt);
	}

	public void KillPlayer()
	{
		Player plyr = gm.playerObj.GetComponentInChildren<Player>();
		plyr.Damage(plyr.health);
	}

	public void FullChargeSpecial()
	{
		Player plyr = gm.playerObj.GetComponentInChildren<Player>();
		plyr.hero.IncrementSpecialAbilityCharge(int.MaxValue);
	}

	public void KillAllEnemies()
	{
		EnemyManager enemyManager = GameObject.Find("/Game/EnemyManager").GetComponent<EnemyManager>();
		for (int i = enemyManager.Enemies.Count - 1; i >= 0; i--)
		{
			enemyManager.Enemies[i].Damage(enemyManager.Enemies[i].maxHealth);
		}
	}

	public void SpawnBoss()
	{
		EnemyManager enemyManager = GameObject.Find("/Game/EnemyManager").GetComponent<EnemyManager>();
		enemyManager.SpawnBoss();
	}

	public void AddPowerUp(string name)
	{
		Player plyr = gm.playerObj.GetComponentInChildren<Player>();
		plyr.hero.powerUpHolder.AddPowerUp(name);
	}

#endif
}
