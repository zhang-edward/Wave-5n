using UnityEngine;
using System.Collections;

public class ChargeItem : PowerUpItem
{
	public override void Upgrade(Player player)
	{
		player.hero.IncrementSpecialAbilityCharge (20);
	}
}

