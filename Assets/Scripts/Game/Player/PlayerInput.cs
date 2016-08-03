using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public Player player;

	private float timeInputHeldDown;

	public bool isInputEnabled = true;	

	public TouchInputHandler touchInputHandler;
	private Vector3 calibratedAccelerometer;
	private Vector3 accel;
	public float tiltSensitivity = 10f;

	void Start()
	{
#if UNITY_ANDROID
		Invoke("CalibrateAccelerometer", 0.5f);
		touchInputHandler.enabled = true;
		touchInputHandler.OnSwipe += HandleSwipe;
		touchInputHandler.OnTapHold += HandleTapHold;
		touchInputHandler.OnTapRelease += HandleTapRelease;
#endif
	}



	private void CalibrateAccelerometer()
	{
		calibratedAccelerometer = Input.acceleration;
		//calibratedAccelerometer = new Vector2(0, -0.f);
		//Debug.Log (calibratedAccelerometer);

	}

	void Update()
	{
		Vector2 movementDir;
		if (isInputEnabled)
		{
#if UNITY_ANDROID
			// get accelerometer input
			accel = (Vector2)Vector3.Lerp(accel, Input.acceleration - calibratedAccelerometer, 10f * Time.deltaTime);

			float x = Mathf.Abs(accel.x) < 0.01f ? 0 : accel.x;
			float y = Mathf.Abs(accel.y) < 0.01f ? 0 : accel.y;
			movementDir = new Vector2(x, y) * tiltSensitivity;
			movementDir = Vector2.ClampMagnitude(movementDir, 2);
#else
			/*Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			movementDir = (mousePosition - transform.position);
			if (Vector2.Distance (mousePosition, transform.position) < 0.1f)
				movementDir *= 0;*/
			movementDir = new Vector3(Input.GetAxisRaw("Horizontal"),
				Input.GetAxisRaw("Vertical"), 0);
#endif
			//player.body.Move (movementDir);

			// Get mouse or touch input
#if UNITY_ANDROID
			touchInputHandler.ListenForTouchInput();
#else
			HandleMouseInput();
#endif
		}
	}

	public void HandleMouseInput()
	{
		if (Input.GetMouseButton(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			player.dir = ((Vector2)(mousePos - transform.position));
			player.hero.HandleHoldDown ();
		}
		if (Input.GetMouseButtonUp (0))
		{
			player.hero.HandleTapRelease ();
		}
		if (Input.GetMouseButtonDown(1))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			player.dir = ((Vector2)(mousePos - transform.position));
			player.hero.HandleSwipe ();
		}
	}

	private void HandleSwipe(Vector2 dir)
	{
		player.dir = dir;
		player.hero.HandleSwipe ();
	}

	private void HandleTapHold(Vector3 pos)
	{
		player.dir = pos - transform.position;
		player.hero.HandleHoldDown ();
	}

	private void HandleTapRelease (Vector3 pos)
	{
		player.dir = pos - transform.position;
		player.hero.HandleTapRelease ();
	}
}
