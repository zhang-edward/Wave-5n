using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class ShopItem : MonoBehaviour
{
	public int cost;
	public abstract void OnPurchased (Player player);

	public bool Selected {
		get {
			return GetComponent<Toggle> ().isOn;
		}
	}
}

