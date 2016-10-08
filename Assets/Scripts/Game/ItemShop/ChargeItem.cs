using UnityEngine;
using System.Collections;

public class ChargeItem : ShopItem
{
	public override void OnPurchased(Player player)
	{
		player.hero.IncrementSpecialAbilityCharge (20);
	}
}

