using UnityEngine;
using System.Collections;

public class ShopNPC : MonoBehaviour
{
	public Shop shop;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			shop.AnimateIn ();
		}
	}
}
