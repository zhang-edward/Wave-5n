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
	public Text priceText;

	private ShopItemsHolder shopItemsHolder;
	private Animator animator;

	void Awake()
	{
		shopItemsHolder = GetComponent<ShopItemsHolder> ();
		animator = GetComponent<Animator> ();
		player.OnPlayerInitialized += InitShopItemsHolder;
	}

	void Update()
	{
		if (selectedItem != null)
			priceText.text = selectedItem.cost.ToString ();
		else
			priceText.text = "";
	}

	// get a list of potential shop items (create addPowerUpItems from the player's hero)
	private void InitShopItemsHolder()
	{
		shopItemsHolder.InitShopItemsList (player.hero);
	}

	// instantiate a random selection of 5 shop items from the potential items list
	public void GetShopItems()
	{
		// set up the shop items holder
		shopItemsHolder.ResetShopItems ();
		shopItemsHolder.GetRandomShopItems (5);
		// get the active shop items and add them to the list
		foreach (GameObject o in shopItemsHolder.potentialShopItems)
		{
			if (o.activeInHierarchy)
				shopItems.Add (o.GetComponent<ShopItem> ());
		}
	}

	public void AnimateIn()
	{
		ResetToggles ();
		animator.SetTrigger ("In");
		purchaseButton.interactable = true;
		// Hard override input for player
		player.input.enabled = false;
	}

	public void AnimateOut()
	{
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

