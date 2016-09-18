using UnityEngine;
using System.Collections;

public class HealthItem : MonoBehaviour, ShopItem
{
	public void OnPurchased(Player player)
	{
		player.Heal (1);
	}
}

