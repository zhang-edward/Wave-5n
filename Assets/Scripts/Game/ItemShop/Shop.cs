using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
	public ShopNPC shopNPC;
	private Animator animator;
	public Player player;
	public List<ShopItem> shopItems;
	public ShopItem selectedItem {
		get {
			foreach (ShopItem shopItem in shopItems)
				if (shopItem.Selected)
					return shopItem;
			return null;
		}
	}
	public Button purchaseButton;

	void Awake()
	{
		animator = GetComponent<Animator> ();
	}

	public void AnimateIn()
	{
		animator.SetTrigger ("In");
		// Hard override input for player
		player.input.enabled = false;
	}

	public void AnimateOut()
	{
		animator.SetTrigger ("Out");
		// Hard override input for player
		player.input.enabled = true;
		shopNPC.Disappear ();
	}

	public void purchaseSelected()
	{
		Wallet wallet = GameManager.instance.wallet;
		if (wallet.money + wallet.moneyEarned > selectedItem.cost)
		{
			wallet.Spend (selectedItem.cost);
			selectedItem.OnPurchased (player);
		}
	}
}

