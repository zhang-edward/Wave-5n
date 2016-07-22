using UnityEngine;
using System.Collections;

public class TouchInputHandler : MonoBehaviour {

	private float startTime;		// the time at which the touch has started to move
	private bool startedMove;		// whether the touch has started moving (use to initialized startTime)

	private Vector2 startPos;
	private bool couldBeSwipe;
	private float minSwipeDist = 2.0f;
	private float maxSwipeTime = 0.3f;

	private bool couldBeTap;
	private float maxTapDist = 1.0f;
	private float maxTapTime = 0.5f;

	public delegate void Swipe(Vector2 dir);
	public event Swipe OnSwipe;

	public delegate void TapHold();
	public event TapHold OnTapHold;


	// Update is called once per frame
	public void ListenForTouchInput () 
	{
		if (Input.touchCount > 0)	// user touched the screen
		{
			Touch touch = Input.touches[0];

			// if touch started moving, begin listening for swipe
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
					OnTapHold ();
			}

			// if touch began
			switch (touch.phase)
			{
			case (TouchPhase.Began):
				couldBeSwipe = true;
				startPos = Camera.main.ScreenToWorldPoint(touch.position);
				break;
			case (TouchPhase.Ended):
				startedMove = false;

				float swipeTime = Time.time - startTime;
				Vector2 curPos = Camera.main.ScreenToWorldPoint (touch.position);
				Vector2 swipeDir = curPos - startPos;
				float swipeDist = (curPos - startPos).magnitude;

				//Debug.Log ("swipe time: " + swipeTime + "\n" + 
				//          "swipe dist: " + swipeDist);

				couldBeSwipe = swipeTime < maxSwipeTime && swipeDist > minSwipeDist;
				if (couldBeSwipe)
				{
					//Debug.Log ("Swipe");
					couldBeSwipe = false;
					OnSwipe (swipeDir);
				}
				break;
			}
		}
	}
}