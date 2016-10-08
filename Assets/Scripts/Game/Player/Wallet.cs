using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wallet
{
	public int money { get; private set; }			// amount of money
	public int moneyEarned { get; private set; }	// amount of money earned this round (if currently in-game)

	public void Spend(int amt)
	{
		if (moneyEarned >= amt)
		{
			moneyEarned -= amt;
		}
		else
		{
			moneyEarned = 0;
			money -= (amt - moneyEarned);
		}
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
}

