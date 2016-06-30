using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class EntityPhysics : MonoBehaviour {

	private Rigidbody2D rb2d;
	public SpriteRenderer sr;
	public float moveSpeed;

	void Awake()
	{
		rb2d = this.GetComponent<Rigidbody2D> ();
		rb2d.drag = 1;
		rb2d.freezeRotation = true;
	}

	public void move(Vector2 dir)
	{
		rb2d.velocity = (dir * moveSpeed);
	}

	void Update()
	{
		if (rb2d.velocity.x < 0)
			sr.flipX = true;
		else if (rb2d.velocity.x > 0)
			sr.flipX = false;
	}
}
