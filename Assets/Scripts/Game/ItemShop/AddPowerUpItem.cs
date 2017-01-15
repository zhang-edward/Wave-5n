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
		icon.sprite = powerUp.icon;
		purchaseLimit = powerUp.maxStacks;
		cost = powerUp.cost;
		GetComponent<ScrollingTextOption> ().text = powerUp.description;
		if (powerUp.maxStacks == 1)
			stacksIndicator.transform.parent.gameObject.SetActive (false);
	}

	void OnEnable()
	{
		stacksIndicator.text = ToRomanNumeral(timesPurchased + 1);
		if (timesPurchased > 0 && !powerUp.stackDescription.Equals(""))
		{
			GetComponent<ScrollingTextOption> ().text = powerUp.stackDescription;
		}
	}

	public override void OnPurchased (Player player)
	{
		base.OnPurchased (player);
		player.hero.powerUpHolder.AddPowerUp (powerUp.powerUpName);
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

