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
	private int upgradesLeft;
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
	public Button confirmButton;		// Button which confirms the upgrade
	public GameObject upgradeButton;    // Button which opens this menu
	public Text numUpgradesText; 		// Text indicating how many upgrades are left

	private PowerUpItemsHolder itemsHolder;
	private Animator animator;

	void Awake()
	{
		itemsHolder = GetComponent<PowerUpItemsHolder> ();
		animator = GetComponent<Animator> ();
		player.OnPlayerInitialized += InitShopItemsHolder;
		player.OnPlayerUpgradesUpdated += (numUpgrades) => {
			upgradesLeft += numUpgrades;
			if (upgradesLeft > itemsHolder.NumUpgradesLeft())
				upgradesLeft = itemsHolder.NumUpgradesLeft();
			
			numUpgradesText.text = upgradesLeft.ToString();
			if (upgradesLeft > 0)
			{
				upgradeButton.SetActive(true);
			}
		};
	}

	// Get a list of potential shop items (create addPowerUpItems from the player's hero)
	private void InitShopItemsHolder()
	{
		itemsHolder.InitShopItemsList (player.hero);
	}

	// Instantiate a random selection of shop items from the potential items list
	public void GetShopItems()
	{
		// Set up the shop items holder
		itemsHolder.GetRandomShopItems (numShopItems);
		// Get the active shop items and add them to the list
		foreach (GameObject o in itemsHolder.potentialShopItems)
		{
			if (o.activeInHierarchy)
				items.Add(o.GetComponent<PowerUpItem>());
		}
	}

	public void RefreshItems()
	{
		GetShopItems();
	}

	public void AnimateIn()
	{
		ResetToggles ();
		animator.SetTrigger ("In");
		confirmButton.interactable = true;
		// Hard override input for player
		player.input.enabled = false;

		print("Upgrades left:" + itemsHolder.NumUpgradesLeft());
	}

	public void AnimateOut()
	{
		animator.SetTrigger ("Out");
		confirmButton.interactable = false;
		// Hard override input for player
		player.input.enabled = true;
		if (upgradesLeft <= 0)
			upgradeButton.SetActive(false);
		// shopNPC.Disappear ();
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
		ResetToggles();
		
		upgradesLeft--;
		if (upgradesLeft <= 0)
		{
			AnimateOut();
		}
		else
		{
			RefreshItems();
		}
	}
}

