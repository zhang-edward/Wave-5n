using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddPowerUpItem : ShopItemProgression
{
	public Image icon;
	private HeroPowerUp powerUp;

	public void Init(HeroPowerUp powerUp)
	{
		this.powerUp = powerUp;
		icon.sprite = powerUp.icon;
		purchaseLimit = powerUp.maxStacks;
		cost = powerUp.cost;
		GetComponent<ScrollingTextOption> ().text = powerUp.description;
	}

	public override void OnPurchased (Player player)
	{
		player.hero.powerUpHolder.AddPowerUp (powerUp.powerUpName);
		timesPurchased++;
		if (timesPurchased >= purchaseLimit)
		{
			available = false;
		}
	}
}

