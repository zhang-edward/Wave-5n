using UnityEngine;
using System.Collections;

public class SwipeInputHandler : MonoBehaviour {

	//private float startTime;
	private Vector2 startPos;
	private bool couldBeSwipe;
	private float minSwipeDist = 2.0f;
	//private float maxSwipeTime = 0.3f;

	private bool couldBeTap;
	private float maxTapDist = 1.0f;
	private float maxTapTime = 0.5f;

	public delegate void Swipe(Vector2 dir);
	public event Swipe OnSwipe;

	/*	private Vector2 startPosX;
	private float xInterval = 3.0f;*/

	// Update is called once per frame
	void Update () 
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];

			// if touch began
			if (touch.phase == TouchPhase.Began)
			{
				couldBeSwipe = true;
				startPos = Camera.main.ScreenToWorldPoint(touch.position);
				//startTime = Time.time;
			}
			// if touch didn't move, touch is not a swipe
			else if (touch.phase == TouchPhase.Stationary)
				couldBeSwipe = false;
			else if (touch.phase == TouchPhase.Ended)
			{
				//float swipeTime = Time.time - startTime;
				Vector2 curPos = Camera.main.ScreenToWorldPoint(touch.position);
				Vector2 swipeDir = curPos - startPos;
				float swipeDist = (curPos - startPos).magnitude;

				//Debug.Log ("swipe time: " + swipeTime + "\n" + 
				//          "swipe dist: " + swipeDist);

				couldBeSwipe = swipeDist > minSwipeDist;
				if (couldBeSwipe)
				{
					//Debug.Log ("Swipe");
					couldBeSwipe = false;
					OnSwipe (swipeDir);
				}
			}
		}
	}
}