using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TouchInputHandler : MonoBehaviour {

	private float startTime;		// the time at which the touch has started to move
	private bool startedMove;		// whether the touch has started moving (use to initialized startTime)

	private Vector2 startPos;
	private bool couldBeSwipe;
	private float minSwipeDist = 1.0f;
	private float maxSwipeTime = 1.5f;

	private bool couldBeTap;
	//private float maxTapDist = 1.0f;
	private float maxTapTime = 0.5f;

	public delegate void DirectionalTouchInput(Vector3 vec);
	public event DirectionalTouchInput OnDragRelease;
	public event DirectionalTouchInput OnTapHold;
	public event DirectionalTouchInput OnDragMove;
	public event DirectionalTouchInput OnTapRelease;
	public delegate void TouchInput();
	public event TouchInput MultiTouch;


	// Update is called once per frame
	public void ListenForTouchInput () 
	{
		if (Input.touchCount > 0)	// user touched the screen
		{
			if (IsPointerOverUIObject ())
				return;
			Touch touch = Input.touches[0];
			if (Input.touchCount >= 2)
				MultiTouch();

			// if touch began
			switch (touch.phase)
			{
				case (TouchPhase.Began):
					startPos = Camera.main.ScreenToWorldPoint(touch.position);
					startTime = Time.time;
					Debug.Log("Touch began:" + startPos);
					break;
				case (TouchPhase.Moved):
					Vector2 curPos = Camera.main.ScreenToWorldPoint(touch.position);
					Vector2 swipeDir = curPos - startPos;
					float swipeDist = (curPos - startPos).magnitude;
					couldBeSwipe = swipeDist > minSwipeDist;
					if (couldBeSwipe)
					{
						OnDragMove(swipeDir);
					}
					break;
				case (TouchPhase.Ended):
					//startedMove = false;

					float swipeTime = Time.time - startTime;
					curPos = Camera.main.ScreenToWorldPoint(touch.position);
					swipeDir = curPos - startPos;
					swipeDist = (curPos - startPos).magnitude;

					/*Debug.Log ("swipe time: " + swipeTime + "\n" + 
							  "swipe dist: " + swipeDist);*/
					couldBeSwipe = /*swipeTime < maxSwipeTime && */swipeDist > minSwipeDist;
					if (couldBeSwipe)
					{
						//Debug.Log ("Swipe");
						couldBeSwipe = false;
						OnDragRelease(swipeDir);
					}
					else if (swipeTime < maxTapTime)
						OnTapRelease(Camera.main.ScreenToWorldPoint(touch.position));

					break;
			}

/*			// if touch started moving, begin listening for swipe
			if (touch.deltaPosition.magnitude > maxTapDist)
			{
				if (!startedMove)
				{
					startedMove = true;
					startTime = Time.time;
					Debug.Log ("StartMove");
				}
			}
			else
			{
				if (OnTapHold != null)
					OnTapHold (Camera.main.ScreenToWorldPoint(touch.position));
			}*/
		}
	}

	private bool IsPointerOverUIObject() 
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}