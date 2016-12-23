using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShopItemsHolder : MonoBehaviour
{
	public GameObject[] universalShopItems;

	[System.Serializable]
	public class HeroItemSet
	{
		public string heroName;
		public GameObject[] shopItemPrefabs;
	}
	public HeroItemSet[] heroItemSets;
	public List<GameObject> shopItems;

	public Transform shopItemPanel;
	public ScrollingText scrollingText;
	public ToggleGroup toggleGroup;

	public void InitShopItemsList (string heroName)
	{
		foreach (GameObject item in universalShopItems)
		{
			CreateShopItem (item);
		}
		foreach (GameObject item in GetHeroItemSet(heroName).shopItemPrefabs)
		{
			CreateShopItem (item);
		}
	}

	public void UpdateShopItemsList()
	{
		for (int i = 0; i < shopItems.Count; i ++)
		{
			ShopItem item = shopItems [i].GetComponent<ShopItem>();
			if (item as ShopItemProgression != null)
			{
				ShopItemProgression itemProgression = item as ShopItemProgression;
				// if item has been purchased
				if (itemProgression.timesPurchased > 1)
				{
					// make all items in unlockable list available for purchase in the next round of shop items
					foreach (GameObject item2 in itemProgression.unlockable)
					{
						CreateShopItem (item2);
					}
				}
			}
		}
	}

	public void GetRandomShopItems(int count)
	{
		count = Mathf.Min (count, shopItems.Count);	// if the available shop items < count, only return the number of available shop items
		for (int i = 0; i < count; i ++)
		{
			print (i);
			int debugCounter = 0;
			while (!TryEnableRandomShopItem () && debugCounter < 1000)
				debugCounter++;
			if (debugCounter > 1000)
				Debug.LogError ("1000+ tries to enable shop items in ShopItemsHolder!");
		}
	}

	public void ResetShopItems()
	{
		foreach (GameObject o in shopItems)
		{
			o.SetActive (false);
		}
	}

	private bool TryEnableRandomShopItem()
	{
		int i = Random.Range (0, shopItems.Count);
		ShopItem shopItem = shopItems [i].GetComponent<ShopItem>();
		if (!shopItem.gameObject.activeInHierarchy && shopItem.available)
		{
			print ("Enabled " + shopItems [i]);
			shopItems [i].gameObject.SetActive (true);
			return true;
		}
		return false;
	}

	private HeroItemSet GetHeroItemSet(string heroName)
	{
		foreach (HeroItemSet itemSet in heroItemSets)
		{
			if (itemSet.heroName.Equals (heroName))
				return itemSet;
		}
		throw new UnityEngine.Assertions.AssertionException ("ShopItemsHolder.cs", 
			"Cannot find HeroItemSet with name " + "\"" + heroName + "\"");
	}

	private void CreateShopItem(GameObject prefab)
	{
		GameObject o = Instantiate (prefab);
		o.transform.SetParent (shopItemPanel, false);
		o.GetComponent<ScrollingTextOption> ().scrollingText = scrollingText;
		o.GetComponent<Toggle> ().group = toggleGroup;
		shopItems.Add (o);
		o.SetActive (false);
	}
}

