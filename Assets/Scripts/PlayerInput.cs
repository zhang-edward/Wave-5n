using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public EntityPhysics body;
	public Player player;

	public bool mouseMovement = true;

	void Update()
	{
		/*float vx = Input.GetAxisRaw ("Horizontal");
		float vy = Input.GetAxisRaw ("Vertical");*/

		if (mouseMovement)
		{
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			Vector2 movementDir = (mousePosition - transform.position);
			if (Vector2.Distance (mousePosition, transform.position) < 0.1f)
				movementDir *= 0;
			//Debug.Log (movementDir.normalized);

			if (Input.GetMouseButtonDown (0))
			{
				player.PrimaryAbility ();
			}
			body.move (movementDir.normalized);
		}
	}
}
