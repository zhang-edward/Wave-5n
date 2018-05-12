using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	public const float INPUT_POSITION_SCALAR = 5;

	public Player player;
	public TouchInputHandler touchInputHandler;
	public bool isInputEnabled = true;

	private Vector2 pointerStartPos;

	//private Vector3 calibratedAccelerometer;
	//private Vector3 accel;
	//public float tiltSensitivity = 10f;

	void Start()
	{
		Invoke("CalibrateAccelerometer", 0.5f);
		touchInputHandler.enabled = true;
		touchInputHandler.OnDragBegan += HandleDragBegan;
		touchInputHandler.OnDragMove += HandleDragHold;
		touchInputHandler.OnDragRelease += HandleDragRelease;
		touchInputHandler.OnTap += HandleTap;
		touchInputHandler.OnTapHold += HandleTapHold;
		touchInputHandler.OnTapHoldRelease += HandleTapHoldRelease;
		touchInputHandler.MultiTouch += player.hero.HandleMultiTouch;
		touchInputHandler.OnDragCancel += player.hero.HandleDragCancel;
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
		if (EventSystem.current.IsPointerOverGameObject())
			return;
		Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition) * INPUT_POSITION_SCALAR;
		if (Input.GetMouseButtonDown(0))
		{
			pointerStartPos = mousePosition;
			touchInputHandler.HandleTouchBegan(pointerStartPos);
		}
		if (Input.GetMouseButton(0))
		{
			/*timeInputHeldDown += Time.deltaTime;
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			player.dir = ((Vector2)(mousePos - transform.position));
			player.hero.HandleHoldDown();*/
			if (Vector2.Distance(pointerStartPos, mousePosition) > 0.05f)
				touchInputHandler.HandleTouchMoved(mousePosition);
			else
				touchInputHandler.HandleTouchHeld(mousePosition);
		}
		if (Input.GetMouseButtonUp(0))
		{
			/*if (timeInputHeldDown < 0.3f)
				player.hero.HandleTap();
			player.hero.HandleTapRelease();
			timeInputHeldDown = 0;*/
			touchInputHandler.HandleTouchEnded(mousePosition);
		}
		/*if (Input.GetMouseButton(1))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			player.dir = (mousePos - transform.position);
			player.hero.HandleDragHold();
		}
		if (Input.GetMouseButtonUp(1))
		{
			player.hero.HandleDrag();
		}*/
		if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
		{
			player.hero.HandleMultiTouch();
		}
	}

	private void HandleDragBegan(Vector3 dir)
	{
		player.dir = dir;
		player.hero.HandleDragBegan();
	}

	private void HandleDragHold(Vector3 dir)
	{
		player.dir = dir;
		player.hero.HandleDragHold();
	}

	private void HandleDragRelease(Vector3 dir)
	{
		player.dir = dir;
		//Debug.DrawRay(transform.position, dir, Color.white, 0.5f);
		player.hero.HandleDragRelease();
	}

	private void HandleTapHold(Vector3 pos)
	{
		player.dir = pos - transform.position;
		player.hero.HandleHoldDown();
	}

	private void HandleTap(Vector3 pos)
	{
		player.dir = pos - transform.position;
		player.hero.HandleTap();
	}

	private void HandleTapHoldRelease(Vector3 pos)
	{
		player.dir = pos - transform.position;
		player.hero.HandleTapHoldRelease();
	}
}