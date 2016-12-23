using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class ShopItem : MonoBehaviour
{
	public int cost;

	public int timesPurchased;
	public int purchaseLimit;
	public bool available { get; protected set; }

	public bool Selected {
		get {
			return GetComponent<Toggle> ().isOn;
		}
	}

	void Awake()
	{
		available = true;
	}

	public abstract void OnPurchased (Player player);
}

