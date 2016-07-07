using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class EntityPhysics : MonoBehaviour {

	private Rigidbody2D rb2d;
	public SpriteRenderer sr;

	public Vector2 velocity {
		get {
			return rb2d.velocity;
		}
	}

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

	public void HitDisable(Vector2 dir)
	{
		/*Vector3 dirAsVector3 = (Vector3)dir;
		Vector3 perpendicularVector = Vector3.Cross (Vector3.forward, dirAsVector3);

		Debug.Log (perpendicularVector);

		rb2d.AddForce ((Vector2)perpendicularVector, ForceMode2D.Impulse);*/
		rb2d.AddForce (new Vector2 (Random.Range (-2, 3), Random.Range (-2, 3)), ForceMode2D.Impulse);
	}

	void Update()
	{
		if (rb2d.velocity.x < -0.5f)
			sr.flipX = true;
		else if (rb2d.velocity.x > 0.5f)
			sr.flipX = false;
	}
}
