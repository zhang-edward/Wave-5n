using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	public const float INPUT_POSITION_SCALAR = 8;

	public Player player;
	public TouchInputHandler touchInputHandler;
	public bool isInputEnabled = true;

	private Vector2 pointerStartPos;

	public void Init()
	{	
		touchInputHandler.ClearListeners();
		touchInputHandler.enabled = true;
		touchInputHandler.OnTouchBegan 		+= SetDirTouchBegan;
		touchInputHandler.OnTouchEnded 		+= SetDirTouchEnded;
		touchInputHandler.OnDragBegan		+= SetDirDragBegan;
		touchInputHandler.OnDragMove 		+= SetDirDragHold;
		touchInputHandler.OnDragRelease 	+= SetDirDragRelease;
		touchInputHandler.OnTap 			+= SetDirTap;
		touchInputHandler.OnTapHold 		+= SetDirTapHold;
		touchInputHandler.OnTapHoldRelease 	+= SetDirTapHoldRelease;
		touchInputHandler.MultiTouch 		+= player.hero.HandleMultiTouch;
		touchInputHandler.OnDragCancel 		+= player.hero.HandleDragCancel;
	}

	void Update()
	{
		//Vector2 movementDir;
		if (isInputEnabled)
		{
			// Get mouse or touch input
#if UNITY_ANDROID || UNITY_IOS
			touchInputHandler.ListenForTouchInput();
#else
			HandleMouseKeyboardInput();
#endif
		}
	}

	public void HandleMouseKeyboardInput()
	{
		Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition) * INPUT_POSITION_SCALAR;
		if (Input.GetMouseButtonDown(0))
		{
			if (EventSystem.current.IsPointerOverGameObject())
				return;
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

	private void SetDirTouchBegan(Vector3 pos) {
		player.dir = pos - transform.position;
		player.hero.HandleTouchBegan();
	}

	private void SetDirTouchEnded(Vector3 pos) {
		player.dir = pos - transform.position;
		player.hero.HandleTouchEnded();
	}

	private void SetDirDragBegan(Vector3 dir) {
		player.dir = dir;
		player.hero.HandleDragBegan();
	}

	private void SetDirDragHold(Vector3 dir) {
		player.dir = dir;
		player.hero.HandleDragHold();
	}

	private void SetDirDragRelease(Vector3 dir) {
		player.dir = dir;
		//Debug.DrawRay(transform.position, dir, Color.white, 0.5f);
		player.hero.HandleDragRelease();
	}

	private void SetDirTapHold(Vector3 pos) {
		player.dir = pos - transform.position;
		player.hero.HandleHoldDown();
	}

	private void SetDirTap(Vector3 pos) {
		player.dir = pos - transform.position;
		player.hero.HandleTap();
	}

	private void SetDirTapHoldRelease(Vector3 pos) {
		player.dir = pos - transform.position;
		player.hero.HandleTapHoldRelease();
	}
}