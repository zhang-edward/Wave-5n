using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wallet
{
	public int money { get; private set; }			// amount of money
	public int moneyEarned { get; private set; }	// amount of money earned this round (if currently in-game)

	public bool TrySpend(int amt)
	{
		if (moneyEarned >= amt)
		{
			moneyEarned -= amt;
			return true;
		}
		else if (moneyEarned + money >= amt)
		{
			money -= (amt - moneyEarned);
			moneyEarned = 0;
			return true;
		}
		else
			return false;
	}

	public void Earn(int amt, bool addDirect)
	{
		if (addDirect)
			money += amt;
		else
			moneyEarned += amt;
	}

	public void MergeEarnedMoney()
	{
		money += moneyEarned;
		moneyEarned = 0;
	}

	// DEBUG
	public void SetMoneyDebug(int amt)
	{
		money = amt;
	}

	public void SetEarnedMoneyDebug(int amt)
	{
		moneyEarned = amt;
	}
}

