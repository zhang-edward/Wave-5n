using UnityEngine;
using System.Collections;

public class AdvicePointer : MonoBehaviour {

	public Animator anim;
	private float timer = 6f;

	void Update()
	{
		timer -= Time.deltaTime;
		if (Input.GetMouseButtonDown(0))
		{
			timer = 10f;
		}
		if (timer <= 0)
		{
			StartAdvicePointer ();
			timer = 6f;
		}
	}

	private void StartAdvicePointer()
	{
		transform.SetAsLastSibling ();
		anim.SetTrigger ("Animate");
	}
}
