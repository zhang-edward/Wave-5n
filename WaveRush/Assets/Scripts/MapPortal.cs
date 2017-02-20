using UnityEngine;
using System.Collections;

public class MapPortal : MonoBehaviour {

	public SimpleAnimation appearAnim;
	public SimpleAnimation loop;
	public SimpleAnimationPlayer anim;

	private bool activated = false;

	void Start()
	{
		StartCoroutine ("UpdatePortal");
	}

	private IEnumerator UpdatePortal()
	{
		anim.anim = appearAnim;
		anim.looping = false;
		anim.Play ();
		while (anim.isPlaying)
			yield return null;

		anim.looping = true;
		anim.anim = loop;
		anim.Play ();
		activated = true;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag ("Player") && activated)
		{
			Debug.Log ("MapPortal collided");
			Player player = col.GetComponentInChildren<Player> ();
			player.body.Move (Vector2.zero);
			GameManager.instance.TeleportMaps ("hell");
		}
	}
}
