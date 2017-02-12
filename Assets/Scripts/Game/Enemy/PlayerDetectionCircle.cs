using UnityEngine;

public class PlayerDetectionCircle : MonoBehaviour
{
	public Enemy e;
	public float radius;
	private Vector3 pos;

	void Awake()
	{
		pos = transform.localPosition;
	}

	void Update()
	{
		if (e.sr.flipX)
			transform.localPosition = new Vector3(pos.x * -1, pos.y);
		else
			transform.localPosition = new Vector3(pos.x, pos.y);
	}

	public Player Activate()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Player"))
			{
				return col.GetComponentInChildren<Player>();
			}
		}
		return null;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}