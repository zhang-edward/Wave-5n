using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class EntityPhysics : MonoBehaviour {

	public Rigidbody2D rb2d { get; private set; }
	public SpriteRenderer sr;
	public float moveSpeed;

	//public float hitDisabled;

	void Awake()
	{
		rb2d = this.GetComponent<Rigidbody2D> ();
		rb2d.freezeRotation = true;
	}

	public void Move(Vector2 dir)
	{
		StartCoroutine(MoveRoutine(dir, 0));
	}

	public void Move(Vector2 dir, float time)
	{
		StartCoroutine(MoveRoutine(dir, time));
	}

	private IEnumerator MoveRoutine(Vector2 dir, float time)
	{
		float t = 0;
		// Do-while loop so this executes at least once, even if time = 0
		do
		{
			rb2d.velocity = (dir * moveSpeed);
			if (rb2d.velocity.x < -0.1f)
				sr.flipX = true;
			else if (rb2d.velocity.x > 0.1f)
				sr.flipX = false;
			t += Time.deltaTime;
			yield return null;
		} while (t < time);

	}

	public void AddRandomImpulse(float amt)
	{
		rb2d.AddForce (new Vector2 (Random.Range (-amt, amt), Random.Range (-amt, amt)), ForceMode2D.Impulse);
	}
}
