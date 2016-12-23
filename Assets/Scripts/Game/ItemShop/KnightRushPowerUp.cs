using UnityEngine;
using System.Collections;

public class KnightRushPowerUp : ShopItem
{
	public override void OnPurchased (Player player)
	{
		UnityEngine.Assertions.Assert.IsNotNull (player.hero as KnightHero);
		KnightHero knight = player.hero as KnightHero;
		knight.rushMoveSpeed *= 1.1f;
		timesPurchased++;
		if (timesPurchased >= 3)
		{
			available = false;
		}
	}
}

