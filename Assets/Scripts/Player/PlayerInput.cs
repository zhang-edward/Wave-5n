using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public Player player;

	private float timeInputHeldDown;

	public bool isInputEnabled = true;	
	private Vector3 calibratedAccelerometer;
	private Vector3 accel = Input.acceleration;

	void Start()
	{
		CalibrateAccelerometer ();
	}

	private void CalibrateAccelerometer()
	{
		calibratedAccelerometer = Input.acceleration;
		//calibratedAccelerometer = new Vector2(0, -0.f);
		//Debug.Log (calibratedAccelerometer);

	}

	void Update()
	{
		/*float vx = Input.GetAxisRaw ("Horizontal");
		float vy = Input.GetAxisRaw ("Vertical");*/
		Vector2 movementDir;

		if (isInputEnabled)
		{
#if UNITY_ANDROID
			// get accelerometer input
			accel = (Vector2)Vector3.Lerp(accel, Input.acceleration - calibratedAccelerometer, 10f * Time.deltaTime);

			float x = Mathf.Abs(accel.x) < 0.005f ? 0 : accel.x;
			float y = Mathf.Abs(accel.y) < 0.005f ? 0 : accel.y;
			movementDir = new Vector2(x, y);
#else
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			movementDir = (mousePosition - transform.position);
			if (Vector2.Distance (mousePosition, transform.position) < 0.1f)
				movementDir *= 0;
#endif
			if (Input.GetMouseButton(0))
			{
				player.ability.AbilityHoldDown ();
			}
			if (Input.GetMouseButtonUp (0))
			{
				player.ability.Ability ();
			}
			player.dir = movementDir;
			player.body.Move (movementDir.normalized);
		}
	}
}
