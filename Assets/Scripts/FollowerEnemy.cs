using UnityEngine;
using System.Collections;

public class FollowerEnemy : MonoBehaviour {

	public Transform player;
	public EntityPhysics body;

	// Update is called once per frame
	void Update () {
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 1f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Player"))
			{
//				Debug.Log ("found player");
			}
		}

		body.move ((player.position - transform.position).normalized);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 1f);
	}
}
