using UnityEngine;
using System.Collections;

public class AdvicePointer : MonoBehaviour {

	public ScrollViewSnap scrollView;
	public Animator anim;

	void OnEnable()
	{
		scrollView.OnEndDrag += StartAdvicePointer;
	}

	private void StartAdvicePointer()
	{
		transform.SetAsLastSibling ();
		StopAllCoroutines ();
		StartCoroutine(AdvicePointerRoutine());
	}

	private IEnumerator AdvicePointerRoutine()
	{
		yield return new WaitForSeconds (3.0f);
		anim.SetTrigger ("Animate");
	}
}
