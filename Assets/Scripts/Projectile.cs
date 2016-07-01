using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	private Rigidbody2D rb2d;

	void Awake()
	{
		rb2d = this.GetComponent<Rigidbody2D> ();
	}

	public void Init(Vector3 pos, Vector2 dir)
	{
		transform.position = pos;
		rb2d.velocity = dir * 5;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3 (0, 0, angle);
	}
}

