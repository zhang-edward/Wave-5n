using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public Player player;

	private float timeInputHeldDown;

	public bool isInputEnabled = true;	

	public TouchInputHandler touchInputHandler;
	//private Vector3 calibratedAccelerometer;
	//private Vector3 accel;
	public float tiltSensitivity = 10f;

	void Start()
	{
#if UNITY_ANDROID || UNITY_IOS || UNITY_IOS
		Invoke("CalibrateAccelerometer", 0.5f);
		touchInputHandler.enabled = true;
		touchInputHandler.OnSwipe += HandleSwipe;
		touchInputHandler.OnTapHold += HandleTapHold;
		touchInputHandler.OnTapRelease += HandleTapRelease;
		touchInputHandler.MultiTouch += HandleMultiTouch;
#endif
	}



	private void CalibrateAccelerometer()
	{
		//calibratedAccelerometer = Input.acceleration;
		//calibratedAccelerometer = new Vector2(0, -0.f);
		//Debug.Log (calibratedAccelerometer);

	}

	void Update()
	{
		//Vector2 movementDir;
		if (isInputEnabled)
		{
#if UNITY_ANDROID || UNITY_IOS
			// get accelerometer input
			//accel = (Vector2)Vector3.Lerp(accel, Input.acceleration - calibratedAccelerometer, 10f * Time.deltaTime);

			//float x = Mathf.Abs(accel.x) < 0.01f ? 0 : accel.x;
			//float y = Mathf.Abs(accel.y) < 0.01f ? 0 : accel.y;
			//movementDir = new Vector2(x, y) * tiltSensitivity;
			//movementDir = Vector2.ClampMagnitude(movementDir, 2);
#else
			/*Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			movementDir = (mousePosition - transform.position);
			if (Vector2.Distance (mousePosition, transform.position) < 0.1f)
				movementDir *= 0;*/
			//movementDir = new Vector3(Input.GetAxisRaw("Horizontal"),
				//Input.GetAxisRaw("Vertical"), 0);
#endif
			//player.body.Move (movementDir);

			// Get mouse or touch input
#if UNITY_ANDROID || UNITY_IOS || UNITY_IOS
			touchInputHandler.ListenForTouchInput();
#else
			HandleMouseKeyboardInput();
#endif
		}
	}

	public void HandleMouseKeyboardInput()
	{
		if (EventSystem.current.IsPointerOverGameObject ())
			return;
		if (Input.GetMouseButton(0))
		{
			timeInputHeldDown += Time.deltaTime;
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			player.dir = ((Vector2)(mousePos - transform.position));
			player.hero.HandleHoldDown ();
		}
		if (Input.GetMouseButtonUp (0))
		{
			if (timeInputHeldDown < 0.3f)
				player.hero.HandleTap ();
			player.hero.HandleTapRelease();
			timeInputHeldDown = 0;
		}
		if (Input.GetMouseButtonDown(1))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			player.dir = ((Vector2)(mousePos - transform.position));
			player.hero.HandleSwipe ();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			HandleMultiTouch();
		}
	}

	private void HandleSwipe(Vector3 dir)
	{
		player.dir = dir;
		Debug.DrawRay (transform.position, dir, Color.white, 0.5f);
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
		player.hero.HandleTap ();
	}

	private void HandleMultiTouch()
	{
		player.hero.HandleMultiTouch();
	}
}
