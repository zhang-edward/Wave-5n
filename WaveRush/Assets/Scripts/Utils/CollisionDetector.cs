using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour {

	public delegate void CollisionDetected(Collider2D col);
	public event CollisionDetected OnTriggerEnter;
	public event CollisionDetected OnTriggerStay;
	public event CollisionDetected OnTriggerExit;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (OnTriggerEnter != null)
			OnTriggerEnter(col);
	}

	private void OnTriggerStay2D(Collider2D col)
	{
		if (OnTriggerStay != null)
			OnTriggerStay(col);

	}

	private void OnTriggerExit2D(Collider2D col)
	{
		if (OnTriggerExit != null)
			OnTriggerExit(col);

	}
}
