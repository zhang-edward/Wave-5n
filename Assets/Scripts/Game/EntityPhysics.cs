﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class EntityPhysics : MonoBehaviour {

	private Rigidbody2D rb2d;
	public SpriteRenderer sr;

	public Rigidbody2D Rb2d {
		get {
			return rb2d;
		}
	}

	public float moveSpeed;

	//public float hitDisabled;

	void Awake()
	{
		rb2d = this.GetComponent<Rigidbody2D> ();
		//rb2d.drag = 1;
		rb2d.freezeRotation = true;
	}

	public void Move(Vector2 dir)
	{
		rb2d.velocity = (dir * moveSpeed);
		if (rb2d.velocity.x < -0.1f)
			sr.flipX = true;
		else if (rb2d.velocity.x > 0.1f)
			sr.flipX = false;
	}

	public void AddRandomImpulse(float amt)
	{
		/*Vector3 dirAsVector3 = (Vector3)dir;
		Vector3 perpendicularVector = Vector3.Cross (Vector3.forward, dirAsVector3);

		Debug.Log (perpendicularVector);

		rb2d.AddForce ((Vector2)perpendicularVector, ForceMode2D.Impulse);*/
		rb2d.AddForce (new Vector2 (Random.Range (-amt, amt), Random.Range (-amt, amt)), ForceMode2D.Impulse);
	}
}
