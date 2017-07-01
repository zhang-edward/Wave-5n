using UnityEngine;
using System.Collections;

public class UpgradePickup : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			Player player = col.GetComponentInChildren<Player>();
			player.AddUpgrades(2);
			CameraControl.instance.StartFlashColor(Color.white);
			Destroy(gameObject);
		}
	}
}
