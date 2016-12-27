using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShopItemsHolder : MonoBehaviour
{
	public GameObject[] universalShopItems;

	/*[System.Serializable]
	public class HeroItemSet
	{
		public string heroName;
		public GameObject[] shopItemPrefabs;
	}*/
	public GameObject addPowerUpItemPrefab;
	//public HeroItemSet[] heroItemSets;
	public List<GameObject> potentialShopItems;		// available shop items after initialization

	public Transform shopItemPanel;
	public ScrollingText scrollingText;
	public ToggleGroup toggleGroup;

	public void InitShopItemsList (PlayerHero hero)
	{
		string heroName = hero.heroName;
		foreach (GameObject item in universalShopItems)
		{
			CreateShopItem (item);
		}
		foreach (HeroPowerUpHolder.HeroPowerUpDictionaryEntry entry in hero.powerUpHolder.powerUpPrefabs)
		{
			HeroPowerUp powerUp = entry.powerUpPrefab.GetComponent<HeroPowerUp> ();
			if (powerUp.parent == null)		// if this powerup is not dependent on another powerup (has no parent to be unlocked first)
				CreateAddPowerUpShopItem (powerUp);
		}
	}

	private GameObject CreateAddPowerUpShopItem(HeroPowerUp powerUp)
	{
		GameObject o = CreateShopItem (addPowerUpItemPrefab);
		AddPowerUpItem addPowerUpItem = o.GetComponent<AddPowerUpItem> ();
		addPowerUpItem.Init (powerUp);
		// if the powerup unlocks further powerups
		if (powerUp.unlockable.Length > 0)
		{
			foreach (HeroPowerUp childPowerUp in powerUp.unlockable)
			{
				GameObject child = CreateAddPowerUpShopItem (childPowerUp);	// create a shopItem for the child powerUp
				AddPowerUpItem childItem = child.GetComponent<AddPowerUpItem>();
				childItem.SetAvailable (false);
				addPowerUpItem.unlockable.Add(childItem);
			}
		}
		return o;
	}

	public void GetRandomShopItems(int count)
	{
		count = Mathf.Min (count, potentialShopItems.Count);	// if the available shop items < count, only return the number of available shop items
		for (int i = 0; i < count; i ++)
		{
			int debugCounter = 0;
			while (!TryEnableRandomShopItem () && debugCounter < 1000)
				debugCounter++;
			if (debugCounter > 1000)
				Debug.LogError ("1000+ tries to enable shop items in ShopItemsHolder!");
		}
	}

	public void ResetShopItems()
	{
		foreach (GameObject o in potentialShopItems)
		{
			o.SetActive (false);
		}
	}

	private bool TryEnableRandomShopItem()
	{
		int i = Random.Range (0, potentialShopItems.Count);
		ShopItem shopItem = potentialShopItems [i].GetComponent<ShopItem>();
		if (!shopItem.gameObject.activeInHierarchy && shopItem.available)
		{
			print ("Enabled " + potentialShopItems [i].GetComponent<ScrollingTextOption>().text);
			potentialShopItems [i].gameObject.SetActive (true);
			return true;
		}
		return false;
	}

	private GameObject CreateShopItem(GameObject prefab)
	{
		GameObject o = Instantiate (prefab);
		o.transform.SetParent (shopItemPanel, false);
		o.GetComponent<ScrollingTextOption> ().scrollingText = scrollingText;
		o.GetComponent<Toggle> ().group = toggleGroup;
		o.SetActive (false);
		potentialShopItems.Add (o);
		return o;
	}
}

