using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AddPowerUpItem : PowerUpItem
{
	public Image icon;
	public Text stacksIndicator;
	public HeroPowerUp powerUp { get; private set; }
	public List<AddPowerUpItem> unlockable = new List<AddPowerUpItem>();
	

	public void Init(HeroPowerUp powerUp)
	{
		this.powerUp = powerUp;
		SetHolderGraphic(powerUp.GetComponent<HeroPowerUp>().data.tier);	// set the holder graphic to match the tier level of the power up
		icon.sprite = powerUp.data.icon;
		purchaseLimit = powerUp.data.maxStacks + 1;
		timesPurchased = powerUp.isActive ? powerUp.stacks + 1 : 0;			// ternary operator
		cost = powerUp.data.cost;
		GetComponent<ScrollingTextOption> ().text = powerUp.data.description;

		if (powerUp.data.maxStacks == 0)
			stacksIndicator.transform.parent.gameObject.SetActive(false);
		UpdateAvailablity();
	}

	void OnEnable()
	{
		stacksIndicator.text = ToRomanNumeral(timesPurchased);
		if (timesPurchased > 0 && !powerUp.data.stackDescription.Equals(""))
		{
			GetComponent<ScrollingTextOption> ().text = powerUp.data.stackDescription;
		}
	}

	public void UpdateAvailablity()
	{
		// if the purchase limit has already been reached
		if (timesPurchased >= purchaseLimit)
		{
			available = false;
		}
		if (timesPurchased >= 1)
		{
			foreach (AddPowerUpItem item in unlockable)
			{
				print("item:" + item.powerUp + " has been made available");
				item.available = true;
				item.UpdateAvailablity();
			}	
		}

	}

	public override void Upgrade (Player player)
	{
		base.Upgrade (player);
		player.hero.powerUpHolder.AddPowerUp (powerUp.data.powerUpName);
		UpdateAvailablity();
	}

	public void SetAvailable(bool available)
	{
		this.available = available;
	}

	private string ToRomanNumeral(int num)
	{
		if (num >= purchaseLimit - 1)
			return "MAX";
		if (num <= 3)
		{
			string romNum = "";
			for (int i = 0; i < num; i ++)
			{
				romNum += "i";
			}
			return romNum;
		}
		else if (num == 4)
		{
			return "iv";
		}
		else if (num == 5)
		{
			return "v";
		}
		return "ERROR";
	}
}

