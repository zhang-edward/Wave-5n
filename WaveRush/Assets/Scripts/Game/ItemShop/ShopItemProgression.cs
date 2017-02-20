using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ShopItemProgression : ShopItem
{
	public List<ShopItemProgression> unlockable = new List<ShopItemProgression> ();

	public override void OnPurchased (Player player)
	{
		base.OnPurchased (player);
		foreach (ShopItemProgression item in unlockable)
		{
			item.available = true;
		}
	}

	public void SetAvailable(bool available)
	{
		this.available = available;
	}
}

