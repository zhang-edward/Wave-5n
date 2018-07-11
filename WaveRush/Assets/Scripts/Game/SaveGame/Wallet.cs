using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

[System.Serializable]
public class Wallet
{
	public int money { get; private set; }
	public int souls { get; private set; }

	[JsonConstructor]
	public Wallet(int money, int souls) {
		this.money = money;
		this.souls = souls;
	}

	public Wallet() {
		money = 0;
		souls = 0;
	}

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
	public void SetMoney(int amt) {
		money = amt;
	}

	public void SetSouls(int amt) {
		souls = amt;
	}
}

