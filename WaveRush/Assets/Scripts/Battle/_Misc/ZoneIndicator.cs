using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneIndicator : MonoBehaviour {

	public CircleCollider2D circleCollider;

	void Update()
	{
		transform.localScale = Vector3.one * (circleCollider.radius / 1.5f);
	}
}
