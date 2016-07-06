using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class EntityPhysics : MonoBehaviour {

	private Rigidbody2D rb2d;
	public SpriteRenderer sr;
	public float moveSpeed;

	//public float hitDisabled;

	void Awake()
	{
		rb2d = this.GetComponent<Rigidbody2D> ();
		rb2d.drag = 1;
		rb2d.freezeRotation = true;
	}

	public void Move(Vector2 dir)
	{
		rb2d.velocity = (dir * moveSpeed);
	}

	public void HitDisable()
	{
		rb2d.velocity = (new Vector2(
			Random.Range(-5, 6), 
			Random.Range(-5, 6)));
	}

	void Update()
	{
		if (rb2d.velocity.x < 0)
			sr.flipX = true;
		else if (rb2d.velocity.x > 0)
			sr.flipX = false;
	}
}
