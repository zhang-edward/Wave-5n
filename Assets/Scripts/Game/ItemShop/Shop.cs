using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
	[Header("Find in Hierarchy")]
	public ShopNPC shopNPC;
	public Player player;
	[Header("Shop Items")]
	private List<ShopItem> shopItems = new List<ShopItem>();
	public ShopItem selectedItem {
		get {
			foreach (ShopItem shopItem in shopItems)
				if (shopItem.Selected)
					return shopItem;
			return null;
		}
	}
	[Header("Set by Prefab")]
	public Button purchaseButton;

	private ShopItemsHolder shopItemsHolder;
	private Animator animator;

	void Awake()
	{
		shopItemsHolder = GetComponent<ShopItemsHolder> ();
		animator = GetComponent<Animator> ();
		player.OnPlayerInitialized += InitShopItemsHolder;
	}

	private void InitShopItemsHolder()
	{
		shopItemsHolder.InitShopItemsList (player.hero);
	}

	// 
	public void GetShopItems()
	{
		shopItemsHolder.ResetShopItems ();
		shopItemsHolder.UpdateShopItemsList ();		// update available shop item pool
		shopItemsHolder.GetRandomShopItems (5);
		foreach (GameObject o in shopItemsHolder.shopItems)
		{
			if (o.activeInHierarchy)
				shopItems.Add (o.GetComponent<ShopItem> ());
		}
	}

	public void AnimateIn()
	{
		animator.SetTrigger ("In");
		purchaseButton.interactable = true;
		// Hard override input for player
		player.input.enabled = false;
	}

	public void AnimateOut()
	{
		Invoke ("ResetToggles", 2.0f);
		animator.SetTrigger ("Out");
		purchaseButton.interactable = false;
		// Hard override input for player
		player.input.enabled = true;
		shopNPC.Disappear ();
	}

	public void purchaseSelected()
	{
		if (selectedItem == null)
			return;
		Wallet wallet = GameManager.instance.wallet;
		if (wallet.TrySpend (selectedItem.cost))
		{
			selectedItem.OnPurchased (player);
		}
		AnimateOut ();
	}

	private void ResetToggles()
	{
		foreach (ShopItem item in shopItems)
		{
			Toggle toggle = item.GetComponent<Toggle> ();
			toggle.isOn = false;
		}
	}
}

