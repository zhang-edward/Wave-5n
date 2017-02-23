using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ShopItemProgression : UpgradeItem
{
	public List<ShopItemProgression> unlockable = new List<ShopItemProgression> ();

	public override void Upgrade (Player player)
	{
		base.Upgrade (player);
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

