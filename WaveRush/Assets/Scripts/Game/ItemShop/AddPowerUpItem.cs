using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddPowerUpItem : ShopItemProgression
{
	public Image icon;
	public Text stacksIndicator;
	private HeroPowerUp powerUp;

	public void Init(HeroPowerUp powerUp)
	{
		this.powerUp = powerUp;
		icon.sprite = powerUp.info.icon;
		purchaseLimit = powerUp.info.maxStacks;
		cost = powerUp.info.cost;
		GetComponent<ScrollingTextOption> ().text = powerUp.info.description;
		if (powerUp.info.maxStacks == 1)
			stacksIndicator.transform.parent.gameObject.SetActive (false);
	}

	void OnEnable()
	{
		stacksIndicator.text = ToRomanNumeral(timesPurchased + 1);
		if (timesPurchased > 0 && !powerUp.info.stackDescription.Equals(""))
		{
			GetComponent<ScrollingTextOption> ().text = powerUp.info.stackDescription;
		}
	}

	public override void Upgrade (Player player)
	{
		base.Upgrade (player);
		player.hero.powerUpHolder.AddPowerUp (powerUp.info.powerUpName);
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

