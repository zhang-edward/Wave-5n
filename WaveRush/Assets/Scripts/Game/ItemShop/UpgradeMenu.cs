﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeMenu : MonoBehaviour
{
	[Header("Find in Hierarchy")]
	public ShopNPC shopNPC;
	public Player player;

	[Header("Shop Items")]
	private List<UpgradeItem> items = new List<UpgradeItem>();
	public UpgradeItem selectedItem {
		get {
			foreach (UpgradeItem item in items)
				if (item.Selected)
					return item;
			return null;
		}
	}
	[Header("Set by Prefab")]
	public Button upgradeButton;

	private UpgradeItemsHolder itemsHolder;
	private Animator animator;

	void Awake()
	{
		itemsHolder = GetComponent<UpgradeItemsHolder> ();
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
		itemsHolder.GetRandomShopItems (5);
		// get the active shop items and add them to the list
		foreach (GameObject o in itemsHolder.potentialShopItems)
		{
			if (o.activeInHierarchy)
				items.Add (o.GetComponent<UpgradeItem> ());
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
		foreach (UpgradeItem item in items)
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
