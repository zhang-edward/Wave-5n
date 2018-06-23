// using UnityEngine;
// using System.Collections;

// public class UpgradePickup : MonoBehaviour
// {
// 	void OnTriggerEnter2D(Collider2D col)
// 	{
// 		if (col.CompareTag("Player"))
// 		{
// 			//Debug.Break();
// 			Player player = col.GetComponentInChildren<Player>();
// 			player.AddUpgrades(2);
// 			CameraControl.instance.StartFlashColor(Color.white, 0.4f, 0, 0, 1f);
// 			Destroy(gameObject);
// 		}
// 	}
// }
