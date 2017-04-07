using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerUpMenu : MonoBehaviour
{
	[Header("Find in Hierarchy")]
	public ShopNPC shopNPC;
	public Player player;

	[Header("Shop Items")]
	public int numShopItems = 3;
	private List<PowerUpItem> items = new List<PowerUpItem>();
	public PowerUpItem selectedItem {
		get {
			foreach (PowerUpItem item in items)
				if (item.Selected)
					return item;
			return null;
		}
	}
	[Header("Set by Prefab")]
	public Button upgradeButton;

	private PowerUpItemsHolder itemsHolder;
	private Animator animator;

	void Awake()
	{
		itemsHolder = GetComponent<PowerUpItemsHolder> ();
		animator = GetComponent<Animator> ();
		player.OnPlayerInitialized += InitShopItemsHolder;
	}

	// get a list of potential shop items (create addPowerUpItems from the player's hero)
	private void InitShopItemsHolder()
	{
		itemsHolder.InitShopItemsList (player.hero);
	}

	// instantiate a random selection of 5 shop items from the potential items list
	public void GetShopItems()
	{
		// set up the shop items holder
		itemsHolder.ResetShopItems ();
		itemsHolder.GetRandomShopItems (numShopItems);
		// get the active shop items and add them to the list
		foreach (GameObject o in itemsHolder.potentialShopItems)
		{
			if (o.activeInHierarchy)
				items.Add (o.GetComponent<PowerUpItem> ());
		}
	}

	public void AnimateIn()
	{
		ResetToggles ();
		animator.SetTrigger ("In");
		upgradeButton.interactable = true;
		// Hard override input for player
		player.input.enabled = false;
	}

	public void AnimateOut()
	{
		animator.SetTrigger ("Out");
		upgradeButton.interactable = false;
		// Hard override input for player
		player.input.enabled = true;
		shopNPC.Disappear ();
	}

	private void ResetToggles()
	{
		foreach (PowerUpItem item in items)
		{
			Toggle toggle = item.GetComponent<Toggle> ();
			toggle.isOn = false;
		}
	}

	public void SelectItem()
	{
		if (selectedItem == null)
			return;
		selectedItem.Upgrade (player);
		AnimateOut ();
	}
}

