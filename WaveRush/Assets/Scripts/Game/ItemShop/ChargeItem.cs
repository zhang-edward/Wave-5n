using UnityEngine;
using System.Collections;

public class ChargeItem : UpgradeItem
{
	public override void Upgrade(Player player)
	{
		player.hero.IncrementSpecialAbilityCharge (20);
	}
}

