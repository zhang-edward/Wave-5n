using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wallet
{
	public int money { get; private set; }
	public int souls { get; private set; }

	public bool TrySpendMoney(int amt)
	{
		if (money >= amt)
		{
			money -= amt;
			return true;
		}
		return false;
	}

	public bool TrySpendSouls(int amt)
	{
		if (souls >= amt)
		{
			souls -= amt;
			return true;
		}
		return false;
	}

	public void AddMoney(int amt)
	{
		money += amt;
	}

	public void AddSouls(int amt)
	{
		souls += amt;
	}

	// ==========
	// DEBUG
	// ==========
	public void SetMoneyDebug(int amt)
	{
		money = amt;
	}

	public void SetSoulsDebug(int amt)
	{
		souls = amt;
	}
}

