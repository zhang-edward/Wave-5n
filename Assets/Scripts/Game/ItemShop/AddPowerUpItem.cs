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
		base.OnPurchased (player);
		player.hero.powerUpHolder.AddPowerUp (powerUp.powerUpName);
	}
}

