using UnityEngine;
using System.Collections;

public class HealthItem : ShopItem
{
	public override void OnPurchased(Player player)
	{
		player.Heal (1);
	}
}

