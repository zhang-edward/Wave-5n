using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TouchInputHandler : MonoBehaviour {

	private Vector2 touchStartPos;
	private float   touchStartTime;		// The time at which the touch has started to move
	private float   minDragDist = 0.5f;
	private float   maxTapTime  = 0.3f;
	private bool 	isDragging;
	private bool 	touchStarted;

	public delegate void DirectionalTouchInput(Vector3 vec);
	public event DirectionalTouchInput OnTouchBegan;		// General touch begin
	public event DirectionalTouchInput OnTouchEnded;		// General touch end
	public event DirectionalTouchInput OnDragBegan;
	public event DirectionalTouchInput OnDragMove;
	public event DirectionalTouchInput OnDragRelease;
	public event DirectionalTouchInput OnTapHold;
	public event DirectionalTouchInput OnTapHoldRelease;
	public event DirectionalTouchInput OnTap;
	public delegate void TouchInput();
	public event TouchInput OnDragCancel;
	public event TouchInput MultiTouch;


	// Update is called once per frame
	public void ListenForTouchInput () 
	{
		if (Input.touchCount > 0)	// user touched the screen
		{
			Touch touch = Input.touches[0];
			if (Input.touchCount >= 2)
				MultiTouch();

			// if touch began
			switch (touch.phase)
			{
				case (TouchPhase.Began):
					if (IsPointerOverUIObject())
						return;
					HandleTouchBegan(Camera.main.ScreenToViewportPoint(touch.position * PlayerInput.INPUT_POSITION_SCALAR));
					break;
				case (TouchPhase.Moved):
					HandleTouchMoved(Camera.main.ScreenToViewportPoint(touch.position * PlayerInput.INPUT_POSITION_SCALAR));
					break;
				case (TouchPhase.Stationary):
					HandleTouchHeld (Camera.main.ScreenToViewportPoint(touch.position * PlayerInput.INPUT_POSITION_SCALAR));
					break;
				case (TouchPhase.Ended):
					HandleTouchEnded(Camera.main.ScreenToViewportPoint(touch.position * PlayerInput.INPUT_POSITION_SCALAR));
					break;
			}
		}
	}

	public void HandleTouchBegan(Vector2 viewportPos)
	{
		touchStarted = true;
		touchStartPos = viewportPos;
		touchStartTime = Time.time;
		if (OnTouchBegan != null)
			OnTouchBegan(Camera.main.ViewportToWorldPoint(viewportPos / PlayerInput.INPUT_POSITION_SCALAR));
		//Debug.Log("Touch began:" + touchStartPos);
	}

	public void HandleTouchMoved(Vector2 viewportPos)
	{
		if (!touchStarted)
			return;
		Vector2 touchDir = viewportPos - touchStartPos;
		float touchDist = (viewportPos - touchStartPos).magnitude;
		if (touchDist > minDragDist)
		{
			if (!isDragging)
			{
				isDragging = true;
				OnDragBegan(touchDir);
			}
			else
				OnDragMove(touchDir);
		}
		else if (isDragging)
		{
			isDragging = false;
			OnDragCancel();
		}
	}

	public void HandleTouchHeld(Vector2 viewportPos)
	{
		if (!touchStarted)
			return;
		float touchTime = Time.time - touchStartTime;
		if (touchTime > maxTapTime)
			OnTapHold(Camera.main.ViewportToWorldPoint(viewportPos / PlayerInput.INPUT_POSITION_SCALAR));
	}

	public void HandleTouchEnded(Vector2 viewportPos)
	{
		if (!touchStarted)
			return;
		Vector2 touchDir = viewportPos - touchStartPos;
		float touchTime = Time.time - touchStartTime;

		if (isDragging)
			OnDragRelease(touchDir);
		else if (!isDragging)
		{
			if (touchTime > maxTapTime)
				OnTapHoldRelease(Camera.main.ViewportToWorldPoint(viewportPos / PlayerInput.INPUT_POSITION_SCALAR));
			else
				OnTap(Camera.main.ViewportToWorldPoint(viewportPos / PlayerInput.INPUT_POSITION_SCALAR));
		}
		if (OnTouchEnded != null)
			OnTouchEnded(Camera.main.ViewportToWorldPoint(viewportPos / PlayerInput.INPUT_POSITION_SCALAR));

		touchStarted = false;
		isDragging = false;
	}

	private bool IsPointerOverUIObject() 
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public void ClearListeners() {
		OnDragBegan			= null;
		OnDragMove 			= null;
		OnDragRelease 		= null;
		OnTap 				= null;
		OnTapHold 			= null;
		OnTapHoldRelease	= null;
		MultiTouch 			= null;
		OnDragCancel 		= null;
	}
}