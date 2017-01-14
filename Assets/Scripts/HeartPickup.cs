using UnityEngine;
using System.Collections;

public class HeartPickup : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			Player player = col.GetComponentInChildren<Player> ();
			UnityEngine.Assertions.Assert.IsNotNull (player);
			player.HealEffect (4, true);
			CameraControl.instance.StartFlashColor (Color.white);
			Destroy (gameObject);
		}
	}
}
