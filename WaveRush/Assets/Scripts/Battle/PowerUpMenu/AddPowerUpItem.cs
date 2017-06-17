using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddPowerUpItem : ShopItemProgression
{
	public Image icon;
	public Text stacksIndicator;
	public HeroPowerUp powerUp { get; private set; }

	public void Init(HeroPowerUp powerUp)
	{
		this.powerUp = powerUp;
		SetHolderGraphic(powerUp.GetComponent<HeroPowerUp>().data.tier);	// set the holder graphic to match the tier level of the power up
		icon.sprite = powerUp.data.icon;
		purchaseLimit = powerUp.data.maxStacks + 1;
		cost = powerUp.data.cost;
		GetComponent<ScrollingTextOption> ().text = powerUp.data.description;

		if (powerUp.data.maxStacks == 0)
			stacksIndicator.transform.parent.gameObject.SetActive (false);
	}

	void OnEnable()
	{
		stacksIndicator.text = ToRomanNumeral(timesPurchased + 1);
		if (timesPurchased > 0 && !powerUp.data.stackDescription.Equals(""))
		{
			GetComponent<ScrollingTextOption> ().text = powerUp.data.stackDescription;
		}
	}

	public override void Upgrade (Player player)
	{
		base.Upgrade (player);
		player.hero.powerUpHolder.AddPowerUp (powerUp.data.powerUpName);
	}

	private string ToRomanNumeral(int num)
	{
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

