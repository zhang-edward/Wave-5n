using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public EntityPhysics body;
	public Player player;

	public bool isInputEnabled = true;

	void Update()
	{
		/*float vx = Input.GetAxisRaw ("Horizontal");
		float vy = Input.GetAxisRaw ("Vertical");*/
		Vector2 movementDir;

		if (isInputEnabled)
		{
#if UNITY_ANDROID
			// get accelerometer input
			Vector2 accel = (Vector2)Input.acceleration;

			float x = Mathf.Abs(accel.x) < 0.01f ? 0 : accel.x;
			float y = Mathf.Abs(accel.y) < 0.01f ? 0 : accel.y;
			movementDir = new Vector2(x, y).normalized;
#else
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			movementDir = (mousePosition - transform.position);
			if (Vector2.Distance (mousePosition, transform.position) < 0.1f)
				movementDir *= 0;
#endif
			if (Input.GetMouseButtonDown (0))
			{
				player.PrimaryAbility ();
			}
			body.Move (movementDir.normalized);
		}
	}
}
