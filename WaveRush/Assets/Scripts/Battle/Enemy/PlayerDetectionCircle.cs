using UnityEngine;
using System.Collections.Generic;

public class PlayerDetectionCircle : MonoBehaviour
{
	public SpriteRenderer sr;
	public CollisionDetector collision;
	public bool dynamic;
	//public float radius;
	private Vector3 pos;
	private IDamageable playerDamageable;
	

	void Awake()
	{
		pos = transform.localPosition;
		collision.OnTriggerEnter += TriggerEnter;
		collision.OnTriggerExit += TriggerExit;
	}

	void Update()
	{
		if (dynamic)
			return;
		if (sr.flipX)
			transform.localPosition = new Vector3(pos.x * -1, pos.y);
		else
			transform.localPosition = new Vector3(pos.x, pos.y);
	}

	public IDamageable Activate() {
		return playerDamageable;
	}

	private void TriggerEnter(Collider2D col) {
		if (col.CompareTag("Player")) {
			playerDamageable = col.GetComponentInChildren<IDamageable>();
		}
	}

	private void TriggerExit(Collider2D col) {
		if (col.CompareTag("Player")) {
			playerDamageable = null;
		}
	}
}