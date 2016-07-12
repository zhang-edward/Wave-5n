using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	private Rigidbody2D rb2d;
	public string target;
	private int damage;
	//private int speed;


	void Awake()
	{
		rb2d = this.GetComponent<Rigidbody2D> ();
	}

	public void Init(Vector3 pos, Vector2 dir, int speed, string target, int damage)
	{
		transform.position = pos;
		rb2d.velocity = dir * speed;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3 (0, 0, angle);

		this.target = target;
		this.damage = damage;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag(target))
		{
			//Debug.Log (col);
			IDamageable damageableTarget = col.GetComponentInChildren<IDamageable> ();
			damageableTarget.Damage (damage);
		}
	}
}

